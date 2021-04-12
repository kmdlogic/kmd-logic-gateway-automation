namespace Kmd.Logic.Gateway.Automation.Client
{
    internal class ProductValidationModel
    {
        public ProductValidationModel(string name, string key, PoliciesValidationModel policies)
        {
            this.Name = name;
            this.Key = key;
            this.Policies = policies;
        }

        public string Name { get; }

        public string Key { get; }

        public PoliciesValidationModel Policies { get; }
    }
}
