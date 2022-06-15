﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.Client;
using Kmd.Logic.Gateway.Automation.Client.Models;
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
            using var getStart = string.IsNullOrEmpty(product.GetStarted) ? null : new FileStream(path: Path.Combine(folderPath, product.GetStarted), FileMode.Open);
            var response = await client.CustomCreateProductAsync(
                subscriptionId: subscriptionId,
                name: product.Name,
                key: product.Key,
                description: product.Description,
                contactProvider: product.ContactProvider,
                providerId: providerId.ToString(),
                apiKeyRequired: product.ApiKeyRequired,
                providerApprovalRequired: product.ProviderApprovalRequired,
                productTerms: product.LegalTerms,
                visibility: product.Visibility,
                logo: logo,
                documentation: document,
                getStarted: getStart,
                clientCredentialRequired: product.ClientCredentialRequired,
                openidConfigIssuer: product.OpenidConfigIssuer,
                openidConfigCustomUrl: product.OpenidConfigCustomUrl,
                applicationId: product.ApplicationId).ConfigureAwait(false);

            var createdProduct = response as ProductListModel;

            if (createdProduct != null)
            {
                this._publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.ProductCreated, EntityId = createdProduct.Id, EntityName = createdProduct.Name });

                return createdProduct.Id;
            }
            else if (createdProduct == null)
            {
                this._publishResults.Add(new GatewayAutomationResult() { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = response.ToString() });
            }

            return null;
        }

        private async Task<Guid?> UpdateProduct(IGatewayClient client, Guid subscriptionId, Guid providerId, string folderPath, Product product, Guid productId)
        {
            using var logo = new FileStream(path: Path.Combine(folderPath, product.Logo), FileMode.Open);
            using var document = new FileStream(path: Path.Combine(folderPath, product.Documentation), FileMode.Open);
            using var getStart = string.IsNullOrEmpty(product.GetStarted) ? null : new FileStream(path: Path.Combine(folderPath, product.GetStarted), FileMode.Open);
            var response = await client.CustomUpdateProductAsync(
                subscriptionId: subscriptionId,
                productId: productId,
                name: product.Name,
                key: product.Key,
                description: product.Description,
                contactProvider: product.ContactProvider,
                providerId: providerId.ToString(),
                apiKeyRequired: product.ApiKeyRequired,
                providerApprovalRequired: product.ProviderApprovalRequired,
                productTerms: product.LegalTerms,
                visibility: product.Visibility,
                logo: logo,
                documentation: document,
                getStarted: getStart,
                clientCredentialRequired: product.ClientCredentialRequired,
                openidConfigIssuer: product.OpenidConfigIssuer,
                openidConfigCustomUrl: product.OpenidConfigCustomUrl,
                applicationId: product.ApplicationId).ConfigureAwait(false);

            var updatedProduct = response as ProductListModel;

            if (updatedProduct != null)
            {
                this._publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.ProductUpdated, EntityId = updatedProduct.Id, EntityName = updatedProduct.Name });
                return updatedProduct.Id;
            }
            else if (updatedProduct == null)
            {
                var errorMessage = $"Product update failed for ProductId: {productId} as {response}";
                this._publishResults.Add(new GatewayAutomationResult() { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = errorMessage });
            }

            return null;
        }
    }
}
