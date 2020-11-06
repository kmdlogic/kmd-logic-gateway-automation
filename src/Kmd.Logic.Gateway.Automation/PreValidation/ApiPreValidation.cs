using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.Gateway;
using YamlDotNet.Serialization.NodeTypeResolvers;
using Kmd.Logic.Gateway.Automation.PublishFile;

namespace Kmd.Logic.Gateway.Automation.PreValidation
{
    internal class ApiPreValidation : EntityPreValidationBase, IValidation
    {
        public ApiPreValidation(string folderPath)
            : base(folderPath)
        {
        }

        public Task<GatewayValidationResult> ValidateAsync(GatewayDetails gatewayDetails)
        {
            var isValidationSuccess = true;
            if (publishFileModel != null)
            {
                var duplicateApis = gatewayDetails.Apis.GroupBy(x => x.Name).Any(x => x.Count() > 1);
                if (duplicateApis)
                {
                    this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Duplicate api names exist" });
                    isValidationSuccess = false;
                }

                foreach (var api in gatewayDetails.Apis)
                {
                    var duplicateVersions = api.ApiVersions.GroupBy(x => x.VersionName).Any(x => x.Count() > 1);
                    if (duplicateVersions)
                    {
                        this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Duplicate version names exist" });
                        isValidationSuccess = false;
                    }

                    if (string.IsNullOrEmpty(api.Name))
                    {
                        this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Api Name not exist" });
                        isValidationSuccess = false;
                    }

                    if (string.IsNullOrEmpty(api.Path))
                    {
                        this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Api Path not exist for {api.Name}" });
                        isValidationSuccess = false;
                    }

                    foreach (var version in api.ApiVersions)
                    {
                        if (string.IsNullOrEmpty(version.VersionName))
                        {
                            this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Api version name not exist" });
                            isValidationSuccess = false;
                        }

                        if (string.IsNullOrEmpty(version.BackendLocation) || !Uri.IsWellFormedUriString(version.BackendLocation, UriKind.Absolute))
                        {
                            this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Api Backend Location not exist or not valid uri format for {api.Name} - {version.VersionName}" });
                            isValidationSuccess = false;
                        }

                        if (!string.IsNullOrEmpty(version.PolicyXmlFile)
                            && !this.ValidateFile(FileType.PolicyXml, version.PolicyXmlFile, $"{api.Name} - {version.VersionName}", nameof(version.PolicyXmlFile)))
                        {
                            isValidationSuccess = false;
                        }

                        if (!this.ValidateFile(FileType.Logo, version.ApiLogoFile, $"{api.Name} - {version.VersionName}", nameof(version.ApiLogoFile)))
                        {
                            isValidationSuccess = false;
                        }

                        if (!this.ValidateFile(FileType.Document, version.ApiDocumentation, $"{api.Name} - {version.VersionName}", nameof(version.ApiDocumentation)))
                        {
                            isValidationSuccess = false;
                        }

                        if (!this.ValidateFile(FileType.OpenApiSpec, version.OpenApiSpecFile, $"{api.Name} - {version.VersionName}", nameof(version.OpenApiSpecFile)))
                        {
                            isValidationSuccess = false;
                        }

                        if (version.Revisions != null)
                        {
                            foreach (var revision in version.Revisions)
                            {
                                if (string.IsNullOrEmpty(revision.RevisionDescription))
                                {
                                    this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Revision Description not exist for {api.Name} - {version.VersionName}" });
                                    isValidationSuccess = false;
                                }

                                if (!this.ValidateFile(FileType.OpenApiSpec, revision.OpenApiSpecFile, $"{api.Name} - {version.VersionName} - {revision.RevisionDescription}", nameof(revision.OpenApiSpecFile)))
                                {
                                    isValidationSuccess = false;
                                }
                            }
                        }
                    }
                }
            }

            var publishResult = new GatewayValidationResult()
            {
                IsError = isValidationSuccess,
                ValidationResults = this.ValidationResults,
            };
            return Task.FromResult(publishResult);
        }
    }
}
