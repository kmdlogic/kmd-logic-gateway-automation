namespace Kmd.Logic.Gateway.Automation.PublishFile
{
    internal class Product
    {
        public string Name { get; set; }

        public string Logo { get; set; }

        public string Documentation { get; set; }

        public string PoliciesXmlFile { get; set; }

        public string Description { get; set; }

        public string Published { get; set; }

        public bool? ApiKeyRequired { get; set; }

        public bool? ClientCredentialRequired { get; set; }

        public bool? ProviderApprovalRequired { get; set; }

        public string LegalTerms { get; set; }

        public string Visibility { get; set; }

        public string OpenidConfigIssuer { get; set; }

#pragma warning disable CA1056 // URI-like properties should not be strings
        public string OpenidConfigCustomUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

        public string ApplicationId { get; set; }
    }
}
