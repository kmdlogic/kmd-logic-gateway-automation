namespace Kmd.Logic.Gateway.Automation.PublishFile
{
    public class RateLimitPolicy
    {
        public string Name { get; }

        public string Description { get; set; }

        public int Calls { get; }

        public int RenewalPeriod { get; }
    }
}
