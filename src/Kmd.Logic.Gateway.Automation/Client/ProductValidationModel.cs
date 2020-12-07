namespace Kmd.Logic.Gateway.Automation.Client
{
    internal class ProductValidationModel
    {
        public ProductValidationModel(string name, PoliciesValidationModel policies)
        {
            this.Name = name;
            this.Policies = policies;
        }

        public string Name { get; }

        public PoliciesValidationModel Policies { get; }
    }
}
