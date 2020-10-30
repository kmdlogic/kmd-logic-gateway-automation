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
        private IList<PublishResult> publishResults;

        private readonly MediaConfiguration _mediaConfiguration = new MediaConfiguration()
        {
            MaxLogoSizeBytes = 1000000,
            MaxMarkDownDocumentSizeBytes = 1000000,
            MaxOpenApiSpecSizeBytes = 20971520,
            AllowedLogoMimeTypes = new[] { ".png", ".jpeg" },
            AllowedMarkdownDocumentExtension = ".md",
        };

        public IList<PublishResult> IsProductAndFolderValid(string folderPath)
        {
            this.publishResults = new List<PublishResult>();

            if (!Directory.Exists(folderPath))
            {
                this.publishResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = "Specified folder doesn’t exist" });
                return this.publishResults;
            }

            if (!File.Exists(Path.Combine(folderPath, @"publish.yml")))
            {
                this.publishResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = "Publish yml not found" });
                return this.publishResults;
            }

            using (var publishYml = File.OpenText(Path.Combine(folderPath, @"publish.yml")))
            {
                var yaml = new Deserializer().Deserialize<GatewayDetails>(publishYml);

                foreach (var product in yaml.Products)
                {
#pragma warning disable CA1307 // Folder path remains the same always
                    var logoPath = product.Logo.Replace(@"\", "/");
                    var markdownPath = product.Documentation.Replace(@"\", "/");
#pragma warning restore CA1307 // Folder path remains the same always

                    if (!File.Exists(Path.Combine(folderPath, logoPath)))
                    {
                        this.publishResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = "Logo not found" });
                        return this.publishResults;
                    }
                    else if (File.Exists(Path.Combine(folderPath, logoPath)))
                    {
                        FileInfo productImage = new FileInfo(Path.Combine(folderPath, logoPath));

                        if (productImage.Length > this._mediaConfiguration.MaxLogoSizeBytes)
                        {
                            this.publishResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = "Logo size exceeds the limit" });
                            return this.publishResults;
                        }
                        else if (!this._mediaConfiguration.AllowedLogoMimeTypes.Contains(productImage.Extension))
                        {
                            this.publishResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = "Logo format is not supported" });
                            return this.publishResults;
                        }
                    }

                    if (!File.Exists(Path.Combine(folderPath, markdownPath)))
                    {
                        this.publishResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = "Markdown document not found" });
                        return this.publishResults;
                    }
                    else if (File.Exists(Path.Combine(folderPath, markdownPath)))
                    {
                        FileInfo productMdFile = new FileInfo(Path.Combine(folderPath, markdownPath));

                        if (productMdFile.Length > this._mediaConfiguration.MaxMarkDownDocumentSizeBytes)
                        {
                            this.publishResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = "MarkDown file size exceeds the limit" });
                            return this.publishResults;
                        }
                        else if (!(this._mediaConfiguration.AllowedMarkdownDocumentExtension == productMdFile.Extension))
                        {
                            this.publishResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = "MarkDown file is not supported" });
                            return this.publishResults;
                        }
                    }
                }
            }

            this.publishResults.Add(new PublishResult { IsError = false, ResultCode = ResultCode.ProductValidated, Message = "Product documents validated" });
            return this.publishResults;
        }
    }
}