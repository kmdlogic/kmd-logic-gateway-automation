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

    public enum GatewayFileType
    {
        Logo,
        Document,
        OpenApiSpec,
        PolicyXml,
    }
}