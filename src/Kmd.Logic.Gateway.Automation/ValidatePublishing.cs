﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.Client;
using Kmd.Logic.Gateway.Automation.PreValidation;
using Kmd.Logic.Gateway.Automation.PublishFile;
using Kmd.Logic.Identity.Authorization;
using YamlDotNet.Serialization;

namespace Kmd.Logic.Gateway.Automation
{
    internal class ValidatePublishing
    {
        private readonly GatewayClientFactory gatewayClientFactory;
        private readonly GatewayOptions options;
        private readonly HttpClient httpClient;
        private readonly LogicTokenProviderFactory logicTokenProviderFactory;

        public ValidatePublishing(HttpClient httpClient, LogicTokenProviderFactory tokenProviderFactory, GatewayOptions options)
        {
            this.options = options;
            this.gatewayClientFactory = new GatewayClientFactory(tokenProviderFactory, httpClient, options);
            this.httpClient = httpClient;
            this.logicTokenProviderFactory = tokenProviderFactory;
        }

        public async Task<ValidationResult> ValidateAsync(string folderPath)
        {
            var publishYml = File.ReadAllText(Path.Combine(folderPath, "publish.yml"));
            var publishFileModel = new Deserializer().Deserialize<PublishFileModel>(publishYml);

            var preValidationsResults = PreValidateEntities(folderPath, publishFileModel, this.httpClient, this.options, this.logicTokenProviderFactory);
            if (preValidationsResults.Any(r => r.IsError))
            {
                return new ValidationResult(preValidationsResults);
            }

            using var client = this.gatewayClientFactory.CreateClient();
            using var validatePublishingRequest = GetValidatePublishingRequest(folderPath, this.options.ProviderId, publishFileModel);
            var validatePublishingResult = await client.ValidatePublishingAsync(
                this.options.SubscriptionId, validatePublishingRequest).ConfigureAwait(false);

            return validatePublishingResult.IsSuccess
                ? new ValidationResult(validatePublishingResult)
                : new ValidationResult(GetValidatePublishingErrors(validatePublishingResult));
        }

        private static IEnumerable<GatewayAutomationResult> GetValidatePublishingErrors(ValidatePublishingResult validatePublishingResult)
        {
            var results = new List<GatewayAutomationResult>();

            foreach (var api in validatePublishingResult.Apis)
            {
                if (api.Errors.Any())
                {
                    results.Add(new GatewayAutomationResult
                    {
                        ResultCode = ResultCode.ValidationFailed,
                        Message = $"[API] {api.Name}, {api.Path}/{api.Version}: {string.Join(", ", api.Errors)}",
                        IsError = true,
                    });
                }

                foreach (var rev in api.Revisions)
                {
                    if (rev.Errors.Any())
                    {
                        results.Add(new GatewayAutomationResult
                        {
                            ResultCode = ResultCode.ValidationFailed,
                            Message = $"[API Revision] {api.Name}, {api.Path}/{api.Version}: {string.Join(", ", rev.Errors)}",
                            IsError = true,
                        });
                    }
                }
            }

            foreach (var product in validatePublishingResult.Products)
            {
                if (product.Errors.Any())
                {
                    results.Add(new GatewayAutomationResult
                    {
                        ResultCode = ResultCode.ValidationFailed,
                        Message = $"[Product] {product.Name}: {string.Join(", ", product.Errors)}",
                        IsError = true,
                    });
                }
            }

            return results;
        }

        private static IEnumerable<GatewayAutomationResult> PreValidateEntities(string folderPath, PublishFileModel publishFileModel, HttpClient httpClient, GatewayOptions options, LogicTokenProviderFactory logicTokenProvider)
        {
            var preValidations = new List<IPreValidation>
            {
                new ProductsPreValidation(folderPath),
                new ApisPreValidation(folderPath, httpClient, logicTokenProvider,  options),
            };

            var publishResults = new List<GatewayAutomationResult>();
            foreach (var validation in preValidations)
            {
                var results = validation.ValidateAsync(publishFileModel);
                publishResults.AddRange(results);
            }

            return publishResults;
        }

        private static ValidatePublishingRequest GetValidatePublishingRequest(string folderPath, Guid providerId, PublishFileModel input)
        {
            var apis = new List<ApiValidationModel>();
            foreach (var api in input.Apis)
            {
                foreach (var apiVersion in api.ApiVersions)
                {
                    var fs = new FileStream(Path.Combine(folderPath, apiVersion.OpenApiSpecFile), FileMode.Open, FileAccess.Read);
                    var revisions = apiVersion.Revisions?.Select(r =>
                    {
                        var fsRev = new FileStream(Path.Combine(folderPath, r.OpenApiSpecFile), FileMode.Open, FileAccess.Read);
                        return new ApiRevisionValidationModel(fsRev, r.RevisionDescription);
                    });
                    apis.Add(new ApiValidationModel(
                        api.Name,
                        api.Path,
                        apiVersion.VersionName,
                        fs,
                        apiVersion.ProductNames,
                        revisions));
                }
            }

            var products = input.Products.Select(p => new ProductValidationModel(p.Name));

            return new ValidatePublishingRequest(providerId, apis, products);
        }
    }
}
