using System;

namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ProductValidationResult : ItemValidationResultBase
    {
        public string Name { get; set; }

        public Guid? ProductId { get; set; }

        public override string ToString()
        {
            var result = $"* Product Name: {this.Name}\n";
            result += this.ProductId.HasValue ? $"* Product ID: {this.ProductId.Value}\n" : string.Empty;
            result += base.ToString();
            result += "\n";
            return result;
        }
    }
}
