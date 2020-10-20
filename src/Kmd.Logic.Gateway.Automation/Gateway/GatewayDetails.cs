using System.Collections.Generic;

namespace Kmd.Logic.Gateway.Automation.Gateway
{
    public class GatewayDetails
    {
        public IEnumerable<Product> Products { get; set; }

        public IEnumerable<Api> Apis { get; set; }
    }
}
