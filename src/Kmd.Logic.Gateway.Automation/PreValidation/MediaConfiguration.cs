namespace Kmd.Logic.Gateway.Automation
{
    internal class MediaConfiguration
    {
        public int MaxLogoSizeBytes { get; set; }

        public int MaxMarkDownDocumentSizeBytes { get; set; }

        public int MaxOpenApiSpecSizeBytes { get; set; }

        public int MaxPolicyXmlSizeBytes { get; set; }

        public string[] AllowedLogoFileExtensions { get; set; }

        public string AllowedMarkdownDocumentExtension { get; set; }

        public string AllowedOpenApiSpecExtension { get; set; }

        public string AllowedPolicyXmlExtension { get; set; }
    }

    internal enum GatewayFileType
    {
        /// <summary>
        /// File type logo
        /// </summary>
        Logo,

        /// <summary>
        /// File type for markdown document
        /// </summary>
        Document,

        /// <summary>
        /// Json open api spec for api
        /// </summary>
        OpenApiSpec,

        /// <summary>
        /// Policy xml for api
        /// </summary>
        PolicyXml,
    }
}