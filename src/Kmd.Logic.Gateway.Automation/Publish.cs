using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.Client;
using Kmd.Logic.Gateway.Automation.Client.Models;
using Kmd.Logic.Gateway.Automation.PublishFile;
using Kmd.Logic.Identity.Authorization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Kmd.Logic.Gateway.Automation
{
    internal class Publish
    {
        private readonly GatewayOptions options;
        private readonly ValidatePublishing validatePublishing;
        private readonly PublishApis publishApis;
        private readonly PublishProducts publishProducts;
        private List<GatewayAutomationResult> publishResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="Publish"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client to use. The caller is expected to manage this resource and it will not be disposed.</param>
        /// <param name="tokenProviderFactory">The Logic access token provider factory.</param>
        /// <param name="options">The required configuration options.</param>
        public Publish(HttpClient httpClient, LogicTokenProviderFactory tokenProviderFactory, GatewayOptions options)
        {
            if (tokenProviderFactory == null)
            {
                throw new ArgumentNullException(nameof(tokenProviderFactory));
            }

            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.validatePublishing = new ValidatePublishing(httpClient, tokenProviderFactory, options);
            this.publishResults = new List<GatewayAutomationResult>();
            this.publishApis = new PublishApis(httpClient, tokenProviderFactory, options, this.publishResults);
            this.publishProducts = new PublishProducts(httpClient, tokenProviderFactory, options, this.publishResults);
        }

        /// <summary>
        /// Create gateway entities.
        /// </summary>
        /// <param name="folderPath">Folder path provider all gateway entries details.</param>
        /// <returns>Error details on failure, gateway entities name on success.</returns>
        public async Task<IEnumerable<GatewayAutomationResult>> PublishAsync(string folderPath)
        {
            this.publishResults.Clear();

            if (!this.IsValidInput(folderPath))
            {
                return this.publishResults;
            }

            PublishFileModel publishFileModel;
            using var publishYml = File.OpenText(Path.Combine(folderPath, @"publish.yml"));
            try
            {
                publishFileModel = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build().Deserialize<PublishFileModel>(publishYml);
            }
            catch (Exception e) when (e is YamlDotNet.Core.SemanticErrorException || e is YamlDotNet.Core.SyntaxErrorException)
            {
                this.publishResults.Add(new GatewayAutomationResult() { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = "Invalid yaml file, check is publish yaml file is having format and syntax errors" });
                return this.publishResults;
            }

            var validationResult = await this.validatePublishing.ValidateAsync(folderPath).ConfigureAwait(false);
            if (validationResult.IsError)
            {
                this.publishResults.AddRange(validationResult.Errors);
                return this.publishResults;
            }
            else
            {
                await this.publishProducts.CreateOrUpdateProducts(
                      subscriptionId: this.options.SubscriptionId,
                      providerId: this.options.ProviderId.Value,
                      products: publishFileModel.Products,
                      productValidationResults: validationResult.ValidatePublishingResult.Products,
                      folderPath: folderPath).ConfigureAwait(false);

                await this.publishApis.CreateOrUpdateApis(
                    subscriptionId: this.options.SubscriptionId,
                    providerId: this.options.ProviderId.Value,
                    apis: publishFileModel.Apis,
                    apiValidationResults: validationResult.ValidatePublishingResult.Apis,
                    folderPath: folderPath).ConfigureAwait(false);
            }

            return this.publishResults;
        }

        private bool IsValidInput(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                this.publishResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = "Specified folder doesn’t exist" });
                return false;
            }

            if (!File.Exists(Path.Combine(folderPath, @"publish.yml")))
            {
                this.publishResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = "Publish yml not found" });
                return false;
            }

            return true;
        }
    }
}