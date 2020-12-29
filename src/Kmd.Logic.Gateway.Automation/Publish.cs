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

namespace Kmd.Logic.Gateway.Automation
{
    internal class Publish
    {
        private readonly GatewayOptions options;
        private readonly ValidatePublishing validatePublishing;
        private readonly PublishApis _publishApis;
        private readonly PublishProducts _publishProducts;
        private readonly PublishPolicies _publishPolicies;
        private List<GatewayAutomationResult> publishResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="Publish"/> class.
        /// </summary>
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
            this._publishPolicies = new PublishPolicies(httpClient, tokenProviderFactory, options, this.publishResults);
            this._publishApis = new PublishApis(httpClient, tokenProviderFactory, options, this.publishResults);
            this._publishProducts = new PublishProducts(httpClient, tokenProviderFactory, options, this.publishResults);
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
                publishFileModel = new Deserializer().Deserialize<PublishFileModel>(publishYml);
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
              await this._publishProducts.CreateOrUpdateProducts(
                    subscriptionId: this.options.SubscriptionId,
                    providerId: this.options.ProviderId,
                    products: publishFileModel.Products,
                    productValidationResults: validationResult.ValidatePublishingResult.Products,
                    folderPath: folderPath).ConfigureAwait(false);

              await this._publishApis.CreateOrUpdateApis(
                    subscriptionId: this.options.SubscriptionId,
                    providerId: this.options.ProviderId,
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