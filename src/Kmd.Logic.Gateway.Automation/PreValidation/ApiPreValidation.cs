using Kmd.Logic.Gateway.Automation.Gateway;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Kmd.Logic.Gateway.Automation
{
    public class ApiPreValidation : PreValidationBase, IValidation
    {
        public ApiPreValidation(string folderPath)
            : base(folderPath)
        {
        }

        public async Task<bool> ValidateAsync(GatewayDetails gatewayDetails)
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

                    if (string.IsNullOrEmpty(api.Path) || !Uri.IsWellFormedUriString(api.Path, UriKind.Absolute))
                    {
                        this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Api Path not exist or not valid uri format" });
                        isValidationSuccess = false;
                    }

                    foreach (var version in api.ApiVersions)
                    {
                        if (string.IsNullOrEmpty(version.VersionName))
                        {
                            this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Api Name not exist" });
                            isValidationSuccess = false;
                        }

                        if (!string.IsNullOrEmpty(version.PoliciesXmlFile))
                        {
                            this.ValidateFile(version.PoliciesXmlFile, $"{api.Name} - {version.VersionName}", "PolicyXmlFile", GatewayFileType.PolicyXml);
                            isValidationSuccess = false;
                        }

                        if (!this.ValidateFile(version.ApiLogoFile, $"{api.Name} - {version.VersionName}", "Logo", GatewayFileType.Logo))
                        {
                            isValidationSuccess = false;
                        }

                        if (!this.ValidateFile(version.ApiDocumentation, $"{api.Name} - {version.VersionName}", "Documentation", GatewayFileType.Document))
                        {
                            isValidationSuccess = false;
                        }

                        if (!this.ValidateFile(version.OpenApiSpecFile, $"{api.Name} - {version.VersionName}", "OpenApiSpec", GatewayFileType.OpenApiSpec))
                        {
                            isValidationSuccess = false;
                        }
                    }
                }
            }

            return await Task.FromResult<bool>(isValidationSuccess).ConfigureAwait(false);
        }
    }
}
