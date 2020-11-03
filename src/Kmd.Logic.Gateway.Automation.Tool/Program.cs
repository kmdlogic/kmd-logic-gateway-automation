using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using Serilog;

namespace Kmd.Logic.Gateway.Automation.Tool
{
    internal static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                using var helpWriter = new StringWriter();
                using var commandLineParser = new Parser(s =>
                {
                    s.HelpWriter = helpWriter;
                    s.CaseSensitive = false;
                    s.CaseInsensitiveEnumValues = true;
                });

                var result = await commandLineParser.ParseArguments<PublishCommand, ValidateCommand>(args)
                    .WithParsed((CommandBase o) =>
                    {
                        InitLogger(o.Verbose);
                        Log.Verbose("Started KMD Logic Gateway Automation Tool");
                        Log.Verbose("Arguments {@Parsed}", o);
                    })
                    .MapResult(
                        (PublishCommand cmd) => new PublishCommandHandler().Handle(cmd),
                        (ValidateCommand cmd) => new ValidateCommandHandler().Handle(cmd),
                        errs =>
                        {
                            Console.WriteLine(helpWriter.ToString());
                            return Task.FromResult(1);
                        })
                    .ConfigureAwait(false);

                Log.CloseAndFlush();
                return result;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Log.Error(ex, "Unexpected error: {Message}", ex.Message);
                Log.CloseAndFlush();
                return -1;
            }
        }

        private static void InitLogger(bool verbose)
        {
            var minLevel = verbose ? Serilog.Events.LogEventLevel.Verbose : Serilog.Events.LogEventLevel.Information;
            var minLogLevelSwitch = new Serilog.Core.LoggingLevelSwitch(minLevel);
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(minLogLevelSwitch)
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}
