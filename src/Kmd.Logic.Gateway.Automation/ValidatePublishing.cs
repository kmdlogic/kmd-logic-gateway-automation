using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.Client;
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

        public async Task<ValidatePublishingResult> ValidateAsync(string folderPath)
        {
            var publishYml = File.ReadAllText(Path.Combine(folderPath, "publish.yml"));
            var yaml = new Deserializer().Deserialize<PublishFileModel>(publishYml);
            using var client = this.gatewayClientFactory.CreateClient();
            return await client.ValidatePublishingAsync(
                this.options.SubscriptionId,
                GetValidatePublishingRequest(folderPath, this.options.ProviderId, yaml)).ConfigureAwait(false);
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
