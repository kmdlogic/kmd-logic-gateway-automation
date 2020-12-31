using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.Client;
using Kmd.Logic.Gateway.Automation.PublishFile;
using Kmd.Logic.Identity.Authorization;

namespace Kmd.Logic.Gateway.Automation
{
    internal class PublishProducts
    {
        private readonly PublishPolicies _publishPolicies;
        private readonly GatewayOptions options;
        private readonly GatewayClientFactory gatewayClientFactory;
        private List<GatewayAutomationResult> _publishResults;

        internal PublishProducts(HttpClient httpClient, LogicTokenProviderFactory tokenProviderFactory, GatewayOptions options, List<GatewayAutomationResult> publishResults)
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

            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.gatewayClientFactory = new GatewayClientFactory(tokenProviderFactory, httpClient, options);
            this._publishResults = publishResults;
            this._publishPolicies = new PublishPolicies(httpClient, tokenProviderFactory, options, this._publishResults);
        }

        public async Task CreateOrUpdateProducts(Guid subscriptionId, Guid providerId, IEnumerable<Product> products, IEnumerable<ProductValidationResult> productValidationResults, string folderPath)
        {
            using var client = this.gatewayClientFactory.CreateClient();

            if (productValidationResults == null)
            {
                throw new ArgumentNullException(nameof(productValidationResults));
            }

            foreach (var productValidationResult in productValidationResults)
            {
                var product = products.Single(p => p.Name == productValidationResult.Name);
                var productId = productValidationResult.Status switch
                {
                    ValidationStatus.CanBeCreated => await this.CreateProduct(client, subscriptionId, providerId, folderPath, product).ConfigureAwait(false),
                    ValidationStatus.CanBeUpdated => await this.UpdateProduct(client, subscriptionId, providerId, folderPath, product, productValidationResult.EntityId.Value).ConfigureAwait(false),
                    _ => throw new NotSupportedException("Unsupported ValidationStatus in CreateOrUpdateProducts"),
                };

                await this._publishPolicies.CreateOrUpdatePolicies(
                    subscriptionId: subscriptionId,
                    entityId: productId,
                    folderPath: folderPath,
                    policiesResults: productValidationResult.Policies,
                    entityType: "Product",
                    rateLimitPolicy: product.RateLimitPolicy,
                    customPolicies: product.CustomPolicies).ConfigureAwait(false);
            }
        }

        private async Task<Guid?> CreateProduct(IGatewayClient client, Guid subscriptionId, Guid providerId, string folderPath, Product product)
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
                this._publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.ProductCreated, EntityId = response.Id });
                return response.Id;
            }

            return null;
        }

        private async Task<Guid?> UpdateProduct(IGatewayClient client, Guid subscriptionId, Guid providerId, string folderPath, Product product, Guid productId)
        {
            using var logo = new FileStream(path: Path.Combine(folderPath, product.Logo), FileMode.Open);
            using var document = new FileStream(path: Path.Combine(folderPath, product.Documentation), FileMode.Open);
            var response = await client.UpdateProductAsync(
                subscriptionId: subscriptionId,
                productId: productId,
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
                this._publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.ProductUpdated, EntityId = response.Id });
                return response.Id;
            }

            return null;
        }
    }
}
