using FileTypeChecker;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Kmd.Logic.Gateway.Automation
{
    public class PreValidationBase
    {
        private readonly MediaConfiguration _mediaConfiguration = new MediaConfiguration()
        {
            MaxLogoSizeBytes = 1000000,
            MaxMarkDownDocumentSizeBytes = 1000000,
            MaxOpenApiSpecSizeBytes = 20971520,
            MaxPolicyXmlSizeBytes = 10240,
            AllowedLogoFileExtensions = new[] { "png", "jpeg", "jpg" },
            AllowedMarkdownDocumentExtension = ".md",
            AllowedOpenApiSpecExtension = ".json",
            AllowedPolicyXmlExtension = ".xml",
        };

        public PreValidationBase(string folderPath)
        {
            this.FolderPath = folderPath;
            this.ValidationResults = new List<PublishResult>();
        }

        public string FolderPath { get; }

        public List<PublishResult> ValidationResults { get; }

        protected bool ValidateFile(string path, string enityName, string propName, GatewayFileType fileType)
        {
            switch (fileType)
            {
                case GatewayFileType.Logo:
                    return this.ValidateFileExist(path, enityName, propName)
                        && this.ValidateLogoSize(path, enityName)
                        && this.ValidateLogoType(path, enityName);
                case GatewayFileType.Document:
                    return this.ValidateFileExist(path, enityName, propName)
                        && this.ValidateDocumentSize(path, enityName)
                        && this.ValidateDocumentType(path, enityName);
                case GatewayFileType.OpenApiSpec:
                    return this.ValidateFileExist(path, enityName, propName)
                        && this.ValidateOpenApiSpecSize(path, enityName)
                         && this.ValidateOpenApiSpecType(path, enityName);
                case GatewayFileType.PolicyXml:
                    return this.ValidateFileExist(path, enityName, propName)
                        && this.ValidatePolicyXmlSize(path, enityName)
                         && this.ValidatePolicyXmlType(path, enityName);
            }

            return true;
        }

        private bool ValidatePolicyXmlType(string path, string enityName)
        {
            using var file = File.OpenRead(Path.Combine(this.FolderPath, path));
            var fileType = FileTypeValidator.GetFileType(file).Extension;
            if (!FileTypeValidator.IsTypeRecognizable(file) || this._mediaConfiguration.AllowedPolicyXmlExtension != fileType)
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Invalid PolicyXml file type: unable to recognize type for {enityName}" });
                return false;
            }

            return true;
        }

        private bool ValidatePolicyXmlSize(string path, string enityName)
        {
            using var file = File.OpenRead(Path.Combine(this.FolderPath, path));
            if (file.Length > this._mediaConfiguration.MaxPolicyXmlSizeBytes)
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"PolicyXml size limit ({this._mediaConfiguration.MaxPolicyXmlSizeBytes} bytes) exceeds for {enityName}" });
                return false;
            }

            return true;
        }

        private bool ValidateOpenApiSpecType(string path, string enityName)
        {
            using var file = File.OpenRead(Path.Combine(this.FolderPath, path));
            var fileType = FileTypeValidator.GetFileType(file).Extension;
            if (!FileTypeValidator.IsTypeRecognizable(file) || this._mediaConfiguration.AllowedOpenApiSpecExtension != fileType)
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Invalid OpenApiSpec file type: unable to recognize type for {enityName}" });
                return false;
            }

            return true;
        }

        private bool ValidateOpenApiSpecSize(string path, string enityName)
        {
            using var file = File.OpenRead(Path.Combine(this.FolderPath, path));
            if (file.Length > this._mediaConfiguration.MaxOpenApiSpecSizeBytes)
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"OpenApiSpec size limit ({this._mediaConfiguration.MaxOpenApiSpecSizeBytes} bytes) exceeds for {enityName}" });
                return false;
            }

            return true;
        }

        private bool ValidateDocumentType(string path, string enityName)
        {
            using var file = File.OpenRead(Path.Combine(this.FolderPath, path));
            var fileType = FileTypeValidator.GetFileType(file).Extension;
            if (!FileTypeValidator.IsTypeRecognizable(file) || this._mediaConfiguration.AllowedMarkdownDocumentExtension != fileType)
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Invalid Document file type: unable to recognize type for {enityName}" });
                return false;
            }

            return true;
        }

        private bool ValidateDocumentSize(string path, string enityName)
        {
            using var file = File.OpenRead(Path.Combine(this.FolderPath, path));
            if (file.Length > this._mediaConfiguration.MaxMarkDownDocumentSizeBytes)
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Markdown document size limit ({this._mediaConfiguration.MaxMarkDownDocumentSizeBytes} bytes) exceeds for {enityName}" });
                return false;
            }

            return true;
        }

        private bool ValidateFileExist(string path, string enityName, string propName)
        {
            if (string.IsNullOrEmpty(path))
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"{propName} not specified for {enityName}" });
                return false;
            }

            path = path.Replace(@"\", "/", true, CultureInfo.InvariantCulture);
            if (!File.Exists(Path.Combine(this.FolderPath, path)))
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"{path} not found for {enityName}" });
                return false;
            }

            return true;
        }

        private bool ValidateLogoSize(string path, string enityName)
        {
            using var file = File.OpenRead(Path.Combine(this.FolderPath, path));
            if (file.Length > this._mediaConfiguration.MaxLogoSizeBytes)
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Image size limit ({this._mediaConfiguration.MaxLogoSizeBytes} bytes) exceeds for {enityName}" });
                return false;
            }

            return true;
        }

        private bool ValidateLogoType(string path, string enityName)
        {
            using var file = File.OpenRead(Path.Combine(this.FolderPath, path));
            var fileType = FileTypeValidator.GetFileType(file).Extension;
            if (!FileTypeValidator.IsTypeRecognizable(file) || !this._mediaConfiguration.AllowedLogoFileExtensions.Any(ext => ext == fileType))
            {
                this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Invalid logo file type: unable to recognize type for {enityName}" });
                return false;
            }

            return true;
        }
    }
}