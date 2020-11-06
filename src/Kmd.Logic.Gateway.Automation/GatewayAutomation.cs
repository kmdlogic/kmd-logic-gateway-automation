using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
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

            this.validatePublishing = new ValidatePublishing(httpClient, tokenProviderFactory, options);
            this.publish = new Publish(httpClient, tokenProviderFactory, options);
        }

        public Task<IEnumerable<PublishResult>> PublishAsync(string folderPath)
        {
            return this.publish.PublishAsync(folderPath);
        }

        public Task<ValidationResult> ValidateAsync(string folderPath)
        {
            return this.validatePublishing.ValidateAsync(folderPath);
        }
    }
}
