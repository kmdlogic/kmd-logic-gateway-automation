﻿using System;
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
                var productId = productValidationResult.Status switch
                {
                    ValidationStatus.CanBeCreated => await this.CreateProduct(client, subscriptionId, providerId, folderPath, product).ConfigureAwait(false),
                    ValidationStatus.CanBeUpdated => await this.UpdateProduct(client, subscriptionId, providerId, folderPath, product, productValidationResult.EntityId.Value).ConfigureAwait(false),
                    _ => throw new NotSupportedException("Unsupported ValidationStatus in CreateOrUpdateProducts"),
                };

                await this.CreateOrUpdatePolicies(
                    client: client,
                    subscriptionId: subscriptionId,
                    entityId: productId,
                    folderPath: folderPath,
                    policiesResults: productValidationResult.Policies,
                    entityType: "Product",
                    rateLimitPolicy: product.RateLimitPolicy,
                    customPolicies: product.CustomPolicies).ConfigureAwait(false);
            }
        }

        private async Task CreateOrUpdatePolicies(
            IGatewayClient client,
            Guid subscriptionId,
            Guid? entityId,
            string folderPath,
            PoliciesValidationResult policiesResults,
            string entityType,
            RateLimitPolicy rateLimitPolicy,
            IEnumerable<CustomPolicy> customPolicies)
        {
            if (entityId.HasValue)
            {
                if (rateLimitPolicy != null)
                {
                    var rateLimitPolicyRequest = new RateLimitPolicyRequest(
                        rateLimitPolicy.Name, entityId, entityType, rateLimitPolicy.Description, rateLimitPolicy.Calls, rateLimitPolicy.RenewalPeriod);
                    switch (policiesResults.RateLimitPolicy.Status)
                    {
                        case ValidationStatus.CanBeCreated:
                            var created = await client.CreateRateLimitPolicyAsync(subscriptionId, rateLimitPolicyRequest).ConfigureAwait(false);
                            this.publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.RateLimitPolicyCreated, EntityId = created.Id });
                            break;
                        case ValidationStatus.CanBeUpdated:
                            // TODO: Update RateLimitPolicy
                            break;
                        default:
                            throw new NotSupportedException("Unsupported RateLimitPolicy ValidationStatus in CreateOrUpdatePolicies");
                    }
                }

                if (customPolicies != null)
                {
                    foreach (var customPolicy in customPolicies)
                    {
                        var customPolicyResult = policiesResults.CustomPolicies.Single(cp => string.Equals(cp.Name, customPolicy.Name, StringComparison.OrdinalIgnoreCase));

                        using var xmlFs = new FileStream(path: Path.Combine(folderPath, customPolicy.XmlFile), FileMode.Open, FileAccess.Read);
                        using var sr = new StreamReader(xmlFs);
                        var xmlContent = await sr.ReadToEndAsync().ConfigureAwait(false);

                        var customPolicyRequest = new CustomPolicyRequest(customPolicy.Name, xmlContent, entityId, customPolicy.Description, entityType);
                        switch (customPolicyResult.Status)
                        {
                            case ValidationStatus.CanBeCreated:
                                var created = await client.CreateCustomPolicyAsync(subscriptionId, customPolicyRequest).ConfigureAwait(false);
                                this.publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.CustomPolicyCreated, EntityId = created.Id });
                                break;
                            case ValidationStatus.CanBeUpdated:
                                // TODO: Update CustomPolicy
                                break;
                            default:
                                throw new NotSupportedException("Unsupported CustomPolicy ValidationStatus in CreateOrUpdatePolicies");
                        }
                    }
                }
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
                this.publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.ProductCreated, EntityId = response.Id });
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
                this.publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.ProductUpdated, EntityId = response.Id });
                return response.Id;
            }

            return null;
        }

        private async Task CreateOrUpdateApis(Guid subscriptionId, Guid providerId, IEnumerable<Api> apis, IEnumerable<ApiValidationResult> apiValidationResults, string folderPath)
        {
            if (!apis.Any())
            {
                return;
            }

            using var client = this.gatewayClientFactory.CreateClient();

            var allProducts = await client.GetAllProductsAsync(subscriptionId, providerId).ConfigureAwait(false);
            var existingApis = await client.GetAllApisAsync(subscriptionId, providerId).ConfigureAwait(false);

            foreach (var api in apis)
            {
                foreach (var apiVersion in api.ApiVersions)
                {
                    var apiVersionValidationResult = apiValidationResults.SingleOrDefault(result => result.Name == api.Name && result.Version == apiVersion.VersionName);

                    var apiId = apiVersionValidationResult.Status switch
                    {
                        ValidationStatus.CanBeCreated => await this.CreateApi(client, subscriptionId, providerId, folderPath, allProducts, existingApis, api, apiVersion).ConfigureAwait(false),
                        ValidationStatus.CanBeUpdated => await this.UpdateApi(client, subscriptionId, apiVersionValidationResult, folderPath, allProducts, apiVersion).ConfigureAwait(false),
                        _ => throw new NotSupportedException("Unsupported ValidationStatus in CreateOrUpdateApis"),
                    };

                    await this.CreateOrUpdatePolicies(
                        client: client,
                        subscriptionId: subscriptionId,
                        entityId: apiId,
                        folderPath: folderPath,
                        policiesResults: apiVersionValidationResult.Policies,
                        entityType: "Api",
                        rateLimitPolicy: apiVersion.RateLimitPolicy,
                        customPolicies: apiVersion.CustomPolicies).ConfigureAwait(false);
                }
            }
        }

        private async Task<Guid?> UpdateApi(IGatewayClient client, Guid subscriptionId, ApiValidationResult apiVersionValidationResult, string folderPath, IList<GetProductListModel> allProducts, ApiVersion apiVersion)
        {
            var productIds = apiVersion.ProductNames.Select(n => allProducts.SingleOrDefault(p => string.Compare(p.Name, n, comparisonType: StringComparison.OrdinalIgnoreCase) == 0)?.Id)?.ToList();
            using var logo = new FileStream(path: Path.Combine(folderPath, apiVersion.ApiLogoFile), FileMode.Open, FileAccess.Read);
            using var document = new FileStream(path: Path.Combine(folderPath, apiVersion.ApiDocumentation), FileMode.Open, FileAccess.Read);

            var response = await client.CustomUpdateApiAsync(
                subscriptionId: subscriptionId,
                apiId: apiVersionValidationResult.EntityId.Value,
                name: apiVersionValidationResult.Name,
                apiVersion: apiVersion.VersionName,
                visibility: apiVersion.Visibility,
                backendServiceUrl: apiVersion.BackendLocation,
                productIds: productIds?.Where(x => x.HasValue)?.ToList(),
                logo: logo,
                documentation: document,
                status: apiVersion.Status.HasValue ? apiVersion.Status.Value.ToString() : default).ConfigureAwait(false);

            var updatedApi = response as ApiListModel;

            if (updatedApi != null)
            {
                this.publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.ApiUpdated, EntityId = updatedApi.Id });

                await this.IfSoThenMakeVersionCurrent(client, subscriptionId, updatedApi.Id.Value, apiVersion.IsCurrent.Value).ConfigureAwait(false);

                await this.CreateOrUpdateRevisions(client, subscriptionId, folderPath, apiVersion, apiVersionValidationResult).ConfigureAwait(false);

                return updatedApi.Id;
            }

            return null;
        }

        private async Task<Guid?> CreateApi(IGatewayClient client, Guid subscriptionId, Guid providerId, string folderPath, IList<GetProductListModel> allProducts, IList<ApiListModel> existingApis, Api api, ApiVersion apiVersion)
        {
            Guid? apiVersionSetId = existingApis.FirstOrDefault(a => a.Name == api.Name)?.ApiVersionSetId;

            var productIds = apiVersion.ProductNames.Select(n => allProducts.SingleOrDefault(p => string.Compare(p.Name, n, comparisonType: StringComparison.OrdinalIgnoreCase) == 0)?.Id)?.ToList();
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
                productIds: productIds?.Where(x => x.HasValue)?.ToList(),
                logo: logo,
                documentation: document,
                status: apiVersion.Status.HasValue ? apiVersion.Status.Value.ToString() : default,
                isCurrent: apiVersion.IsCurrent.Value).ConfigureAwait(false);

            var createdApi = response as ApiListModel;

            if (createdApi != null)
            {
                existingApis.Add(createdApi);
                this.publishResults.Add(new GatewayAutomationResult() { ResultCode = apiVersionSetId.HasValue ? ResultCode.VersionCreated : ResultCode.ApiCreated, EntityId = createdApi.Id });

                await this.CreateRevisions(client, subscriptionId, createdApi.Id.Value, folderPath, apiVersion.Revisions).ConfigureAwait(false);

                return createdApi.Id;
            }

            return null;
        }

        private async Task CreateRevisions(IGatewayClient client, Guid subscriptionId, Guid apiVersionId, string folderPath, IEnumerable<Revision> revisions)
        {
            if (revisions == null)
            {
                return;
            }

            await Task.WhenAll(revisions.Select(r => this.CreateRevision(client, subscriptionId, apiVersionId, folderPath, r))).ConfigureAwait(false);
        }

        private Task CreateOrUpdateRevisions(IGatewayClient client, Guid subscriptionId, string folderPath, ApiVersion apiVersion, ApiValidationResult apiVersionValidationResult)
        {
            if (apiVersionValidationResult.Revisions == null || !apiVersionValidationResult.Revisions.Any())
            {
                return Task.CompletedTask;
            }

            var validationResults = apiVersionValidationResult.Revisions.ToArray();
            var revisions = apiVersion.Revisions.ToArray();

            // We rely here on high probability of the same order of input revisions and revisions validations output.
            // We should make it more reliable, when we find out how to do that.
            var tasks = new Task[validationResults.Length];
            for (int i = 0; i < validationResults.Length; i++)
            {
                var revision = revisions[i];
                var revisionId = validationResults[i].EntityId;

                tasks[i] = validationResults[i].Status == ValidationStatus.CanBeCreated
                    ? Task.Run(() => this.CreateRevision(client, subscriptionId, apiVersionValidationResult.EntityId.Value, folderPath, revision))
                    : Task.Run(() => this.UpdateRevision(client, subscriptionId, apiVersionValidationResult.EntityId.Value, revisionId.Value, folderPath, revision));
            }

            return Task.WhenAll(tasks);
        }

        private async Task CreateRevision(IGatewayClient client, Guid subscriptionId, Guid apiVersionId, string folderPath, Revision revision)
        {
            using var revisionOpenApiSpec = new FileStream(path: Path.Combine(folderPath, revision.OpenApiSpecFile), FileMode.Open, FileAccess.Read);
            var revisionResponse = await client.CreateRevisionAsync(
                subscriptionId: subscriptionId,
                apiId: apiVersionId,
                openApiSpec: revisionOpenApiSpec,
                revisionDescription: revision.RevisionDescription).ConfigureAwait(false);

            var createdRevision = revisionResponse as RevisionResponseModel;
            if (createdRevision != null)
            {
                this.publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.RevisionCreated, EntityId = createdRevision.Id });
            }
        }

        private async Task UpdateRevision(IGatewayClient client, Guid subscriptionId, Guid apiVersionId, Guid revisionId, string folderPath, Revision revision)
        {
            using var revisionOpenApiSpec = new FileStream(path: Path.Combine(folderPath, revision.OpenApiSpecFile), FileMode.Open, FileAccess.Read);
            var revisionResponse = await client.UpdateRevisionAsync(
                subscriptionId: subscriptionId,
                apiId: apiVersionId,
                apiRevisionId: revisionId,
                request: new RevisionUpdateRequestModel(revision.RevisionDescription, revision.IsCurrent)).ConfigureAwait(false);

            var updatedRevision = revisionResponse as RevisionResponseModel;
            if (updatedRevision != null)
            {
                this.publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.RevisionUpdated, EntityId = updatedRevision.Id });
            }
        }

        private async Task IfSoThenMakeVersionCurrent(IGatewayClient client, Guid subscriptionId, Guid apiVersionId, bool isCurrent)
        {
            if (isCurrent)
            {
                var response = await client.MakeVersionIsCurrentAsync(subscriptionId, apiVersionId, isCurrent).ConfigureAwait(false);

                var currentApiVersion = response as ApiListModel;
                if (currentApiVersion != null)
                {
                    this.publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.ApiVersionMarkedAsCurrent, EntityId = currentApiVersion.Id });
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