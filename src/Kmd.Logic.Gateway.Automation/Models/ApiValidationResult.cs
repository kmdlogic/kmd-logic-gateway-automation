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
            var result = $"* API Name: {this.Name}\n";
            result += $"* Path: {this.Path}\n";
            result += $"* Version: {this.Version}\n";
            result += this.ApiId.HasValue ? $"* Api ID: {this.ApiId.Value}\n" : string.Empty;
            result += this.ApiVersionSetId.HasValue ? $"* API version set: {this.ApiVersionSetId.Value}\n" : string.Empty;
            result += base.ToString();
            result += "\n";

            foreach (var rev in this.Revisions)
            {
                result += rev.ToString();
                result += "\n";
            }

            return result;
        }
    }
}
