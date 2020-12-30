using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kmd.Logic.Gateway.Automation
{
    public class ItemValidationResultBase
    {
        public Guid? EntityId { get; set; }

        public ValidationStatus Status { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (this.Errors.Any())
            {
                sb.AppendLine("Errors:");
                foreach (var error in this.Errors)
                {
                    sb.AppendLine($"\t- {error}");
                }
            }
            else
            {
                sb.Append(this.EntityId.HasValue ? $"* Entity ID: {this.EntityId.Value}\n" : string.Empty);
                sb.AppendLine($"* Status: {this.Status}");
            }

            return sb.ToString();
        }
    }
}
