using System.Collections.Generic;

namespace Kmd.Logic.Gateway.Automation.PublishFile
{
    public class ApiVersion
    {
        public string VersionName { get; set; }

        public string ApiLogoFile { get; set; }

        public string ApiDocumentation { get; set; }

        public string OpenApiSpecFile { get; set; }

        public string Published { get; set; }

        public IEnumerable<string> ProductNames { get; set; }

        public string BackendLocation { get; set; }

        public IEnumerable<Revision> Revisions { get; set; }

        public string Visibility { get; set; }

        public ApiStatus? Status { get; set; }

        public bool? IsCurrent { get; set; }

        public RateLimitPolicy RateLimitPolicy { get; set; }

        public IEnumerable<CustomPolicy> CustomPolicies { get; set; }
    }
}
