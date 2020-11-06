using System;
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

        public ValidationResult ValidateAsync(PublishFileModel publishFileModel)
        {
            if (publishFileModel != null)
            {
                var duplicateApis = publishFileModel.Apis.GroupBy(x => x.Name).Any(x => x.Count() > 1);
                if (duplicateApis)
                {
                    this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Duplicate api names exist" });
                }

                foreach (var api in publishFileModel.Apis)
                {
                    var duplicateVersions = api.ApiVersions.GroupBy(x => x.VersionName).Any(x => x.Count() > 1);
                    if (duplicateVersions)
                    {
                        this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"[Api: {api.Name}] Duplicate version names exist" });
                    }

                    if (string.IsNullOrEmpty(api.Name))
                    {
                        this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"[Api: {api.Name}] Name does not exist" });
                    }

                    if (string.IsNullOrEmpty(api.Path))
                    {
                        this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"[Api: {api.Name}] Api Path does not exist for {api.Name}" });
                    }

                    foreach (var version in api.ApiVersions)
                    {
                        if (string.IsNullOrEmpty(version.VersionName))
                        {
                            this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"[Api: {api.Name} - {version.VersionName}] Api version name does not exist" });
                        }

                        if (string.IsNullOrEmpty(version.BackendLocation) || !Uri.IsWellFormedUriString(version.BackendLocation, UriKind.Absolute))
                        {
                            this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"[Api: {api.Name} - {version.VersionName}] Api Backend Location does not exist or not valid uri format" });
                        }

                        this.ValidateFile(FileType.PolicyXml, version.PolicyXmlFile, $"{api.Name} - {version.VersionName}", nameof(version.PolicyXmlFile));
                        this.ValidateFile(FileType.Logo, version.ApiLogoFile, $"{api.Name} - {version.VersionName}", nameof(version.ApiLogoFile));
                        this.ValidateFile(FileType.Document, version.ApiDocumentation, $"{api.Name} - {version.VersionName}", nameof(version.ApiDocumentation));
                        this.ValidateFile(FileType.OpenApiSpec, version.OpenApiSpecFile, $"{api.Name} - {version.VersionName}", nameof(version.OpenApiSpecFile));

                        if (version.Revisions != null)
                        {
                            foreach (var revision in version.Revisions)
                            {
                                if (string.IsNullOrEmpty(revision.RevisionDescription))
                                {
                                    this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Revision Description not exist for {api.Name} - {version.VersionName}" });
                                }

                                this.ValidateFile(FileType.OpenApiSpec, revision.OpenApiSpecFile, $"{api.Name} - {version.VersionName} - {revision.RevisionDescription}", nameof(revision.OpenApiSpecFile));
                            }
                        }
                    }
                }
            }

            return new ValidationResult(this.ValidationResults);
        }
    }
}
