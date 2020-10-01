using CommandLine;

namespace Kmd.Logic.Gateway.Automation.Tool
{
    internal class CommandBase
    {
        [Option('t', "token", Required = true, HelpText = "Token used for authentication with Logic.")]
        public string AuthToken { get; set; }

        [Option('s', "subscriptionId", Required = true, HelpText = "Subscription ID in Logic.")]
        public string SubscriptionId { get; set; }

        [Option("verbose", HelpText = "Logs everything")]
        public bool Verbose { get; set; }
    }
}
