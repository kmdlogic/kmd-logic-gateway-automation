using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Rest;

namespace Kmd.Logic.Gateway.Automation.Client
{
    /// <summary>
    /// Custom implementation of HTTP requesting Kmd.Logic.Gateway.ValidatePublishing endpoint.
    /// </summary>
    internal partial interface IGatewayClient
    {
        /// <summary>
        /// Endpoint interface.
        /// </summary>
        /// <param name="subscriptionId">Logic User subscription ID.</param>
        /// <param name="request">Validate publishing parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Validation result.</returns>
        Task<ValidatePublishingResult> ValidatePublishingAsync(
            Guid subscriptionId,
            ValidatePublishingRequest request,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Custom implementation of HTTP requesting Kmd.Logic.Gateway.ValidatePublishing endpoint.
    /// </summary>
    internal partial class GatewayClient : IGatewayClient
    {
        /// <summary>
        /// Endpoint implementation.
        /// </summary>
        /// <param name="subscriptionId">Logic User subscription ID.</param>
        /// <param name="request">Validate publishing parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidatePublishingResult> ValidatePublishingAsync(
            Guid subscriptionId,
            ValidatePublishingRequest request,
            CancellationToken cancellationToken = default)
        {
            ValidateRequest(subscriptionId, request);

            // Construct URL
            var baseUrl = this.BaseUri.AbsoluteUri;
#pragma warning disable CA1307 // Specify StringComparison
            var url = new Uri(new Uri($"{baseUrl}{(baseUrl.EndsWith("/") ? string.Empty : "/")}"), $"subscriptions/{subscriptionId}/gateway/publishing/validate");
#pragma warning restore CA1307 // Specify StringComparison

            // Create HTTP transport objects
            using var requestContent = await CreateRequestContent(request).ConfigureAwait(false);
            using var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = url,
                Content = requestContent,
            };

            // Set Credentials
            if (this.Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await this.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }

            var response = await this.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<ValidatePublishingResult>(cancellationToken).ConfigureAwait(false);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new HttpOperationException($"Operation finished with status code {response.StatusCode}");
            }

            var responseMessage = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new HttpOperationException($"Operation finished with status code {response.StatusCode}: {responseMessage}");
        }

#pragma warning disable CA2000 // Dispose objects before losing scope
        private static async Task<MultipartFormDataContent> CreateRequestContent(ValidatePublishingRequest request)
        {
            // Serialize the request
            var requestContent = new MultipartFormDataContent
            {
                { new StringContent($"{request.ProviderId}"), $"ProviderId" },
            };

            // Add APIs
            for (int apiIndex = 0; apiIndex < request.Apis?.Count(); apiIndex++)
            {
                var api = request.Apis.ElementAt(apiIndex);
                await AddApiContent(requestContent, api, apiIndex).ConfigureAwait(false);

                // Add Revisions
                for (int revIndex = 0; revIndex < api.Revisions?.Count(); revIndex++)
                {
                    var rev = api.Revisions.ElementAt(revIndex);
                    await AddApiRevisionContent(requestContent, rev, apiIndex, revIndex).ConfigureAwait(false);
                }

                // Add Policies
                AddPolicies(requestContent, $"Apis[{apiIndex}]", api.Policies);
            }

            // Add Products
            for (int productIndex = 0; productIndex < request.Products?.Count(); productIndex++)
            {
                var product = request.Products.ElementAt(productIndex);
                AddProductContent(requestContent, product, productIndex);

                // Add Policies
                AddPolicies(requestContent, $"Products[{productIndex}]", product.Policies);
            }

            return requestContent;
        }

        private static async Task AddApiRevisionContent(MultipartFormDataContent requestContent, ApiRevisionValidationModel rev, int apiIndex, int revIndex)
        {
            requestContent.Add(new StringContent(rev.Description), $"Apis[{apiIndex}].Revisions[{revIndex}].Description");

            using var ms = new MemoryStream();
            await rev.OpenApiSpec.CopyToAsync(ms).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            var openApiSpec = new ByteArrayContent(ms.ToArray());
            requestContent.Add(openApiSpec, $"Apis[{apiIndex}].Revisions[{revIndex}].OpenApiSpec", "swagger.json");
        }

        private static void AddProductContent(MultipartFormDataContent requestContent, ProductValidationModel product, int productIndex)
        {
            requestContent.Add(new StringContent(product.Name), $"Products[{productIndex}].Name");
            if (product.Key != null)
            {
                requestContent.Add(new StringContent(product.Key), $"Products[{productIndex}].Key");
            }
        }

        private static async Task AddApiContent(MultipartFormDataContent requestContent, ApiValidationModel api, int apiIndex)
        {
            requestContent.Add(new StringContent(api.Name), $"Apis[{apiIndex}].Name");
            requestContent.Add(new StringContent(api.Path), $"Apis[{apiIndex}].Path");
            requestContent.Add(new StringContent(api.Version), $"Apis[{apiIndex}].Version");

            using var ms = new MemoryStream();
            await api.OpenApiSpec.CopyToAsync(ms).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            var openApiSpec = new ByteArrayContent(ms.ToArray());
            requestContent.Add(openApiSpec, $"Apis[{apiIndex}].OpenApiSpec", "swagger.json");

            for (int productNameIndex = 0; productNameIndex < api.ProductNames?.Count(); productNameIndex++)
            {
                requestContent.Add(
                    new StringContent(api.ProductNames.ElementAt(productNameIndex)),
                    $"Apis[{apiIndex}].ProductNames[{productNameIndex}]");
            }
        }

