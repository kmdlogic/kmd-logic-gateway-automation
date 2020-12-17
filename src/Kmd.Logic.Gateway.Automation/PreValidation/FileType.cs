namespace Kmd.Logic.Gateway.Automation.PreValidation
{
    internal enum FileType
    {
        /// <summary>
        /// Logo
        /// </summary>
        Logo,

        /// <summary>
        /// Markdown document
        /// </summary>
        Document,

        /// <summary>
        /// JSON Open API spec of API
        /// </summary>
        OpenApiSpec,

        /// <summary>
        /// Policy XML file
        /// </summary>
        CustomPolicyXml,
    }
}