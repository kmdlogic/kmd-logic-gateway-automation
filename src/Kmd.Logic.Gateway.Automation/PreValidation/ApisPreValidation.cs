using System;
using System.Collections.Generic;
using System.Linq;
using Kmd.Logic.Gateway.Automation.PublishFile;

namespace Kmd.Logic.Gateway.Automation.PreValidation
{
    internal class ApisPreValidation : EntityPreValidationBase, IPreValidation
    {
        public ApisPreValidation(string folderPath)
            : base(folderPath)
        {
        }

        public IEnumerable<GatewayAutomationResult> ValidateAsync(PublishFileModel publishFileModel)
        {
            if (publishFileModel != null)
            {
                var duplicateApis = publishFileModel.Apis.GroupBy(x => x.Name).Any(x => x.Count() > 1);
                if (duplicateApis)
                {
                    this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"Duplicate api names exist" });
                }

                foreach (var api in publishFileModel.Apis)
                {
                    var apiPrefix = $"API: {api.Name}, {api.Path}";
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

                        this.ValidateFile(FileType.PolicyXml, version.PolicyXmlFile, apiVersionPrefix, nameof(version.PolicyXmlFile));
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
                    }
                }
            }

            return this.ValidationResults;
        }
    }
}
