using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kmd.Logic.Gateway.Automation.Client;
using Kmd.Logic.Gateway.Automation.PublishFile;
using Kmd.Logic.Identity.Authorization;

namespace Kmd.Logic.Gateway.Automation.PreValidation
{
    internal class ApisPreValidation : EntityPreValidationBase, IPreValidation
    {
        private readonly GatewayClientFactory gatewayClientFactory;
        private readonly GatewayOptions options;

        public ApisPreValidation(string folderPath, HttpClient httpClient, LogicTokenProviderFactory tokenProviderFactory, GatewayOptions options)
            : base(folderPath)
        {
            this.options = options;
            this.gatewayClientFactory = new GatewayClientFactory(tokenProviderFactory, httpClient, options);
        }

        public IEnumerable<GatewayAutomationResult> ValidateAsync(PublishFileModel publishFileModel)
        {
            if (publishFileModel != null && publishFileModel.Apis.Any())
            {
                var duplicateApis = publishFileModel.Apis.GroupBy(x => x.Name).Any(x => x.Count() > 1);
                if (duplicateApis)
                {
                    this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"Duplicate api names exist" });
                }

                using var client = this.gatewayClientFactory.CreateClient();
                var products = client.GetAllProducts(this.options.SubscriptionId, this.options.ProviderId);
                var nameofproducts = products.Select(x => x.Name).ToList();
                var consolidatedProductNames = nameofproducts.Union(publishFileModel.Products.Select(y => y.Name).ToList());

                foreach (var api in publishFileModel.Apis)
                {
                    var apiPrefix = $"API: {api.Name}, {api.Path}";

                    if (api.ApiVersions == null)
                    {
                        this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"Version shouldn't be null" });
                        continue;
                    }

                    if (api.ApiVersions
                            .Where(v => v.IsCurrent.HasValue)
                            .Count(v => v.IsCurrent == true) != 1)
                    {
                        this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{apiPrefix}] Only one Version must be set as current" });
                    }

                    foreach (var version in api.ApiVersions)
                    {
                        foreach (var product in version.ProductNames)
                        {
                            if (!consolidatedProductNames.Contains(product))
                            {
                                this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"Product {product} doesn't exist in DB or YAML" });
                            }
                        }
                    }

                    var duplicateVersions = api.ApiVersions.GroupBy(x => x.VersionName).Any(x => x.Count() > 1);
                    if (duplicateVersions)
                    {
                        this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{apiPrefix}] Duplicate version names exist" });
                    }

                    if (string.IsNullOrEmpty(api.Name))
                    {
                        this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{apiPrefix}] {nameof(api.Name)} does not exist" });
                    }

                    if (string.IsNullOrEmpty(api.Path))
                    {
                        this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{apiPrefix}] {nameof(api.Path)} does not exist" });
                    }

                    foreach (var version in api.ApiVersions)
                    {
                        var apiVersionPrefix = $"API: {api.Name}, {api.Path}/{version.VersionName}";
                        if (string.IsNullOrEmpty(version.VersionName))
                        {
                            this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{apiPrefix}] {nameof(version.VersionName)} does not exist" });
                        }

                        if (string.IsNullOrEmpty(version.BackendLocation) || !Uri.IsWellFormedUriString(version.BackendLocation, UriKind.Absolute))
                        {
                            this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{apiVersionPrefix}] {nameof(version.BackendLocation)} does not exist or not valid uri format" });
                        }

                        if (!version.IsCurrent.HasValue)
                        {
                            this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{apiVersionPrefix}] {nameof(version.IsCurrent)} is not specified" });
                        }

                        this.ValidateFile(FileType.Logo, version.ApiLogoFile, apiVersionPrefix, nameof(version.ApiLogoFile));
                        this.ValidateFile(FileType.Document, version.ApiDocumentation, apiVersionPrefix, nameof(version.ApiDocumentation));
                        this.ValidateFile(FileType.OpenApiSpec, version.OpenApiSpecFile, apiVersionPrefix, nameof(version.OpenApiSpecFile));

                        if (version.Revisions != null)
                        {
                            foreach (var revision in version.Revisions)
                            {
                                var apiRevisionPrefix = $"API Revision: {api.Name}, {api.Path}/{version.VersionName}";
                                if (string.IsNullOrEmpty(revision.RevisionDescription))
                                {
                                    this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{apiRevisionPrefix}] {nameof(revision.RevisionDescription)} not exist" });
                                }

                                this.ValidateFile(FileType.OpenApiSpec, revision.OpenApiSpecFile, $"{api.Name} - {version.VersionName} - {revision.RevisionDescription}", nameof(revision.OpenApiSpecFile));
                            }
                        }

                        if (version.CustomPolicies != null)
                        {
                            foreach (var customPolicy in version.CustomPolicies)
                            {
                                this.ValidateFile(FileType.PolicyXml, customPolicy.PolicyXmlFile, apiVersionPrefix, nameof(customPolicy.PolicyXmlFile));
                            }
                        }
                    }
                }
            }

            return this.ValidationResults;
        }
    }
}
