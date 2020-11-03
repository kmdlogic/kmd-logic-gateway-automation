using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmd.Logic.Identity.Authorization;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Kmd.Logic.Gateway.Automation.Sample
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            InitLogger();

            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddUserSecrets(typeof(Program).Assembly)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build()
                    .Get<AppConfiguration>();

                await Run(config).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                Log.Fatal(ex, "Caught a fatal unhandled exception");
            }
#pragma warning restore CA1031 // Do not catch general exception types
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void InitLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
        }

#pragma warning disable CS1998
        private static async Task Run(AppConfiguration configuration)
        {
            var validator = new ConfigurationValidator(configuration);

            if (!validator.Validate())
            {
                return;
            }

            using var httpClient = new HttpClient();
            using var tokenProviderFactory = new LogicTokenProviderFactory(configuration.TokenProvider);

            Console.Write("Publishing validation... ");
            var validatePublishing = new ValidatePublishing(httpClient, tokenProviderFactory, configuration.Gateway);
            var publishingValidationResult = await validatePublishing.Validate(configuration.FolderPath).ConfigureAwait(false);
            Console.WriteLine(publishingValidationResult.IsSuccess ? "Success" : "Failure");
            PrintPublishingValidationResult(publishingValidationResult);

            if (publishingValidationResult.IsSuccess)
            {
                Console.WriteLine("Publishing...");
                var publish = new Publish(httpClient, tokenProviderFactory, configuration.Gateway);
                var results = await publish.ProcessAsync(configuration.FolderPath).ConfigureAwait(false);

                foreach (var result in results)
                {
                    Console.WriteLine(result.ToString());
                }
            }

            Console.WriteLine("WIP");
        }
#pragma warning restore CA1031

        private static void PrintPublishingValidationResult(Models.ValidatePublishingResponse result)
        {
            Console.WriteLine("***** APIS *****");
            foreach (var api in result.Apis)
            {
                Console.WriteLine($"* API {api.Name} ({api.Path}/{api.Version}) - {api.Status}");
                if (api.Errors.Any())
                {
                    Console.WriteLine("Errors:");
                    Console.WriteLine(string.Join(string.Empty, api.Errors.Select(e => "\t" + e + "\n")));
                }
                else
                {
                    Console.WriteLine("No errors");
                }

                Console.WriteLine();
                foreach (var rev in api.Revisions)
                {
                    Console.WriteLine($"Revision ({rev.ApiRevisionId}) - {rev.Status}");
                    if (rev.Errors.Any())
                    {
                        Console.WriteLine("Errors:");
                        Console.WriteLine(string.Join(string.Empty, rev.Errors.Select(e => "\t" + e + "\n")));
                    }
                    else
                    {
                        Console.WriteLine("No errors");
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("***** PRODUCTS *****");
            foreach (var product in result.Products)
            {
                Console.WriteLine($"* {product.Name} - {product.Status}");
                if (product.Errors.Any())
                {
                    Console.WriteLine("Errors:");
                    Console.WriteLine(string.Join(string.Empty, product.Errors.Select(e => "\t" + e + "\n")));
                }
                else
                {
                    Console.WriteLine("No errors");
                }

                Console.WriteLine();
            }
        }
    }
}