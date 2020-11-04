using System;
using System.Collections.Generic;

namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ApiValidationResult : ItemValidationResultBase
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string Version { get; set; }

        public Guid? ApiId { get; set; }

        public Guid? ApiVersionSetId { get; set; }

        public IEnumerable<ApiRevisionValidationResult> Revisions { get; set; }

        public override string ToString()
        {
            var api = $"API: '{this.Name}' - {this.Path}/{this.Version}";
            var result = this.ToString(api);

            result += this.ApiId.HasValue ? $"* Api ID: {this.ApiId.Value}" : string.Empty;
            result += this.ApiVersionSetId.HasValue ? $"* API version set: {this.ApiId.Value}" : string.Empty;

            foreach (var rev in this.Revisions)
            {
                result += rev.ToString(api + ", ");
            }

            return result;
        }
    }
}
