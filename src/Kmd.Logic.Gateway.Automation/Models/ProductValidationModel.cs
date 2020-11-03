namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ProductValidationModel
    {
        public ProductValidationModel(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
