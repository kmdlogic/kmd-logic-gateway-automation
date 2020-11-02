using System;
using CommandLine;
using Kmd.Logic.Identity.Authorization;

namespace Kmd.Logic.Gateway.Automation.Tool
{

    internal abstract class CommandHandlerBase : IDisposable
    {
        protected LogicTokenProviderFactory logicTokenProviderFactory;
        protected GatewayOptions gatewayOptions;

        protected void Initialize(CommandBase cmd)
        {
            Console.WriteLine("Initialize");
            this.logicTokenProviderFactory = new LogicTokenProviderFactory(
                new LogicTokenProviderOptions
                {
                    AuthorizationScope = cmd.AuthorizationScope,
                    AuthorizationTokenIssuer = cmd.AuthorizationTokenIssuer,
                    ClientId = cmd.ClientId,
                    ClientSecret = cmd.ClientSecret,
                });

            this.gatewayOptions = new GatewayOptions
            {
                GatewayServiceUri = new Uri("https://kmd-logic-services-prod.azurefd.net"),
                SubscriptionId = cmd.SubscriptionId,
                ProviderId = cmd.ProviderId,
            };
        }

        public void Dispose()
        {
            if (this.logicTokenProviderFactory != null)
            {
                this.logicTokenProviderFactory.Dispose();
            }
        }
    }
}
