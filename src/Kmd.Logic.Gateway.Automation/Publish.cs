using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.Client;
using Kmd.Logic.Gateway.Automation.Client.Models;
using Kmd.Logic.Gateway.Automation.PublishFile;
using Kmd.Logic.Identity.Authorization;
using YamlDotNet.Serialization;

namespace Kmd.Logic.Gateway.Automation
{
    internal class Publish
    {
        private readonly GatewayClientFactory gatewayClientFactory;
        private readonly GatewayOptions options;
        private readonly ValidatePublishing validatePublishing;
        private List<GatewayAutomationResult> publishResults;

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
            this.validatePublishing = new ValidatePublishing(httpClient, tokenProviderFactory, options);

            this.publishResults = new List<GatewayAutomationResult>();
        }

        /// <summary>
        /// Create gateway entities.
        /// </summary>
        /// <param name="folderPath">Folder path provider all gateway entries details.</param>
        /// <returns>Error details on failure, gateway entities name on success.</returns>
        public async Task<IEnumerable<GatewayAutomationResult>> PublishAsync(string folderPath)
        {
            this.publishResults.Clear();

            if (!this.IsValidInput(folderPath))
            {
                return this.publishResults;
            }

            PublishFileModel publishFileModel;
            using var publishYml = File.OpenText(Path.Combine(folderPath, @"publish.yml"));
            try
            {
                publishFileModel = new Deserializer().Deserialize<PublishFileModel>(publishYml);
            }
            catch (Exception e) when (e is YamlDotNet.Core.SemanticErrorException || e is YamlDotNet.Core.SyntaxErrorException)
            {
                this.publishResults.Add(new GatewayAutomationResult() { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = "Invalid yaml file, check is publish yaml file is having format and syntax errors" });
                return this.publishResults;
            }

            var validationResult = await this.validatePublishing.ValidateAsync(folderPath).ConfigureAwait(false);
            if (validationResult.IsError)
            {
                this.publishResults.AddRange(validationResult.Errors);
                return this.publishResults;
            }
            else
            {
                await this.CreateOrUpdateProducts(
                    subscriptionId: this.options.SubscriptionId,
                    providerId: this.options.ProviderId,
                    products: publishFileModel.Products,
                    productValidationResults: validationResult.ValidatePublishingResult.Products,
                    folderPath: folderPath).ConfigureAwait(false);

                await this.CreateOrUpdateApis(
                    subscriptionId: this.options.SubscriptionId,
                    providerId: this.options.ProviderId,
                    apis: publishFileModel.Apis,
                    apiValidationResults: validationResult.ValidatePublishingResult.Apis,
                    folderPath: folderPath).ConfigureAwait(false);
            }

            return this.publishResults;
        }

        private async Task CreateOrUpdateProducts(Guid subscriptionId, Guid providerId, IEnumerable<Product> products, IEnumerable<ProductValidationResult> productValidationResults, string folderPath)
        {
            using var client = this.gatewayClientFactory.CreateClient();
            foreach (var productValidationResult in productValidationResults)
            {
                var product = products.Single(p => p.Name == productValidationResult.Name);
                switch (productValidationResult.Status)
                {
                    case ValidationStatus.CanBeCreated:
                        await this.CreateProduct(client, subscriptionId, providerId, folderPath, product).ConfigureAwait(false);
                        break;
                    case ValidationStatus.CanBeUpdated:
                        var productId = productValidationResult.ProductId.Value;
                        await this.UpdateProduct(client, subscriptionId, providerId, folderPath, product, productId).ConfigureAwait(false);
                        break;
                    default:
                        throw new NotSupportedException("Unsupported ValidationStatus in CreateOrUpdateProducts");
                }
            }
        }

        private async Task CreateProduct(IGatewayClient client, Guid subscriptionId, Guid providerId, string folderPath, Product product)
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
                this.publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.ProductCreated, EntityId = response.Id });
            }
        }

        private async Task UpdateProduct(IGatewayClient client, Guid subscriptionId, Guid providerId, string folderPath, Product product, Guid productId)
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
                this.publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.ProductUpdated, EntityId = response.Id });
            }
        }

        private async Task CreateOrUpdateApis(Guid subscriptionId, Guid providerId, IEnumerable<Api> apis, IEnumerable<ApiValidationResult> apiValidationResults, string folderPath)
        {
            if (!apis.Any())
            {
                return;
            }

            using var client = this.gatewayClientFactory.CreateClient();

            var allProducts = await client.GetAllProductsAsync(subscriptionId, providerId).ConfigureAwait(false);

            foreach (var apiValidationResult in apiValidationResults)
            {
                var api = apis.Single(api => api.Name == apiValidationResult.Name);
                switch (apiValidationResult.Status)
                {
                    case ValidationStatus.CanBeCreated:
                        await this.CreateApi(client, subscriptionId, providerId, folderPath, allProducts, api).ConfigureAwait(false);
                        break;
                    case ValidationStatus.CanBeUpdated:
                        var apiId = apiValidationResult.ApiId.Value;

                        // TO DO:Add the necessary code to update
                        break;
                    default:
                        throw new NotSupportedException("Unsupported ValidationStatus in CreateOrUpdateApis");
                }
            }
        }

        private async Task CreateApi(IGatewayClient client, Guid subscriptionId, Guid providerId, string folderPath, IList<GetProductListModel> allProducts, Api api)
        {
            Guid? apiVersionSetId = default;

            foreach (var apiVersion in api.ApiVersions)
            {
                var productIds = apiVersion.ProductNames.Select(productName => allProducts.SingleOrDefault(product => string.Compare(product.Name, productName, comparisonType: StringComparison.OrdinalIgnoreCase) == 0).Id).ToList();
                using var logo = new FileStream(path: Path.Combine(folderPath, apiVersion.ApiLogoFile), FileMode.Open);
                using var document = new FileStream(path: Path.Combine(folderPath, apiVersion.ApiDocumentation), FileMode.Open);
                using var openApiSpec = new FileStream(path: Path.Combine(folderPath, apiVersion.OpenApiSpecFile), FileMode.Open);

                var response = await client.CustomCreateApiAsync(
                subscriptionId: subscriptionId,
                name: api.Name,
                path: api.Path,
                apiVersion: apiVersion.VersionName,
                openApiSpec: openApiSpec,
                apiVersionSetId: apiVersionSetId,
                providerId: providerId.ToString(),
                visibility: apiVersion.Visibility,
                backendServiceUrl: apiVersion.BackendLocation,
                productIds: null,
                logo: logo,
                documentation: document).ConfigureAwait(false);

                var createdApi = response as ApiListModel;

                if (createdApi != null)
                {
                    this.publishResults.Add(new GatewayAutomationResult() { ResultCode = apiVersionSetId.HasValue ? ResultCode.VersionCreated : ResultCode.ApiCreated, EntityId = createdApi.Id });
                    apiVersionSetId = createdApi.ApiVersionSetId;
                }
            }
        }

        private bool IsValidInput(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                this.publishResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = "Specified folder doesn’t exist" });
                return false;
            }

            if (!File.Exists(Path.Combine(folderPath, @"publish.yml")))
            {
                this.publishResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = "Publish yml not found" });
                return false;
            }

            return true;
        }
    }
}