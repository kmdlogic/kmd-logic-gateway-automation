using System;
using CommandLine;

namespace Kmd.Logic.Gateway.Automation.Tool
{
    internal class CommandBase
    {
        [Option('f', "folderPath", Required = true, HelpText = "Path of the root folder with 'publish.yml' file.")]
        public string FolderPath { get; set; }

        [Option("authorizationScope", HelpText = "Authorization scope in Logic Subscription Client Credentials.")]
        public string AuthorizationScope { get; set; } = "https://logicidentityprod.onmicrosoft.com/bb159109-0ccd-4b08-8d0d-80370cedda84/.default";

        [Option("clientId", Required = true, HelpText = "Client ID in Logic Subscription Client Credentials.")]
        public string ClientId { get; set; }

        [Option("clientSecret", Required = true, HelpText = "Client secret in Logic Subscription Client Credentials.")]
        public string ClientSecret { get; set; }

        [Option('p', "providerId", Required = true, HelpText = "Provider ID in Logic.")]
        public Guid ProviderId { get; set; }

        [Option('s', "subscriptionId", Required = true, HelpText = "Subscription ID in Logic.")]
        public Guid SubscriptionId { get; set; }

        [Option("verbose", HelpText = "Logs everything")]
        public bool Verbose { get; set; }
    }
}
