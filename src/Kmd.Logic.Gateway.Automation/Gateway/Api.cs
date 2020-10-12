using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Gateway.Automation.Gateway
{
    public class Api
    {
        public string Name { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        public IList<ApiVersion> ApiVersions { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        public string Path { get; set; }
    }
}
