using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Kmd.Logic.Gateway.Automation.Gateway;
using YamlDotNet.Serialization;

namespace Kmd.Logic.Gateway.Automation
{
    public class ValidateProduct
    {
        private readonly MediaConfiguration _mediaConfiguration = new MediaConfiguration()
        {
            MaxLogoSizeBytes = 1000000,
            MaxMarkDownDocumentSizeBytes = 1000000,
            MaxOpenApiSpecSizeBytes = 20971520,
            AllowedLogoMimeTypes = new[] { ".png", ".jpeg" },
            AllowedMarkdownDocumentExtension = ".md",
        };

        private IList<PublishResult> publishResults;

        public IList<PublishResult> ValidateProducts(string folderPath, GatewayDetails gatewayYaml)
        {
            this.publishResults = new List<PublishResult>();

            foreach (var product in gatewayYaml?.Products)
                {
#pragma warning disable CA1307 // Folder path remains the same always
                    var logoPath = product.Logo.Replace(@"\", "/");
                    var markdownPath = product.Documentation.Replace(@"\", "/");
#pragma warning restore CA1307 // Folder path remains the same always

                    if (!File.Exists(Path.Combine(folderPath, logoPath)))
                    {
                        this.publishResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Logo not found for product {product.Name}" });
                    }
                    else if (File.Exists(Path.Combine(folderPath, logoPath)))
                    {
                        FileInfo productImage = new FileInfo(Path.Combine(folderPath, logoPath));

                        if (productImage.Length > this._mediaConfiguration.MaxLogoSizeBytes)
                        {
                            this.publishResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Logo size exceeds the limit for product {product.Name}" });
                        }
                        else if (!this._mediaConfiguration.AllowedLogoMimeTypes.Contains(productImage.Extension))
                        {
                            this.publishResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Logo format is not supported for product {product.Name}" });
                        }
                    }

                    if (!File.Exists(Path.Combine(folderPath, markdownPath)))
                    {
                        this.publishResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Markdown document not found for product {product.Name}" });
                    }
                    else if (File.Exists(Path.Combine(folderPath, markdownPath)))
                    {
                        FileInfo productMdFile = new FileInfo(Path.Combine(folderPath, markdownPath));

                        if (productMdFile.Length > this._mediaConfiguration.MaxMarkDownDocumentSizeBytes)
                        {
                            this.publishResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"MarkDown file size exceeds the limit for product {product.Name}" });
                        }
                        else if (!(this._mediaConfiguration.AllowedMarkdownDocumentExtension == productMdFile.Extension))
                        {
                            this.publishResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"MarkDown file is not supported for product {product.Name}" });
                        }
                    }
                }

            return this.publishResults;
        }
    }
}