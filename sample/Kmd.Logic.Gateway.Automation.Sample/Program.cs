using System;
using System.Net.Http;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation;
using Kmd.Logic.Identity.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest;
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

            using (var httpClient = new HttpClient())
            using (var tokenProviderFactory = new LogicTokenProviderFactory(configuration.TokenProvider))
            {
                var publish = new Publish(httpClient, tokenProviderFactory, configuration.Gateway);
                var results = await publish.Process(configuration.FolderPath).ConfigureAwait(false);

                foreach (var result in results)
                {
                    Console.WriteLine(result.ToString());
                }

                //var tokenProvider = tokenProviderFactory.GetProvider(httpClient);

                //IGatewayClient gatewayClient = new GatewayClient(new TokenCredentials(tokenProvider))
                //{
                //    BaseUri = configuration.GatewayApiBaseUri,
                //};

                //var result = await gatewayClient.GetAllProductsWithHttpMessagesAsync(subscriptionId: configuration.Gateway.SubscriptionId).ConfigureAwait(false);
            }

            Console.WriteLine("WIP");
        }
#pragma warning restore CA1031
    }
}