using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Gateway.Automation
{
    public class MediaConfiguration
    {
        public long MaxLogoSizeBytes { get; set; }

        public long MaxMarkDownDocumentSizeBytes { get; set; }

        public IEnumerable<string> AllowedLogoMimeTypes { get; set; }

        public string AllowedMarkdownDocumentExtension { get; set; }

        public IEnumerable<string> AllowedMarkdownDocumentMimeTypes { get; set; }

        public long MaxOpenApiSpecSizeBytes { get; set; }
    }
}
