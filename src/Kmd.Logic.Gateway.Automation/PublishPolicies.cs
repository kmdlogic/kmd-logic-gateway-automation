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
    internal class PublishPolicies
    {
        private readonly GatewayOptions options;
        private readonly GatewayClientFactory gatewayClientFactory;
        private List<GatewayAutomationResult> publishResults;

        public PublishPolicies(HttpClient httpClient, LogicTokenProviderFactory tokenProviderFactory, GatewayOptions options, List<GatewayAutomationResult> publishResults)
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
            this.publishResults = publishResults;
        }

        public async Task CreateOrUpdatePolicies(
          Guid subscriptionId,
          Guid? entityId,
          string folderPath,
          PoliciesValidationResult policiesResults,
          string entityType,
          RateLimitPolicy rateLimitPolicy,
          IEnumerable<CustomPolicy> customPolicies)
        {
            using var client = this.gatewayClientFactory.CreateClient();
            if (entityId.HasValue)
            {
                if (rateLimitPolicy != null)
                {
                    var rateLimitPolicyRequest = new RateLimitPolicyRequest(
                        rateLimitPolicy.Name, entityId, entityType, rateLimitPolicy.Description, rateLimitPolicy.Calls, rateLimitPolicy.RenewalPeriod);
                    if (policiesResults == null)
                    {
                        throw new ArgumentNullException(nameof(policiesResults));
                    }

                    switch (policiesResults.RateLimitPolicy.Status)
                    {
                        case ValidationStatus.CanBeCreated:
                            var created = await client.CreateRateLimitPolicyAsync(subscriptionId, rateLimitPolicyRequest).ConfigureAwait(false);
                            this.publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.RateLimitPolicyCreated, EntityId = created.Id });
                            break;
                        case ValidationStatus.CanBeUpdated:
                            var updated = await client.UpdateRateLimitPolicyAsync(subscriptionId, policiesResults.RateLimitPolicy.EntityId.Value, rateLimitPolicyRequest).ConfigureAwait(false);
                            this.publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.RateLimitPolicyUpdated, EntityId = updated.Id });
                            break;
                        default:
                            throw new NotSupportedException("Unsupported RateLimitPolicy ValidationStatus in CreateOrUpdatePolicies");
                    }
                }

                if (customPolicies != null)
                {
                    foreach (var customPolicy in customPolicies)
                    {
                        if (policiesResults == null)
                        {
                            throw new ArgumentNullException(nameof(policiesResults));
                        }

                        var customPolicyResult = policiesResults.CustomPolicies.Single(cp => string.Equals(cp.Name, customPolicy.Name, StringComparison.OrdinalIgnoreCase));

                        using var sr = new StreamReader(Path.Combine(folderPath, customPolicy.XmlFile));
                        var xmlContent = await sr.ReadToEndAsync().ConfigureAwait(false);

                        var customPolicyRequest = new CustomPolicyRequest(customPolicy.Name, xmlContent, entityId, customPolicy.Description, entityType);
                        switch (customPolicyResult.Status)
                        {
                            case ValidationStatus.CanBeCreated:
                                var created = await client.CreateCustomPolicyAsync(subscriptionId, customPolicyRequest).ConfigureAwait(false);
                                this.publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.CustomPolicyCreated, EntityId = created.Id });
                                break;
                            case ValidationStatus.CanBeUpdated:
                                var updated = await client.UpdateCustomPolicyAsync(subscriptionId, customPolicyResult.EntityId.Value, customPolicyRequest).ConfigureAwait(false);
                                this.publishResults.Add(new GatewayAutomationResult() { ResultCode = ResultCode.CustomPolicyUpdated, EntityId = updated.Id });
                                break;
                            default:
                                throw new NotSupportedException("Unsupported CustomPolicy ValidationStatus in CreateOrUpdatePolicies");
                        }
                    }
                }
            }
        }
    }
}
