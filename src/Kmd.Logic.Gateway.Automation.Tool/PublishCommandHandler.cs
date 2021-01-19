using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kmd.Logic.Gateway.Automation.Tool
{
    internal class PublishCommandHandler : CommandHandlerBase
    {
        public async Task<int> Handle(PublishCommand cmd)
        {
            this.Initialize(cmd);

            using var httpClient = new HttpClient();
            var gatewayAutomation = new GatewayAutomation(httpClient, this.logicTokenProviderFactory, this.gatewayOptions);
            var results = await gatewayAutomation.PublishAsync(cmd.FolderPath).ConfigureAwait(false);

            this.outputFormatter.PrintResults(results);

            return results.Any(result => result.IsError) ? 1 : 0;
        }
    }
}
