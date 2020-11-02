using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.Models;
using Microsoft.Rest;

namespace Kmd.Logic.Gateway.Automation.Tool
{
    internal class ValidateCommandHandler : CommandHandlerBase
    {
        public async Task<int> Handle(ValidateCommand cmd)
        {
            this.Initialize(cmd);

            using var httpClient = new HttpClient();
            var validatePublishing = new ValidatePublishing(httpClient, this.logicTokenProviderFactory, this.gatewayOptions);
            var result = await validatePublishing.Validate(cmd.FolderPath).ConfigureAwait(false);
            PrintPublishingValidationResult(result);

            return result.IsSuccess ? 0 : 2;
        }

        private static void PrintPublishingValidationResult(ValidatePublishingResponse result)
        {
            if (result.IsSuccess)
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
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error);
                }
            }
        }
    }
}
