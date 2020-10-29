using System.Net.Http;
using Kmd.Logic.Identity.Authorization;
using Microsoft.Rest;

namespace Kmd.Logic.Gateway.Automation
{
    internal class GatewayClientFactory
    {
        private readonly LogicTokenProviderFactory _logicTokenProviderFactory;
        private readonly HttpClient _httpClient;
        private readonly GatewayOptions _gatewayOptions;

        public GatewayClientFactory(LogicTokenProviderFactory logicTokenProviderFactory, HttpClient httpClient, GatewayOptions gatewayOptions)
        {
            this._logicTokenProviderFactory = logicTokenProviderFactory;
            this._httpClient = httpClient;
            this._gatewayOptions = gatewayOptions;
        }

        public IGatewayClient CreateClient()
        {
            var tokenProvider = this._logicTokenProviderFactory.GetProvider(this._httpClient);
            var client = new GatewayClient(new TokenCredentials(tokenProvider))
            {
                BaseUri = this._gatewayOptions.GatewayServiceUri,
            };
            return client;
        }
    }
}
