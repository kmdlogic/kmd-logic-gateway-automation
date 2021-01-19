using System;
using CommandLine;

namespace Kmd.Logic.Gateway.Automation.Tool
{
    internal class CommandBase
    {
        [Option('g', "gatewayUrl", HelpText = "Gateway URL.")]
        public Uri GatewayUrl { get; set; }

        [Option('f', "folderPath", Required = true, HelpText = "Path of the root folder with 'publish.yml' file.")]
        public string FolderPath { get; set; }

        [Option("scope", HelpText = "Authorization scope in Logic Subscription Client Credentials.")]
        public string AuthorizationScope { get; set; } = "https://logicidentityprod.onmicrosoft.com/LogicAPI/user_impersonation";

        [Option("clientId", Required = true, HelpText = "Client ID in Logic Subscription Client Credentials.")]
        public string ClientId { get; set; }

        [Option("secret", Required = true, HelpText = "Client secret in Logic Subscription Client Credentials.")]
        public string ClientSecret { get; set; }

        [Option('s', "subscriptionId", Required = true, HelpText = "Subscription ID in Logic.")]
        public Guid SubscriptionId { get; set; }

        [Option('p', "providerId", HelpText = "Provider ID in Logic.")]
        public Guid ProviderId { get; set; }

        [Option('v', "verbose", HelpText = "Logs everything.")]
        public bool Verbose { get; set; }

        [Option('o', "output", HelpText = "Output format.")]
        public OutputFormat OutputFormat { get; set; }
    }
}
