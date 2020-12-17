using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kmd.Logic.Gateway.Automation.PreValidation
{
    internal class EntityPreValidationBase
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
            this.ValidationResults = new List<GatewayAutomationResult>();
        }

        public string FolderPath { get; }

        public List<GatewayAutomationResult> ValidationResults { get; }

        protected bool ValidateFile(FileType fileType, string path, string entityName, string propName)
        {
            return fileType switch
            {
                FileType.Logo =>
                    this.ValidateFileExist(path, entityName, propName) &&
                    (this.ValidateLogoSize(path, entityName) ||
                    this.ValidateLogoType(path, entityName)),
                FileType.Document =>
                    this.ValidateFileExist(path, entityName, propName) &&
                    (this.ValidateDocumentSize(path, entityName) ||
                    this.ValidateDocumentType(path, entityName)),
                FileType.OpenApiSpec =>
                    this.ValidateFileExist(path, entityName, propName) &&
                    (this.ValidateOpenApiSpecSize(path, entityName) ||
                    this.ValidateOpenApiSpecType(path, entityName)),
                FileType.CustomPolicyXml =>
                    !string.IsNullOrEmpty(path) && this.ValidateFileExist(path, entityName, propName) &&
                    (this.ValidatePolicyXmlSize(path, entityName) ||
                    this.ValidatePolicyXmlType(path, entityName)),
                _ => true,
            };
        }

        private bool ValidatePolicyXmlType(string path, string entityName)
        {
            var fileType = new FileInfo(Path.Combine(this.FolderPath, path)).Extension;
            if (this._mediaConfiguration.AllowedPolicyXmlExtension != fileType)
            {
                this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{entityName}]: Invalid PolicyXml file type: unable to recognize type" });
                return false;
            }

            return true;
        }

        private bool ValidatePolicyXmlSize(string path, string entityName)
        {
            using var file = File.OpenRead(Path.Combine(this.FolderPath, path));
            if (file.Length > this._mediaConfiguration.MaxPolicyXmlSizeBytes)
            {
                this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{entityName}]: PolicyXml size limit ({this._mediaConfiguration.MaxPolicyXmlSizeBytes} bytes) exceeds" });
                return false;
            }

            return true;
        }

        private bool ValidateOpenApiSpecType(string path, string entityName)
        {
            var fileType = new FileInfo(Path.Combine(this.FolderPath, path)).Extension;
            if (this._mediaConfiguration.AllowedOpenApiSpecExtension != fileType)
            {
                this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{entityName}]: Invalid OpenApiSpec file type: unable to recognize type" });
                return false;
            }

            return true;
        }

        private bool ValidateOpenApiSpecSize(string path, string entityName)
        {
            using var file = File.OpenRead(Path.Combine(this.FolderPath, path));
            if (file.Length > this._mediaConfiguration.MaxOpenApiSpecSizeBytes)
            {
                this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{entityName}]: OpenApiSpec size limit ({this._mediaConfiguration.MaxOpenApiSpecSizeBytes} bytes) exceeds" });
                return false;
            }

            return true;
        }

        private bool ValidateDocumentType(string path, string entityName)
        {
            var fileType = new FileInfo(Path.Combine(this.FolderPath, path)).Extension;
            if (this._mediaConfiguration.AllowedMarkdownDocumentExtension != fileType)
            {
                this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{entityName}]: Invalid Document file type: unable to recognize type" });
                return false;
            }

            return true;
        }

        private bool ValidateDocumentSize(string path, string entityName)
        {
            using var file = File.OpenRead(Path.Combine(this.FolderPath, path));
            if (file.Length > this._mediaConfiguration.MaxMarkDownDocumentSizeBytes)
            {
                this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{entityName}]: Markdown document size limit ({this._mediaConfiguration.MaxMarkDownDocumentSizeBytes} bytes) exceeds" });
                return false;
            }

            return true;
        }

        private bool ValidateFileExist(string path, string entityName, string propName)
        {
            if (string.IsNullOrEmpty(path))
            {
                this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{entityName}]: {propName} not specified" });
                return false;
            }

            path = path.Replace(@"\", "/");
            if (!File.Exists(Path.Combine(this.FolderPath, path)))
            {
                this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{entityName}]: {path} not found" });
                return false;
            }

            return true;
        }

        private bool ValidateLogoSize(string path, string entityName)
        {
            using var file = File.OpenRead(Path.Combine(this.FolderPath, path));
            if (file.Length > this._mediaConfiguration.MaxLogoSizeBytes)
            {
                this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{entityName}]: Image size limit ({this._mediaConfiguration.MaxLogoSizeBytes} bytes) exceeds" });
                return false;
            }

            return true;
        }

        private bool ValidateLogoType(string path, string entityName)
        {
            var fileType = new FileInfo(Path.Combine(this.FolderPath, path)).Extension;
            if (!this._mediaConfiguration.AllowedLogoFileExtensions.Any(ext => ext == fileType))
            {
                this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{entityName}]: Invalid logo file type: unable to recognize type" });
                return false;
            }

            return true;
        }
    }
}