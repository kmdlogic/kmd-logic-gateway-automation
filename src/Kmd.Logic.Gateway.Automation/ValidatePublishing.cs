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

namespace Kmd.Logic.Gateway.Automation
{
    internal class ValidatePublishing
    {
        private readonly GatewayClientFactory gatewayClientFactory;
        private readonly GatewayOptions options;

        public ValidatePublishing(HttpClient httpClient, LogicTokenProviderFactory tokenProviderFactory, GatewayOptions options)
        {
            this.options = options;
            this.gatewayClientFactory = new GatewayClientFactory(tokenProviderFactory, httpClient, options);
        }

        public async Task<ValidationResult> ValidateAsync(string folderPath)
        {
            var publishYml = File.ReadAllText(Path.Combine(folderPath, "publish.yml"));
            var publishFileModel = new Deserializer().Deserialize<PublishFileModel>(publishYml);

            var preValidationsResult = PreValidateEntities(folderPath, publishFileModel);
            if (preValidationsResult.IsError)
            {
                return preValidationsResult;
            }

            using var client = this.gatewayClientFactory.CreateClient();
            var validationResult = await client.ValidatePublishingAsync(
                this.options.SubscriptionId,
                GetValidatePublishingRequest(folderPath, this.options.ProviderId, publishFileModel)).ConfigureAwait(false);

            return GetResult(validationResult);
        }

        private static ValidationResult GetResult(ValidatePublishingResult validationResult)
        {
            var result = new[]
            {
                new PublishResult
                {
                    IsError = !validationResult.IsSuccess,
                    ResultCode = validationResult.IsSuccess ? ResultCode.PublishingValidationSuccess : ResultCode.PublishingValidationFailed,
                    Message = validationResult.ToString(),
                },
            };

            return new ValidationResult(result);
        }

        private static ValidationResult PreValidateEntities(string folderPath, PublishFileModel publishFileModel)
        {
            var preValidations = new List<IPreValidation>
            {
                new ProductsPreValidation(folderPath),
                new ApisPreValidation(folderPath),
            };

            var publishResults = new List<PublishResult>();
            foreach (var validation in preValidations)
            {
                var result = validation.ValidateAsync(publishFileModel);
                publishResults.AddRange(result.ValidationResults);
            }

            return new ValidationResult(publishResults);
        }

        private static ValidatePublishingRequest GetValidatePublishingRequest(string folderPath, Guid providerId, PublishFileModel input)
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
                        revisions));
                }
            }

            var products = input.Products.Select(p => new ProductValidationModel(p.Name));

            return new ValidatePublishingRequest(providerId, apis, products);
        }
    }
}
