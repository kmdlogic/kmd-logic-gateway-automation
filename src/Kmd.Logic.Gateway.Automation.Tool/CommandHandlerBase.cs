using System;
using Kmd.Logic.Gateway.Automation.Tool.OutputFormatters;
using Kmd.Logic.Identity.Authorization;

namespace Kmd.Logic.Gateway.Automation.Tool
{
    internal abstract class CommandHandlerBase : IDisposable
    {
#pragma warning disable SA1401 // Fields should be private
        protected LogicTokenProviderFactory logicTokenProviderFactory;
        protected GatewayOptions gatewayOptions;
        protected IOutputFormatter outputFormatter;
#pragma warning restore SA1401 // Fields should be private

        protected void Initialize(CommandBase cmd)
        {
            this.logicTokenProviderFactory = new LogicTokenProviderFactory(
                new LogicTokenProviderOptions
                {
                    AuthorizationScope = cmd.AuthorizationScope,
                    ClientId = cmd.ClientId,
                    ClientSecret = cmd.ClientSecret,
                });

            this.gatewayOptions = new GatewayOptions
            {
                SubscriptionId = cmd.SubscriptionId,
                ProviderId = cmd.ProviderId,
            };
            this.gatewayOptions.GatewayServiceUri = cmd.GatewayUrl ?? this.gatewayOptions.GatewayServiceUri;
            this.outputFormatter = cmd.OutputFormat == OutputFormat.Json ? new JsonOutputFormatter() : new ListOutputFormatter();
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