        private static void AddPolicies(MultipartFormDataContent requestContent, string rootPrefix, PoliciesValidationModel policies)
        {
            if (policies.RateLimitPolicy != null)
            {
                requestContent.Add(new StringContent(policies.RateLimitPolicy.Name), $"{rootPrefix}.Policies.RateLimitPolicy.Name");
                requestContent.Add(new StringContent($"{policies.RateLimitPolicy.Calls}"), $"{rootPrefix}.Policies.RateLimitPolicy.Calls");
                requestContent.Add(new StringContent($"{policies.RateLimitPolicy.RenewalPeriod}"), $"{rootPrefix}.Policies.RateLimitPolicy.RenewalPeriod");
            }

            for (int cpIndex = 0; cpIndex < policies.CustomPolicies.Count(); cpIndex++)
            {
                var customPolicy = policies.CustomPolicies.ElementAt(cpIndex);
                requestContent.Add(new StringContent(customPolicy.Name), $"{rootPrefix}.Policies.CustomPolicies[{cpIndex}].Name");
                requestContent.Add(new StringContent(customPolicy.Xml), $"{rootPrefix}.Policies.CustomPolicies[{cpIndex}].Xml");
            }
        }
#pragma warning restore CA2000 // Dispose objects before losing scope

        private static void ValidateRequest(Guid subscriptionId, ValidatePublishingRequest request)
        {
            if (subscriptionId == Guid.Empty)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "SubscriptionId");
            }

            if (request.ProviderId == Guid.Empty)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ProviderId");
            }

            foreach (var api in request.Apis)
            {
                if (string.IsNullOrEmpty(api.Name))
                {
                    throw new ValidationException(ValidationRules.CannotBeNull, "Apis.Name");
                }

                if (string.IsNullOrEmpty(api.Path))
                {
                    throw new ValidationException(ValidationRules.CannotBeNull, "Apis.Path");
                }

                if (string.IsNullOrEmpty(api.Version))
                {
                    throw new ValidationException(ValidationRules.CannotBeNull, "Apis.Version");
                }

                if (api.OpenApiSpec == null || api.OpenApiSpec.Length == 0)
                {
                    throw new ValidationException(ValidationRules.CannotBeNull, "Apis.OpenApiSpec");
                }

                if (api.Revisions != null)
                {
                    foreach (var rev in api.Revisions)
                    {
                        if (rev.OpenApiSpec == null || rev.OpenApiSpec.Length == 0)
                        {
                            throw new ValidationException(ValidationRules.CannotBeNull, "Revisions.OpenApiSpec");
                        }
                    }
                }

                if (api.Policies != null)
                {
                    ValidatePoliciesRequest(api.Policies);
                }
            }

            foreach (var product in request.Products)
            {
                if (string.IsNullOrEmpty(product.Name))
                {
                    throw new ValidationException(ValidationRules.CannotBeNull, "Products.Name");
                }

                if (product.Policies != null)
                {
                    ValidatePoliciesRequest(product.Policies);
                }
            }
        }

        public static void ValidatePoliciesRequest(PoliciesValidationModel policiesValidationModel)
        {
            if (policiesValidationModel.RateLimitPolicy != null)
            {
                if (string.IsNullOrEmpty(policiesValidationModel.RateLimitPolicy.Name))
                {
                    throw new ValidationException(ValidationRules.CannotBeNull, "RateLimitPolicy.Name");
                }

                if (policiesValidationModel.RateLimitPolicy.Calls < 1)
                {
                    throw new ValidationException(ValidationRules.InclusiveMinimum, "RateLimitPolicy.Calls");
                }

                if (policiesValidationModel.RateLimitPolicy.RenewalPeriod < 1)
                {
                    throw new ValidationException(ValidationRules.InclusiveMinimum, "RateLimitPolicy.RenewalPeriod");
                }

                if (policiesValidationModel.RateLimitPolicy.RenewalPeriod > 300)
                {
                    throw new ValidationException(ValidationRules.InclusiveMaximum, "RateLimitPolicy.RenewalPeriod");
                }
            }

            if (policiesValidationModel.CustomPolicies != null)
            {
                foreach (var customPolicy in policiesValidationModel.CustomPolicies)
                {
                    if (string.IsNullOrEmpty(customPolicy.Name))
                    {
                        throw new ValidationException(ValidationRules.CannotBeNull, "CustomPolicy.Name");
                    }

                    if (string.IsNullOrEmpty(customPolicy.Xml))
                    {
                        throw new ValidationException(ValidationRules.CannotBeNull, "CustomPolicy.Xml");
                    }

                    try
                    {
                        XElement.Parse(customPolicy.Xml);
                    }
                    catch (XmlException xe)
                    {
                        throw new ValidationException("XmlError", "CustomPolicy.Xml", xe.Message);
                    }
                }
            }
        }
    }
}
