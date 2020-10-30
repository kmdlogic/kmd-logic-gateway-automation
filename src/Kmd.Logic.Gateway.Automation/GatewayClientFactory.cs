using System.Net.Http;
using Kmd.Logic.Identity.Authorization;
using Microsoft.Rest;

namespace Kmd.Logic.Gateway.Automation
{
    internal class GatewayClientFactory
    {
        private readonly LogicTokenProviderFactory logicTokenProviderFactory;
        private readonly HttpClient httpClient;
        private readonly GatewayOptions gatewayOptions;

        public GatewayClientFactory(LogicTokenProviderFactory logicTokenProviderFactory, HttpClient httpClient, GatewayOptions gatewayOptions)
        {
            this.logicTokenProviderFactory = logicTokenProviderFactory;
            this.httpClient = httpClient;
            this.gatewayOptions = gatewayOptions;
        }

        public IGatewayClient CreateClient()
        {
            var tokenProvider = this.logicTokenProviderFactory.GetProvider(this.httpClient);
            var client = new GatewayClient(new TokenCredentials(tokenProvider))
            {
                BaseUri = this.gatewayOptions.GatewayServiceUri,
            };
            return client;
        }
    }
}
