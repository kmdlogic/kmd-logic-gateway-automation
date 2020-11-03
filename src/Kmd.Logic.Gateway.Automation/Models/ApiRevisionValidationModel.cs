using System.IO;

namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ApiRevisionValidationModel
    {
        public ApiRevisionValidationModel(Stream openApiSpec, string description)
        {
            this.OpenApiSpec = openApiSpec;
            this.Description = description;
        }

        public Stream OpenApiSpec { get; }

        public string Description { get; }
    }
}
