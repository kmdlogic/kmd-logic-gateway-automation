using System.Collections.Generic;
using System.IO;

namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ApiValidationModel
    {
        public ApiValidationModel(
            string name,
            Stream openApiSpec,
            string path,
            string version,
            IEnumerable<string> productNames,
            IEnumerable<ApiRevisionValidationModel> revisions)
        {
            this.Name = name;
            this.OpenApiSpec = openApiSpec;
            this.Path = path;
            this.Version = version;
            this.ProductNames = productNames;
            this.Revisions = revisions;
        }

        public string Name { get; }

        public Stream OpenApiSpec { get; }

        public string Path { get; }

        public string Version { get; }

        public IEnumerable<string> ProductNames { get; }

        public IEnumerable<ApiRevisionValidationModel> Revisions { get; }
    }
}
