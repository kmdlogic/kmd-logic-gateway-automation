using System.Text;

namespace Kmd.Logic.Gateway.Automation
{
    public class ProductValidationResult : ItemValidationResultBase
    {
        public string Name { get; set; }

        public PoliciesValidationResult Policies { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"* Product Name: {this.Name}");
            sb.Append(this.EntityId.HasValue ? $"* Product ID: {this.EntityId.Value}\n" : string.Empty);
            sb.Append(base.ToString());
            return sb.ToString();
        }
    }
}
