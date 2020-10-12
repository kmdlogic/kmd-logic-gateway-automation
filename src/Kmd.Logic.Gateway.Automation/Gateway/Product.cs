using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Gateway.Automation.Gateway
{
    public class Product
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

        public string OpenidConfigCustomUrl { get; set; }

        public string ApplicationId { get; set; }
    }
}
