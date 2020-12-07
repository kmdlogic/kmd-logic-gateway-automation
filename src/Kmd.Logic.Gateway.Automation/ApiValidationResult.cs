using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Gateway.Automation
{
    public class ApiValidationResult : ItemValidationResultBase
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string Version { get; set; }

        public Guid? ApiVersionSetId { get; set; }

        public IEnumerable<ApiRevisionValidationResult> Revisions { get; set; }

        public PoliciesValidationResult Policies { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"* API Name: {this.Name}");
            sb.AppendLine($"* Path: {this.Path}");
            sb.AppendLine($"* Version: {this.Version}");
            sb.Append(this.ApiVersionSetId.HasValue ? $"* API version set: {this.ApiVersionSetId.Value}\n" : string.Empty);
            sb.Append(base.ToString());

            foreach (var rev in this.Revisions)
            {
                sb.AppendLine();
                sb.Append(rev.ToString());
            }

            return sb.ToString();
        }
    }
}
