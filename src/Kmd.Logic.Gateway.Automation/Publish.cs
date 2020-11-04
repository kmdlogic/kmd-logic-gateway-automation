using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.Gateway;
using Kmd.Logic.Identity.Authorization;
using YamlDotNet.Serialization;

namespace Kmd.Logic.Gateway.Automation
{
    [SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "HttpClient is not owned by this class.")]
    public class Publish : IPublish
    {
        private readonly GatewayClientFactory gatewayClientFactory;
        private readonly GatewayOptions options;
        private readonly ValidatePublishing validatePublishing;
        private IList<PublishResult> publishResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="Publish"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client to use. The caller is expected to manage this resource and it will not be disposed.</param>
        /// <param name="tokenProviderFactory">The Logic access token provider factory.</param>
        /// <param name="options">The required configuration options.</param>
        public Publish(HttpClient httpClient, LogicTokenProviderFactory tokenProviderFactory, GatewayOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            if (tokenProviderFactory == null)
            {
                throw new ArgumentNullException(nameof(tokenProviderFactory));
            }

            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            this.gatewayClientFactory = new GatewayClientFactory(tokenProviderFactory, httpClient, options);

            this.publishResults = new List<PublishResult>();

            this.validatePublishing = new ValidatePublishing(httpClient, tokenProviderFactory, options);
        }

        /// <summary>
        /// Create gateway entities.
        /// </summary>
        /// <param name="folderPath">Folder path provider all gateway entries details.</param>
        /// <returns>Error details on failure, gateway entities name on success.</returns>
        public async Task<IEnumerable<PublishResult>> ProcessAsync(string folderPath)
        {
            this.publishResults.Clear();

            if (!this.IsValidInput(folderPath))
            {
                return this.publishResults;
            }

            var validationResult = await this.validatePublishing.Validate(folderPath).ConfigureAwait(false);

            if (validationResult.IsSuccess)
            {
                this.publishResults.Add(new PublishResult
                {
                    IsError = false,
                    ResultCode = ResultCode.PublishingValidationSuccess,
                    Message = validationResult.ToString(),
                });

                using var publishYml = File.OpenText(Path.Combine(folderPath, @"publish.yml"));
                var yaml = new Deserializer().Deserialize<GatewayDetails>(publishYml);
                using var client = this.gatewayClientFactory.CreateClient();
                await this.CreateProductsAsync(client, this.options.SubscriptionId, this.options.ProviderId, yaml.Products, folderPath).ConfigureAwait(false);
            }
            else
            {
                this.publishResults.Add(new PublishResult
                {
                    IsError = true,
                    ResultCode = ResultCode.PublishingValidationFailed,
                    Message = validationResult.ToString(),
                });
            }

            return this.publishResults;
        }

        private async Task CreateProductsAsync(IGatewayClient client, Guid subscriptionId, Guid providerId, IEnumerable<Product> products, string folderPath)
        {
            foreach (var product in products)
            {
                using var logo = new FileStream(path: Path.Combine(folderPath, product.Logo), FileMode.Open);
                using var document = new FileStream(path: Path.Combine(folderPath, product.Documentation), FileMode.Open);
                var response = await client.CreateProductAsync(
                    subscriptionId: subscriptionId,
                    name: product.Name,
                    description: product.Description,
                    providerId: providerId.ToString(),
                    apiKeyRequired: product.ApiKeyRequired,
                    providerApprovalRequired: product.ProviderApprovalRequired,
                    productTerms: product.LegalTerms,
                    visibility: product.Visibility,
                    logo: logo,
                    documentation: document,
                    clientCredentialRequired: product.ClientCredentialRequired,
                    openidConfigIssuer: product.OpenidConfigIssuer,
                    openidConfigCustomUrl: product.OpenidConfigCustomUrl,
                    applicationId: product.ApplicationId).ConfigureAwait(false);

                if (response != null)
                {
                    this.publishResults.Add(new PublishResult() { ResultCode = ResultCode.ProductCreated, EntityId = response.Id });
                }
            }
        }

        private bool IsValidInput(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                this.publishResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = "Specified folder doesn’t exist" });
                return false;
            }

            if (!File.Exists(Path.Combine(folderPath, @"publish.yml")))
            {
                this.publishResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = "Publish yml not found" });
                return false;
            }

            return true;
        }
    }
}