namespace Kmd.Logic.Gateway.Automation.Client
{
    public class RateLimitPolicyValidationModel
    {
        internal RateLimitPolicyValidationModel(string name, int calls, int renewalPeriod)
        {
            this.Name = name;
            this.Calls = calls;
            this.RenewalPeriod = renewalPeriod;
        }

        public string Name { get; }

        public int Calls { get; }

        public int RenewalPeriod { get; }
    }
}
