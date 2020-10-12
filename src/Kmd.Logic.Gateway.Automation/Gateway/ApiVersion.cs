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

#pragma warning disable CA1819 // Properties should not return arrays
        public string[] ProductNames { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        public string PoliciesXmlFile { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        public IList<Revision> Revisions { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
