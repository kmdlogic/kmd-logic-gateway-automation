namespace Kmd.Logic.Gateway.Automation.Client
{
    internal class CustomPolicyValidationModel
    {
        public CustomPolicyValidationModel(string name, string xml)
        {
            this.Name = name;
            this.Xml = xml;
        }

        public string Name { get; }

        public string Xml { get; }
    }
}
