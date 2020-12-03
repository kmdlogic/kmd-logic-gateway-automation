namespace Kmd.Logic.Gateway.Automation.PublishFile
{
    public class RateLimitPolicy
    {
        public string Name { get; }

        public int Calls { get; }

        public int RenewalPeriod { get; }
    }
}
