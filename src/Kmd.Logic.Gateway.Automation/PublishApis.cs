using System;
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
    internal class PublishApis
    {
        private readonly PublishPolicies _publishPolicies;
        private readonly GatewayOptions options;
        private readonly GatewayClientFactory gatewayClientFactory;
        private List<GatewayAutomationResult> _publishResults;

        internal PublishApis(HttpClient httpClient, LogicTokenProviderFactory tokenProviderFactory, GatewayOptions options, List<GatewayAutomationResult> publishResults)
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
            this._publishResults = publishResults;
            this._publishPolicies = new PublishPolicies(httpClient, tokenProviderFactory, options, this._publishResults);
        }

        internal async Task CreateOrUpdateApis(Guid subscriptionId, Guid providerId, IEnumerable<Api> apis, IEnumerable<ApiValidationResult> apiValidationResults, string folderPath)
        {
            if (!apis.Any())
            {
                return;
            }

            using var client = this.gatewayClientFactory.CreateClient();
            var allProducts = await client.GetAllProductsAsync(subscriptionId, providerId).ConfigureAwait(false);
            var existingApis = await client.GetAllApisAsync(subscriptionId, providerId).ConfigureAwait(false);

            if (apis == null)
            {
                throw new ArgumentNullException(nameof(apis));
            }

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

                    await this._publishPolicies.CreateOrUpdatePolicies(
                       subscriptionId: subscriptionId,
                       entityId: apiId,
                       folderPath: folderPath,
                       policiesResults: apiVersionValidationResult.Policies,
                       entityType: "Api",
                       rateLimitPolicy: apiVersion.RateLimitPolicy,
                       customPolicies: apiVersion.CustomPolicies)
                    .ConfigureAwait(false);
                }
            }
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
                this._publishResults.Add(new GatewayAutomationResult() { ResultCode = apiVersionSetId.HasValue ? ResultCode.VersionCreated : ResultCode.ApiCreated, EntityId = createdApi.Id });

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
                this._publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.RevisionCreated, EntityId = createdRevision.Id });
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
                this._publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.ApiUpdated, EntityId = updatedApi.Id });

                await this.IfSoThenMakeVersionCurrent(client, subscriptionId, updatedApi.Id.Value, apiVersion.IsCurrent.Value).ConfigureAwait(false);

                await this.CreateOrUpdateRevisions(client, subscriptionId, folderPath, apiVersion, apiVersionValidationResult).ConfigureAwait(false);

                return updatedApi.Id;
            }

            return null;
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
                this._publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.RevisionUpdated, EntityId = updatedRevision.Id });
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
                    this._publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.ApiVersionMarkedAsCurrent, EntityId = currentApiVersion.Id });
                }
            }
        }
    }
}
