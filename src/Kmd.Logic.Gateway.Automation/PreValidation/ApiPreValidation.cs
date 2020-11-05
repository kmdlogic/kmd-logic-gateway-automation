using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.Gateway;

namespace Kmd.Logic.Gateway.Automation
{
    public class ApiPreValidation : EntityPreValidationBase, IValidation
    {
        public ApiPreValidation(string folderPath)
            : base(folderPath)
        {
        }

        public Task<bool> ValidateAsync(GatewayDetails gatewayDetails)
        {
            var isValidationSuccess = true;
            if (gatewayDetails != null)
            {
                foreach (var api in gatewayDetails.Apis)
                {
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
                            && !this.ValidateFile(GatewayFileType.PolicyXml, version.PolicyXmlFile, $"{api.Name} - {version.VersionName}", nameof(version.PolicyXmlFile)))
                        {
                            isValidationSuccess = false;
                        }

                        if (!this.ValidateFile(GatewayFileType.Logo, version.ApiLogoFile, $"{api.Name} - {version.VersionName}", nameof(version.ApiLogoFile)))
                        {
                            isValidationSuccess = false;
                        }

                        if (!this.ValidateFile(GatewayFileType.Document, version.ApiDocumentation, $"{api.Name} - {version.VersionName}", nameof(version.ApiDocumentation)))
                        {
                            isValidationSuccess = false;
                        }

                        if (!this.ValidateFile(GatewayFileType.OpenApiSpec, version.OpenApiSpecFile, $"{api.Name} - {version.VersionName}", nameof(version.OpenApiSpecFile)))
                        {
                            isValidationSuccess = false;
                        }
                    }
                }
            }

            return Task.FromResult<bool>(isValidationSuccess);
        }

        public List<PublishResult> PublishResults
        {
            get { return this.ValidationResults; }
        }
    }
}
