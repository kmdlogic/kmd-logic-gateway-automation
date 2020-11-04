using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kmd.Logic.Gateway.Automation.Tool
{
    internal class PublishCommandHandler : CommandHandlerBase
    {
#pragma warning disable CA1801 // Remove unused parameter
        public async Task<int> Handle(PublishCommand cmd)
#pragma warning restore CA1801 // Remove unused parameter
        {
            this.Initialize(cmd);

            using var httpClient = new HttpClient();
            var publish = new Publish(httpClient, this.logicTokenProviderFactory, this.gatewayOptions);
            var results = await publish.ProcessAsync(cmd.FolderPath).ConfigureAwait(false);

            foreach (var result in results)
            {
                Console.WriteLine(result.ToString());
            }

            return 0;
        }
    }
}
