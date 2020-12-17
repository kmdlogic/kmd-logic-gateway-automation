namespace Kmd.Logic.Gateway.Automation.PublishFile
{
    public class RateLimitPolicy
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Calls { get; set; }

        public int RenewalPeriod { get; set; }
    }
}
