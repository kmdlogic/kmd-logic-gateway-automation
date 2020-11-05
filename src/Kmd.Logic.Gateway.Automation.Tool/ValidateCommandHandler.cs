using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Rest;

namespace Kmd.Logic.Gateway.Automation.Tool
{
    internal class ValidateCommandHandler : CommandHandlerBase
    {
        public async Task<int> Handle(ValidateCommand cmd)
        {
            this.Initialize(cmd);

            try
            {
                using var httpClient = new HttpClient();
                var gatewayAutomation = new GatewayAutomation(httpClient, this.logicTokenProviderFactory, this.gatewayOptions);
                var result = await gatewayAutomation.ValidateAsync(cmd.FolderPath).ConfigureAwait(false);
                Console.WriteLine(result.ToString());

                return result.IsSuccess ? 0 : 2;
            }
            catch (RestException re)
            {
                Console.WriteLine(re.Message);
                return 1;
            }
        }
    }
}
