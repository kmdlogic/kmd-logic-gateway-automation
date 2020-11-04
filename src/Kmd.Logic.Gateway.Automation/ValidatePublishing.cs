using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.Gateway;
using Kmd.Logic.Gateway.Automation.Models;
using Kmd.Logic.Identity.Authorization;
using YamlDotNet.Serialization;

namespace Kmd.Logic.Gateway.Automation
{
    public class ValidatePublishing
    {
        private readonly GatewayClientFactory gatewayClientFactory;
        private readonly GatewayOptions gatewayOptions;

        public ValidatePublishing(HttpClient httpClient, LogicTokenProviderFactory tokenProviderFactory, GatewayOptions gatewayOptions)
        {
            this.gatewayOptions = gatewayOptions;
            this.gatewayClientFactory = new GatewayClientFactory(tokenProviderFactory, httpClient, gatewayOptions);
        }

        public async Task<ValidatePublishingResponse> Validate(string folderPath)
        {
            var publishYml = await File.ReadAllTextAsync(Path.Combine(folderPath, "publish.yml")).ConfigureAwait(false);
            var yaml = new Deserializer().Deserialize<GatewayDetails>(publishYml);
            using var client = this.gatewayClientFactory.CreateClient();
            return await client.ValidatePublishingAsync(
                this.gatewayOptions.SubscriptionId,
                GetValidatePublishingRequest(folderPath, this.gatewayOptions.ProviderId, yaml)).ConfigureAwait(false);
        }

        private static ValidatePublishingRequest GetValidatePublishingRequest(string folderPath, Guid providerId, GatewayDetails input)
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
                        apiVersion.PathIdentifier,
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
