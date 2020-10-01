using CommandLine;

namespace Kmd.Logic.Gateway.Automation.Tool
{
    [Verb("publish", HelpText = "Publishes product using YAML specification file.")]
    internal class PublishOptions : CommonOptions
    {
    }
}
