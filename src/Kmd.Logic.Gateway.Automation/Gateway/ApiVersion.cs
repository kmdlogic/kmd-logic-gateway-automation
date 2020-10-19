using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Gateway.Automation.Gateway
{
    public class ApiVersion
    {
        public string PathIdentifier { get; set; }

        public string ApiLogoFile { get; set; }

        public string ApiDocumentation { get; set; }

        public string OpenApiSpecFile { get; set; }

        public string Published { get; set; }

        public IEnumerable<string> ProductNames { get; set; }

        public string PoliciesXmlFile { get; set; }

        public IEnumerable<Revision> Revisions { get; set; }
    }
}
