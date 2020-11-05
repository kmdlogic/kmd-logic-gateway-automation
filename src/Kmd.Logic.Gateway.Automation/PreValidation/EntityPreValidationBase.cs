using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Kmd.Logic.Gateway.Automation
{
    public class EntityPreValidationBase
    {
        private readonly MediaConfiguration _mediaConfiguration = new MediaConfiguration()
        {
            MaxLogoSizeBytes = 1000000,
            MaxMarkDownDocumentSizeBytes = 1000000,
            MaxOpenApiSpecSizeBytes = 20971520,
            MaxPolicyXmlSizeBytes = 10240,
            AllowedLogoFileExtensions = new[] { ".png", ".jpeg", ".jpg" },
            AllowedMarkdownDocumentExtension = ".md",
            AllowedOpenApiSpecExtension = ".json",
            AllowedPolicyXmlExtension = ".xml",
        };

        public EntityPreValidationBase(string folderPath)
        {
            this.FolderPath = folderPath;
            this.ValidationResults = new List<PublishResult>();
        }

        public string FolderPath { get; }

        public List<PublishResult> ValidationResults { get; }

        protected bool ValidateFile(GatewayFileType fileType, string path, string entityName, string propName)
        {
            switch (fileType)
            {
                case GatewayFileType.Logo:
                    return this.ValidateFileExist(path, entityName, propName)
                        && this.ValidateLogoSize(path, entityName)
                        && this.ValidateLogoType(path, entityName);
                case GatewayFileType.Document:
                    return this.ValidateFileExist(path, entityName, propName)
                        && this.ValidateDocumentSize(path, entityName)
                        && this.ValidateDocumentType(path, entityName);
                case GatewayFileType.OpenApiSpec:
                    return this.ValidateFileExist(path, entityName, propName)
                        && this.ValidateOpenApiSpecSize(path, entityName)
                         && this.ValidateOpenApiSpecType(path, entityName);
                case GatewayFileType.PolicyXml:
                    return this.ValidateFileExist(path, entityName, propName)
                        && this.ValidatePolicyXmlSize(path, entityName)
                         && this.ValidatePolicyXmlType(path, entityName);
            }

            return true;
        }

        private bool ValidatePolicyXmlType(string path, string entityName)
        {
            var fileType = new FileInfo(Path.Combine(this.FolderPath, path)).Extension;
            if (this._mediaConfiguration.AllowedPolicyXmlExtension != fileType)
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Invalid PolicyXml file type: unable to recognize type for {entityName}" });
                return false;
            }

            return true;
        }

        private bool ValidatePolicyXmlSize(string path, string entityName)
        {
            using var file = File.OpenRead(Path.Combine(this.FolderPath, path));
            if (file.Length > this._mediaConfiguration.MaxPolicyXmlSizeBytes)
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"PolicyXml size limit ({this._mediaConfiguration.MaxPolicyXmlSizeBytes} bytes) exceeds for {entityName}" });
                return false;
            }

            return true;
        }

        private bool ValidateOpenApiSpecType(string path, string entityName)
        {
            var fileType = new FileInfo(Path.Combine(this.FolderPath, path)).Extension;
            if (this._mediaConfiguration.AllowedOpenApiSpecExtension != fileType)
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Invalid OpenApiSpec file type: unable to recognize type for {entityName}" });
                return false;
            }

            return true;
        }

        private bool ValidateOpenApiSpecSize(string path, string entityName)
        {
            using var file = File.OpenRead(Path.Combine(this.FolderPath, path));
            if (file.Length > this._mediaConfiguration.MaxOpenApiSpecSizeBytes)
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"OpenApiSpec size limit ({this._mediaConfiguration.MaxOpenApiSpecSizeBytes} bytes) exceeds for {entityName}" });
                return false;
            }

            return true;
        }

        private bool ValidateDocumentType(string path, string entityName)
        {
            var fileType = new FileInfo(Path.Combine(this.FolderPath, path)).Extension;
            if (this._mediaConfiguration.AllowedMarkdownDocumentExtension != fileType)
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Invalid Document file type: unable to recognize type for {entityName}" });
                return false;
            }

            return true;
        }

        private bool ValidateDocumentSize(string path, string entityName)
        {
            using var file = File.OpenRead(Path.Combine(this.FolderPath, path));
            if (file.Length > this._mediaConfiguration.MaxMarkDownDocumentSizeBytes)
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Markdown document size limit ({this._mediaConfiguration.MaxMarkDownDocumentSizeBytes} bytes) exceeds for {entityName}" });
                return false;
            }

            return true;
        }

        private bool ValidateFileExist(string path, string entityName, string propName)
        {
            if (string.IsNullOrEmpty(path))
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"{propName} not specified for {entityName}" });
                return false;
            }

            path = path.Replace(@"\", "/", true, CultureInfo.InvariantCulture);
            if (!File.Exists(Path.Combine(this.FolderPath, path)))
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"{path} not found for {entityName}" });
                return false;
            }

            return true;
        }

        private bool ValidateLogoSize(string path, string entityName)
        {
            using var file = File.OpenRead(Path.Combine(this.FolderPath, path));
            if (file.Length > this._mediaConfiguration.MaxLogoSizeBytes)
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Image size limit ({this._mediaConfiguration.MaxLogoSizeBytes} bytes) exceeds for {entityName}" });
                return false;
            }

            return true;
        }

        private bool ValidateLogoType(string path, string entityName)
        {
            var fileType = new FileInfo(Path.Combine(this.FolderPath, path)).Extension;
            if (!this._mediaConfiguration.AllowedLogoFileExtensions.Any(ext => ext == fileType))
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Invalid logo file type: unable to recognize type for {entityName}" });
                return false;
            }

            return true;
        }
    }
}