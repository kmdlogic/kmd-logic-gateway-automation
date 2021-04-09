using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.Client.Models;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;

namespace Kmd.Logic.Gateway.Automation.Client
{
    /// <summary>
    /// Custom extensions to GatewayClient.
    /// </summary>
    internal partial interface IGatewayClient
    {
        Task<object> CustomCreateProductAsync(System.Guid subscriptionId, string name, string key, string description = default(string), string providerId = default(string), bool? apiKeyRequired = default(bool?), bool? providerApprovalRequired = default(bool?), string productTerms = default(string), string visibility = default(string), IList<System.Guid?> apiIds = default(IList<System.Guid?>), Stream logo = default(Stream), Stream documentation = default(Stream), bool? clientCredentialRequired = default(bool?), string openidConfigIssuer = default(string), string openidConfigCustomUrl = default(string), string applicationId = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<object> CustomUpdateProductAsync(System.Guid subscriptionId, System.Guid productId, string name = default(string), string key = default(string), string description = default(string), string providerId = default(string), bool? apiKeyRequired = default(bool?), bool? clientCredentialRequired = default(bool?), string openidConfigIssuer = default(string), string openidConfigCustomUrl = default(string), bool? providerApprovalRequired = default(bool?), string applicationId = default(string), string productTerms = default(string), string visibility = default(string), IList<System.Guid?> apiIds = default(IList<System.Guid?>), Stream logo = default(Stream), Stream documentation = default(Stream), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Custom extensions of GatewayClient.
    /// </summary>
    internal partial class GatewayClient : IGatewayClient
    {
        public async Task<object> CustomCreateProductAsync(System.Guid subscriptionId, string name, string key, string description = default(string), string providerId = default(string), bool? apiKeyRequired = default(bool?), bool? providerApprovalRequired = default(bool?), string productTerms = default(string), string visibility = default(string), IList<System.Guid?> apiIds = default(IList<System.Guid?>), Stream logo = default(Stream), Stream documentation = default(Stream), bool? clientCredentialRequired = default(bool?), string openidConfigIssuer = default(string), string openidConfigCustomUrl = default(string), string applicationId = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var result = await this.CustomCreateProductWithHttpMessagesAsync(subscriptionId, name, key, description, providerId, apiKeyRequired, providerApprovalRequired, productTerms, visibility, apiIds, logo, documentation, clientCredentialRequired, openidConfigIssuer, openidConfigCustomUrl, applicationId, null, cancellationToken).ConfigureAwait(false))
            {
                return result.Body;
            }
        }

        public async Task<object> CustomUpdateProductAsync(System.Guid subscriptionId, System.Guid productId, string name = default(string), string key = default(string), string description = default(string), string providerId = default(string), bool? apiKeyRequired = default(bool?), bool? clientCredentialRequired = default(bool?), string openidConfigIssuer = default(string), string openidConfigCustomUrl = default(string), bool? providerApprovalRequired = default(bool?), string applicationId = default(string), string productTerms = default(string), string visibility = default(string), IList<System.Guid?> apiIds = default(IList<System.Guid?>), Stream logo = default(Stream), Stream documentation = default(Stream), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var result = await this.CustomUpdateProductWithHttpMessagesAsync(subscriptionId, productId, name, key, description, providerId, apiKeyRequired, clientCredentialRequired, openidConfigIssuer, openidConfigCustomUrl, providerApprovalRequired, applicationId, productTerms, visibility, apiIds, logo, documentation, null, cancellationToken).ConfigureAwait(false))
            {
                return result.Body;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Can't address. Has implications!!")]
        private async Task<HttpOperationResponse<object>> CustomCreateProductWithHttpMessagesAsync(System.Guid subscriptionId, string name, string key, string description = default(string), string providerId = default(string), bool? apiKeyRequired = default(bool?), bool? providerApprovalRequired = default(bool?), string productTerms = default(string), string visibility = default(string), IList<System.Guid?> apiIds = default(IList<System.Guid?>), Stream logo = default(Stream), Stream documentation = default(Stream), bool? clientCredentialRequired = default(bool?), string openidConfigIssuer = default(string), string openidConfigCustomUrl = default(string), string applicationId = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "name");
            }

            // Tracing
            bool shouldTrace = ServiceClientTracing.IsEnabled;
            string invocationId = null;
            if (shouldTrace)
            {
                invocationId = ServiceClientTracing.NextInvocationId.ToString(CultureInfo.InvariantCulture);
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("subscriptionId", subscriptionId);
                tracingParameters.Add("key", key);
                tracingParameters.Add("name", name);
                tracingParameters.Add("description", description);
                tracingParameters.Add("providerId", providerId);
                tracingParameters.Add("apiKeyRequired", apiKeyRequired);
                tracingParameters.Add("providerApprovalRequired", providerApprovalRequired);
                tracingParameters.Add("productTerms", productTerms);
                tracingParameters.Add("visibility", visibility);
                tracingParameters.Add("apiIds", apiIds);
                tracingParameters.Add("logo", logo);
                tracingParameters.Add("documentation", documentation);
                tracingParameters.Add("clientCredentialRequired", clientCredentialRequired);
                tracingParameters.Add("openidConfigIssuer", openidConfigIssuer);
                tracingParameters.Add("openidConfigCustomUrl", openidConfigCustomUrl);
                tracingParameters.Add("applicationId", applicationId);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(invocationId, this, "CreateProduct", tracingParameters);
            }

            // Construct URL
            string url = this.ConstructUrl(subscriptionId);

            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            HttpResponseMessage httpResponse = null;
            httpRequest.Method = new HttpMethod("POST");
            httpRequest.RequestUri = new System.Uri(url);

            // Set Headers
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    if (httpRequest.Headers.Contains(header.Key))
                    {
                        httpRequest.Headers.Remove(header.Key);
                    }

                    httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // Serialize Request
            string requestContent = null;
            MultipartFormDataContent multiPartContent = new MultipartFormDataContent();
            if (name != null)
            {
                StringContent nameContent = new StringContent(name, System.Text.Encoding.UTF8);
                multiPartContent.Add(nameContent, "name");
            }

            if (key != null)
            {
                StringContent keyContent = new StringContent(key, System.Text.Encoding.UTF8);
                multiPartContent.Add(keyContent, "key");
            }

            if (description != null)
            {
                StringContent descriptionContent = new StringContent(description, System.Text.Encoding.UTF8);
                multiPartContent.Add(descriptionContent, "description");
            }

            if (apiKeyRequired != null)
            {
                StringContent isApiKeyRequired = new StringContent(SafeJsonConvert.SerializeObject(apiKeyRequired, this.SerializationSettings).Trim('"'), System.Text.Encoding.UTF8);
                multiPartContent.Add(isApiKeyRequired, "apiKeyRequired");
            }

            if (providerApprovalRequired != null)
            {
                StringContent isProviderApprovalRequired = new StringContent(SafeJsonConvert.SerializeObject(providerApprovalRequired, this.SerializationSettings).Trim('"'), System.Text.Encoding.UTF8);
                multiPartContent.Add(isProviderApprovalRequired, "providerApprovalRequired");
            }

            if (providerId != null)
            {
                StringContent providerIdContent = new StringContent(providerId, System.Text.Encoding.UTF8);
                multiPartContent.Add(providerIdContent, "providerId");
            }

            if (productTerms != null)
            {
                StringContent productTermsContent = new StringContent(productTerms, System.Text.Encoding.UTF8);
                multiPartContent.Add(productTermsContent, "productTerms");
            }

            if (visibility != null)
            {
                StringContent visibilityContent = new StringContent(SafeJsonConvert.SerializeObject(visibility, this.SerializationSettings).Trim('"'), System.Text.Encoding.UTF8);
                multiPartContent.Add(visibilityContent, "visibility");
            }

            if (clientCredentialRequired != null)
            {
                StringContent clientCredentialRequiredContent = new StringContent(SafeJsonConvert.SerializeObject(clientCredentialRequired, this.SerializationSettings).Trim('"'), System.Text.Encoding.UTF8);
                multiPartContent.Add(clientCredentialRequiredContent, "clientCredentialRequired");
            }

            if (openidConfigIssuer != null)
            {
                StringContent productTermsContent = new StringContent(productTerms, System.Text.Encoding.UTF8);
                multiPartContent.Add(productTermsContent, "productTerms");
            }

            if (openidConfigCustomUrl != null)
            {
                StringContent openidConfigCustomUrlContent = new StringContent(openidConfigCustomUrl, System.Text.Encoding.UTF8);
                multiPartContent.Add(openidConfigCustomUrlContent, "openidConfigCustomUrl");
            }

            if (applicationId != null)
            {
                StringContent applicationIdContent = new StringContent(applicationId, System.Text.Encoding.UTF8);
                multiPartContent.Add(applicationIdContent, "applicationId");
            }

            if (apiIds != null)
            {
                int i = 0;
                foreach (var apiId in apiIds)
                {
                    var productIdContent = new StringContent(apiId.ToString());
                    multiPartContent.Add(productIdContent, $"apiIds[{i}]");
                    i++;
                }
            }

            if (logo != null)
            {
                StreamContent logoContent = new StreamContent(logo);
                logoContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                ContentDispositionHeaderValue logoContentDispositionHeaderValue = new ContentDispositionHeaderValue("form-data");
                logoContentDispositionHeaderValue.Name = "logo";

                // get filename from stream if it's a file otherwise, just use  'unknown'
                var logoFileStream = logo as FileStream;
                var logoFileName = (logoFileStream != null ? logoFileStream.Name : null) ?? "unknown";
                if (System.Linq.Enumerable.Any(logoFileName, c => c > 127))
                {
                    // non ASCII chars detected, need UTF encoding:
                    logoContentDispositionHeaderValue.FileNameStar = logoFileName;
                }
                else
                {
                    // ASCII only
                    logoContentDispositionHeaderValue.FileName = logoFileName;
                }

                logoContent.Headers.ContentDisposition = logoContentDispositionHeaderValue;
                multiPartContent.Add(logoContent, "logo");
            }

            if (documentation != null)
            {
                StreamContent documentationContent = new StreamContent(documentation);
                documentationContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                ContentDispositionHeaderValue documentationContentDispositionHeaderValue = new ContentDispositionHeaderValue("form-data");
                documentationContentDispositionHeaderValue.Name = "documentation";

                // get filename from stream if it's a file otherwise, just use  'unknown'
                var documentationfileStream = documentation as FileStream;
                var documentationfileName = (documentationfileStream != null ? documentationfileStream.Name : null) ?? "unknown";
                if (System.Linq.Enumerable.Any(documentationfileName, c => c > 127))
                {
                    // non ASCII chars detected, need UTF encoding:
                    documentationContentDispositionHeaderValue.FileNameStar = documentationfileName;
                }
                else
                {
                    // ASCII only
                    documentationContentDispositionHeaderValue.FileName = documentationfileName;
                }

                documentationContent.Headers.ContentDisposition = documentationContentDispositionHeaderValue;
                multiPartContent.Add(documentationContent, "documentation");
            }

            httpRequest.Content = multiPartContent;

            // Set Credentials
            if (this.Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await this.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }

            // Send Request
            if (shouldTrace)
            {
                ServiceClientTracing.SendRequest(invocationId, httpRequest);
            }

            cancellationToken.ThrowIfCancellationRequested();
            httpResponse = await this.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            if (shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(invocationId, httpResponse);
            }

            HttpStatusCode statusCode = httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string responseContent = null;

            if ((int)statusCode != 201 && (int)statusCode != 400 && (int)statusCode != 409 && (int)statusCode != 422)
            {
                var ex = new HttpOperationException($"Operation returned an invalid status code '{statusCode}'");
                if (httpResponse.Content != null)
                {
                    responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    responseContent = string.Empty;
                }

                ex.Request = new HttpRequestMessageWrapper(httpRequest, requestContent);
                ex.Response = new HttpResponseMessageWrapper(httpResponse, responseContent);
                if (shouldTrace)
                {
                    ServiceClientTracing.Error(invocationId, ex);
                }

                httpRequest.Dispose();
                if (httpResponse != null)
                {
                    httpResponse.Dispose();
                }

                throw ex;
            }

            // Create Result
            var result = new HttpOperationResponse<object>();
            result.Request = httpRequest;
            result.Response = httpResponse;

            // Deserialize Response
            if (statusCode == HttpStatusCode.Created)
            {
                responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    result.Body = SafeJsonConvert.DeserializeObject<ProductListModel>(responseContent, this.DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    httpRequest.Dispose();
                    if (httpResponse != null)
                    {
                        httpResponse.Dispose();
                    }

                    throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                }
            }

            // Deserialize Response
            if (statusCode == HttpStatusCode.BadRequest || statusCode == HttpStatusCode.Conflict || (int)statusCode == 422)
            {
                responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    result.Body = SafeJsonConvert.DeserializeObject<string>(responseContent, this.DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    httpRequest.Dispose();
                    if (httpResponse != null)
                    {
                        httpResponse.Dispose();
                    }

                    throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                }
            }

            if (shouldTrace)
            {
                ServiceClientTracing.Exit(invocationId, result);
            }

            return result;
        }

        private string ConstructUrl(Guid subscriptionId)
        {
            var baseUrl = this.BaseUri.AbsoluteUri;
            var url = new System.Uri(new System.Uri(baseUrl + (baseUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? string.Empty : "/")), "subscriptions/{subscriptionId}/gateway/products").ToString();
            url = url.Replace("{subscriptionId}", System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(subscriptionId, this.SerializationSettings).Trim('"')));
            List<string> queryParameters = new List<string>();

            if (queryParameters.Count > 0)
            {
                url += "?" + string.Join("&", queryParameters);
            }

            return url;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "This code is from auto genrated code")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "This code is from auto genrated code")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "This code is from auto genrated code")]

        public async Task<HttpOperationResponse<object>> CustomUpdateProductWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid productId, string name = default(string), string key = default(string), string description = default(string), string providerId = default(string), bool? apiKeyRequired = default(bool?), bool? clientCredentialRequired = default(bool?), string openidConfigIssuer = default(string), string openidConfigCustomUrl = default(string), bool? providerApprovalRequired = default(bool?), string applicationId = default(string), string productTerms = default(string), string visibility = default(string), IList<System.Guid?> apiIds = default(IList<System.Guid?>), Stream logo = default(Stream), Stream documentation = default(Stream), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Tracing
            bool shouldTrace = ServiceClientTracing.IsEnabled;
            string invocationId = null;
            if (shouldTrace)
            {
                invocationId = ServiceClientTracing.NextInvocationId.ToString(CultureInfo.InvariantCulture);
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("subscriptionId", subscriptionId);
                tracingParameters.Add("key", key);
                tracingParameters.Add("name", name);
                tracingParameters.Add("description", description);
                tracingParameters.Add("providerId", providerId);
                tracingParameters.Add("apiKeyRequired", apiKeyRequired);
                tracingParameters.Add("providerApprovalRequired", providerApprovalRequired);
                tracingParameters.Add("productTerms", productTerms);
                tracingParameters.Add("visibility", visibility);
                tracingParameters.Add("apiIds", apiIds);
                tracingParameters.Add("logo", logo);
                tracingParameters.Add("documentation", documentation);
                tracingParameters.Add("clientCredentialRequired", clientCredentialRequired);
                tracingParameters.Add("openidConfigIssuer", openidConfigIssuer);
                tracingParameters.Add("openidConfigCustomUrl", openidConfigCustomUrl);
                tracingParameters.Add("applicationId", applicationId);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(invocationId, this, "UpdateProduct", tracingParameters);
            }

            // Construct URL
            var baseUrl = this.BaseUri.AbsoluteUri;
            var url = new System.Uri(new System.Uri(baseUrl + (baseUrl.EndsWith("/") ? string.Empty : "/")), "subscriptions/{subscriptionId}/gateway/products/{productId}").ToString();
            url = url.Replace("{subscriptionId}", System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(subscriptionId, this.SerializationSettings).Trim('"')));
            url = url.Replace("{productId}", System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(productId, this.SerializationSettings).Trim('"')));

            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            HttpResponseMessage httpResponse = null;
            httpRequest.Method = new HttpMethod("PUT");
            httpRequest.RequestUri = new System.Uri(url);

            // Set Headers
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    if (httpRequest.Headers.Contains(header.Key))
                    {
                        httpRequest.Headers.Remove(header.Key);
                    }

                    httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // Serialize Request
            string requestContent = null;
            MultipartFormDataContent multiPartContent = new MultipartFormDataContent();
            if (name != null)
            {
                StringContent nameContent = new StringContent(name, System.Text.Encoding.UTF8);
                multiPartContent.Add(nameContent, "name");
            }

            if (key != null)
            {
                StringContent keyContent = new StringContent(key, System.Text.Encoding.UTF8);
                multiPartContent.Add(keyContent, "key");
            }

            if (description != null)
            {
                StringContent descriptionContent = new StringContent(description, System.Text.Encoding.UTF8);
                multiPartContent.Add(descriptionContent, "description");
            }

            if (providerId != null)
            {
                StringContent providerIdContent = new StringContent(providerId, System.Text.Encoding.UTF8);
                multiPartContent.Add(providerIdContent, "providerId");
            }

            if (apiKeyRequired != null)
            {
                StringContent isApiKeyRequired = new StringContent(SafeJsonConvert.SerializeObject(apiKeyRequired, this.SerializationSettings).Trim('"'), System.Text.Encoding.UTF8);
                multiPartContent.Add(isApiKeyRequired, "apiKeyRequired");
            }

            if (clientCredentialRequired != null)
            {
                StringContent clientCredentialRequiredContent = new StringContent(SafeJsonConvert.SerializeObject(clientCredentialRequired, this.SerializationSettings).Trim('"'), System.Text.Encoding.UTF8);
                multiPartContent.Add(clientCredentialRequiredContent, "clientCredentialRequired");
            }

            if (openidConfigIssuer != null)
            {
                StringContent productTermsContent = new StringContent(productTerms, System.Text.Encoding.UTF8);
                multiPartContent.Add(productTermsContent, "productTerms");
            }

            if (openidConfigCustomUrl != null)
            {
                StringContent openidConfigCustomUrlContent = new StringContent(openidConfigCustomUrl, System.Text.Encoding.UTF8);
                multiPartContent.Add(openidConfigCustomUrlContent, "openidConfigCustomUrl");
            }

            if (providerApprovalRequired != null)
            {
                StringContent isProviderApprovalRequired = new StringContent(SafeJsonConvert.SerializeObject(providerApprovalRequired, this.SerializationSettings).Trim('"'), System.Text.Encoding.UTF8);
                multiPartContent.Add(isProviderApprovalRequired, "providerApprovalRequired");
            }

            if (applicationId != null)
            {
                StringContent applicationIdContent = new StringContent(applicationId, System.Text.Encoding.UTF8);
                multiPartContent.Add(applicationIdContent, "applicationId");
            }

            if (productTerms != null)
            {
                StringContent productTermsContent = new StringContent(productTerms, System.Text.Encoding.UTF8);
                multiPartContent.Add(productTermsContent, "productTerms");
            }

            if (visibility != null)
            {
                StringContent visibilityContent = new StringContent(SafeJsonConvert.SerializeObject(visibility, this.SerializationSettings).Trim('"'), System.Text.Encoding.UTF8);
                multiPartContent.Add(visibilityContent, "visibility");
            }

            if (apiIds != null)
            {
                int i = 0;
                foreach (var apiId in apiIds)
                {
                    var productIdContent = new StringContent(apiId.ToString());
                    multiPartContent.Add(productIdContent, $"apiIds[{i}]");
                    i++;
                }
            }

            if (logo != null)
            {
                StreamContent logoContent = new StreamContent(logo);
                logoContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                ContentDispositionHeaderValue logoContentDispositionHeaderValue = new ContentDispositionHeaderValue("form-data");
                logoContentDispositionHeaderValue.Name = "logo";

                // get filename from stream if it's a file otherwise, just use  'unknown'
                var logoFileStream = logo as FileStream;
                var logoFileName = (logoFileStream != null ? logoFileStream.Name : null) ?? "unknown";
                if (System.Linq.Enumerable.Any(logoFileName, c => c > 127))
                {
                    // non ASCII chars detected, need UTF encoding:
                    logoContentDispositionHeaderValue.FileNameStar = logoFileName;
                }
                else
                {
                    // ASCII only
                    logoContentDispositionHeaderValue.FileName = logoFileName;
                }

                logoContent.Headers.ContentDisposition = logoContentDispositionHeaderValue;
                multiPartContent.Add(logoContent, "logo");
            }

            if (documentation != null)
            {
                StreamContent documentationContent = new StreamContent(documentation);
                documentationContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                ContentDispositionHeaderValue documentationContentDispositionHeaderValue = new ContentDispositionHeaderValue("form-data");
                documentationContentDispositionHeaderValue.Name = "documentation";

                // get filename from stream if it's a file otherwise, just use  'unknown'
                var documentationfileStream = documentation as FileStream;
                var documentationfileName = (documentationfileStream != null ? documentationfileStream.Name : null) ?? "unknown";
                if (System.Linq.Enumerable.Any(documentationfileName, c => c > 127))
                {
                    // non ASCII chars detected, need UTF encoding:
                    documentationContentDispositionHeaderValue.FileNameStar = documentationfileName;
                }
                else
                {
                    // ASCII only
                    documentationContentDispositionHeaderValue.FileName = documentationfileName;
                }

                documentationContent.Headers.ContentDisposition = documentationContentDispositionHeaderValue;
                multiPartContent.Add(documentationContent, "documentation");
            }

            httpRequest.Content = multiPartContent;

            // Set Credentials
            if (this.Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await this.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }

            // Send Request
            if (shouldTrace)
            {
                ServiceClientTracing.SendRequest(invocationId, httpRequest);
            }

            cancellationToken.ThrowIfCancellationRequested();
            httpResponse = await this.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            if (shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(invocationId, httpResponse);
            }

            HttpStatusCode statusCode = httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string responseContent = null;

            if ((int)statusCode != 200 && (int)statusCode != 400 && (int)statusCode != 404 && (int)statusCode != 409 && (int)statusCode != 422)
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", statusCode));
                if (httpResponse.Content != null)
                {
                    responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    responseContent = string.Empty;
                }

                ex.Request = new HttpRequestMessageWrapper(httpRequest, requestContent);
                ex.Response = new HttpResponseMessageWrapper(httpResponse, responseContent);
                if (shouldTrace)
                {
                    ServiceClientTracing.Error(invocationId, ex);
                }

                httpRequest.Dispose();
                if (httpResponse != null)
                {
                    httpResponse.Dispose();
                }

                throw ex;
            }

            // Create Result
            var result = new HttpOperationResponse<object>();
            result.Request = httpRequest;
            result.Response = httpResponse;

            // Deserialize Response
            if ((int)statusCode == 200)
            {
                responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    result.Body = SafeJsonConvert.DeserializeObject<ProductListModel>(responseContent, this.DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    httpRequest.Dispose();
                    if (httpResponse != null)
                    {
                        httpResponse.Dispose();
                    }

                    throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                }
            }

            // Deserialize Response
            if ((int)statusCode == 400)
            {
                responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    result.Body = SafeJsonConvert.DeserializeObject<string>(responseContent, this.DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    httpRequest.Dispose();
                    if (httpResponse != null)
                    {
                        httpResponse.Dispose();
                    }

                    throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                }
            }

            // Deserialize Response
            if ((int)statusCode == 409)
            {
                responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    result.Body = SafeJsonConvert.DeserializeObject<IDictionary<string, IList<string>>>(responseContent, this.DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    httpRequest.Dispose();
                    if (httpResponse != null)
                    {
                        httpResponse.Dispose();
                    }

                    throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                }
            }

            if (shouldTrace)
            {
                ServiceClientTracing.Exit(invocationId, result);
            }

            return result;
        }
    }
}
