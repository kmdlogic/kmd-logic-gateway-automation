using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Gateway.Automation
{
    public class ProductValidationResult : ItemValidationResultBase
    {
        public string Name { get; set; }

        public Guid? ProductId { get; set; }

        public IEnumerable<PolicyValidationResult> Policies { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"* Product Name: {this.Name}");
            sb.Append(this.ProductId.HasValue ? $"* Product ID: {this.ProductId.Value}\n" : string.Empty);
            sb.Append(base.ToString());
            return sb.ToString();
        }
    }
}
