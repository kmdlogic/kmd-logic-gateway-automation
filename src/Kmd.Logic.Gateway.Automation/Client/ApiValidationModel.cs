using System.Collections.Generic;
using System.IO;

namespace Kmd.Logic.Gateway.Automation.Client
{
    internal class ApiValidationModel
    {
        public ApiValidationModel(
            string name,
            string path,
            string version,
            Stream openApiSpec,
            IEnumerable<string> productNames,
            IEnumerable<ApiRevisionValidationModel> revisions,
            PoliciesValidationModel policies)
        {
            this.Name = name;
            this.Path = path;
            this.Version = version;
            this.OpenApiSpec = openApiSpec;
            this.ProductNames = productNames;
            this.Revisions = revisions;
            this.Policies = policies;
        }

        public string Name { get; }

        public string Path { get; }

        public string Version { get; }

        public Stream OpenApiSpec { get; }

        public IEnumerable<string> ProductNames { get; }

        public IEnumerable<ApiRevisionValidationModel> Revisions { get; }

        public PoliciesValidationModel Policies { get; }
    }
}
