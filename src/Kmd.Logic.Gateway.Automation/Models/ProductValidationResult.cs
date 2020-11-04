using System;

namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ProductValidationResult : ItemValidationResultBase
    {
        public string Name { get; set; }

        public Guid? ProductId { get; set; }

        public override string ToString()
        {
            var product = $"Product: {this.Name}";
            var result = this.ToString(product);

            result += this.ProductId.HasValue ? $"* Product ID: {this.ProductId.Value}\n" : string.Empty;

            return result;
        }
    }
}
