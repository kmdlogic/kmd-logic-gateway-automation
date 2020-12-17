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
                var validationResult = await gatewayAutomation.ValidateAsync(cmd.FolderPath).ConfigureAwait(false);
                if (validationResult.IsError)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        Console.WriteLine(error.ToString());
                    }
                }
                else
                {
                    Console.WriteLine(validationResult.ValidatePublishingResult.ToString());
                }

                return !validationResult.IsError ? 0 : 1;
            }
            catch (RestException re)
            {
                Console.WriteLine(re.Message);
                return 1;
            }
        }
    }
}
