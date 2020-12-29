using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.Client;
using Kmd.Logic.Gateway.Automation.PreValidation;
using Kmd.Logic.Gateway.Automation.PublishFile;
using Kmd.Logic.Identity.Authorization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Kmd.Logic.Gateway.Automation
{
    internal class ValidatePublishing
    {
        private readonly GatewayClientFactory gatewayClientFactory;
        private readonly GatewayOptions options;
        private readonly HttpClient httpClient;
        private readonly LogicTokenProviderFactory logicTokenProviderFactory;

        public ValidatePublishing(HttpClient httpClient, LogicTokenProviderFactory tokenProviderFactory, GatewayOptions options)
        {
            this.options = options;
            this.gatewayClientFactory = new GatewayClientFactory(tokenProviderFactory, httpClient, options);
            this.httpClient = httpClient;
            this.logicTokenProviderFactory = tokenProviderFactory;
        }

        public async Task<ValidationResult> ValidateAsync(string folderPath)
        {
            var publishYml = File.ReadAllText(Path.Combine(folderPath, "publish.yml"));
            var publishFileModel = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build().Deserialize<PublishFileModel>(publishYml);

            if (publishFileModel != null && !publishFileModel.Products.Any() && !publishFileModel.Apis.Any())
            {
                return new ValidationResult(new List<GatewayAutomationResult>()
                {
                    new GatewayAutomationResult
                    {
                        IsError = true,
                        ResultCode = ResultCode.ValidationFailed,
                        Message = $"Both products and apis are empty in the file. Nothing to validate/publish",
                    },
                });
            }

            var preValidationsResults = PreValidateEntities(folderPath, publishFileModel, this.httpClient, this.options, this.logicTokenProviderFactory);
            if (preValidationsResults.Any(r => r.IsError))
            {
                return new ValidationResult(preValidationsResults);
            }

            using var client = this.gatewayClientFactory.CreateClient();
            using var validatePublishingRequest = await GetValidatePublishingRequest(folderPath, this.options.ProviderId, publishFileModel).ConfigureAwait(false);
            var validatePublishingResult = await client.ValidatePublishingAsync(
                this.options.SubscriptionId, validatePublishingRequest).ConfigureAwait(false);

            return validatePublishingResult.IsSuccess
                ? new ValidationResult(validatePublishingResult)
                : new ValidationResult(GetValidatePublishingErrors(validatePublishingResult));
        }

        private static IEnumerable<GatewayAutomationResult> GetValidatePublishingErrors(ValidatePublishingResult validatePublishingResult)
        {
            var results = new List<GatewayAutomationResult>();

            foreach (var api in validatePublishingResult.Apis)
            {
                if (api.Errors.Any())
                {
                    results.Add(new GatewayAutomationResult
                    {
                        ResultCode = ResultCode.ValidationFailed,
                        Message = $"[API] {api.Name}, {api.Path}/{api.Version}: {string.Join(", ", api.Errors)}",
                        IsError = true,
                    });
                }

                foreach (var rev in api.Revisions)
                {
                    if (rev.Errors.Any())
                    {
                        results.Add(new GatewayAutomationResult
                        {
                            ResultCode = ResultCode.ValidationFailed,
                            Message = $"[API Revision] {api.Name}, {api.Path}/{api.Version}: {string.Join(", ", rev.Errors)}",
                            IsError = true,
                        });
                    }
                }
            }

            foreach (var product in validatePublishingResult.Products)
            {
                if (product.Errors.Any())
                {
                    results.Add(new GatewayAutomationResult
                    {
                        ResultCode = ResultCode.ValidationFailed,
                        Message = $"[Product] {product.Name}: {string.Join(", ", product.Errors)}",
                        IsError = true,
                    });
                }
            }

            return results;
        }

        private static IEnumerable<GatewayAutomationResult> PreValidateEntities(string folderPath, PublishFileModel publishFileModel, HttpClient httpClient, GatewayOptions options, LogicTokenProviderFactory logicTokenProvider)
        {
            var preValidations = new List<IPreValidation>
            {
                new ProductsPreValidation(folderPath),
                new ApisPreValidation(folderPath, httpClient, logicTokenProvider,  options),
            };

            var publishResults = new List<GatewayAutomationResult>();
            foreach (var validation in preValidations)
            {
                var results = validation.ValidateAsync(publishFileModel);
                publishResults.AddRange(results);
            }

            return publishResults;
        }

        private static async Task<ValidatePublishingRequest> GetValidatePublishingRequest(string folderPath, Guid providerId, PublishFileModel input)
        {
            var apis = new List<ApiValidationModel>();
            foreach (var api in input.Apis)
            {
                foreach (var apiVersion in api.ApiVersions)
                {
                    var fs = new FileStream(Path.Combine(folderPath, apiVersion.OpenApiSpecFile), FileMode.Open, FileAccess.Read);
                    var revisions = apiVersion.Revisions?.Select(r =>
                    {
                        var fsRev = new FileStream(Path.Combine(folderPath, r.OpenApiSpecFile), FileMode.Open, FileAccess.Read);
                        return new ApiRevisionValidationModel(fsRev, r.RevisionDescription);
                    });

                    apis.Add(new ApiValidationModel(
                        api.Name,
                        api.Path,
                        apiVersion.VersionName,
                        fs,
                        apiVersion.ProductNames,
                        revisions,
                        await GetPoliciesValidationModel(folderPath, apiVersion.CustomPolicies, apiVersion.RateLimitPolicy).ConfigureAwait(false)));
                }
            }

            var products = await Task.WhenAll(input.Products.Select(async p => new ProductValidationModel(
                p.Name,
                await GetPoliciesValidationModel(folderPath, p.CustomPolicies, p.RateLimitPolicy).ConfigureAwait(false)))).ConfigureAwait(false);

            return new ValidatePublishingRequest(providerId, apis, products);
        }

        private static async Task<PoliciesValidationModel> GetPoliciesValidationModel(
            string folderPath,
            IEnumerable<CustomPolicy> customPolicies,
            RateLimitPolicy rateLimitPolicy)
        {
            var rateLimitPolicyToValidate = rateLimitPolicy == null
                ? null
                : new RateLimitPolicyValidationModel(rateLimitPolicy.Name, rateLimitPolicy.Calls, rateLimitPolicy.RenewalPeriod);

            var customPoliciesToValidate = customPolicies == null
                ? Enumerable.Empty<CustomPolicyValidationModel>()
                : await Task.WhenAll(customPolicies.Select(async cp =>
                {
                    using var xmlFs = new FileStream(Path.Combine(folderPath, cp.XmlFile), FileMode.Open, FileAccess.Read);
                    using var sr = new StreamReader(xmlFs);
                    return new CustomPolicyValidationModel(cp.Name, await sr.ReadToEndAsync().ConfigureAwait(false));
                })).ConfigureAwait(false);

            return new PoliciesValidationModel(customPoliciesToValidate, rateLimitPolicyToValidate);
        }
    }
}
