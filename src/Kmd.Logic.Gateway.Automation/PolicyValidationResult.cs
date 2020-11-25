using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Gateway.Automation
{
    public class PolicyValidationResult
    {
        public Guid? PolicyId { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("* Policy");
            sb.Append(this.PolicyId.HasValue ? $"\t* Policy ID: {this.PolicyId.Value}\n" : string.Empty);
            sb.AppendLine($"* Policy Name: {this.Name}");
            sb.Append(base.ToString());
            return sb.ToString();
        }
    }
}
