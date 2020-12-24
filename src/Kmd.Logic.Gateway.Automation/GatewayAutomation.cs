using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.Client;
using Kmd.Logic.Identity.Authorization;

namespace Kmd.Logic.Gateway.Automation
{
    public class GatewayAutomation : IGatewayAutomation
    {
        private readonly ValidatePublishing validatePublishing;
        private readonly Publish publish;

        public GatewayAutomation(HttpClient httpClient, LogicTokenProviderFactory tokenProviderFactory, GatewayOptions options)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            if (tokenProviderFactory == null)
            {
                throw new ArgumentNullException(nameof(tokenProviderFactory));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (!options.ProviderId.HasValue)
            {
                using var client = new GatewayClientFactory(tokenProviderFactory, httpClient, options).CreateClient();
                var provider = client.GetGatewayProviders(options.SubscriptionId.ToString())?.SingleOrDefault(x => x.SubscriptionId == options.SubscriptionId);
                if (provider == null)
                {
                    throw new ArgumentException("providerId not found");
                }

                options.ProviderId = provider.Id;
            }

            this.validatePublishing = new ValidatePublishing(httpClient, tokenProviderFactory, options);
            this.publish = new Publish(httpClient, tokenProviderFactory, options);
        }

        public Task<IEnumerable<GatewayAutomationResult>> PublishAsync(string folderPath)
        {
            return this.publish.PublishAsync(folderPath);
        }

        public Task<ValidationResult> ValidateAsync(string folderPath)
        {
            return this.validatePublishing.ValidateAsync(folderPath);
        }
    }
}
