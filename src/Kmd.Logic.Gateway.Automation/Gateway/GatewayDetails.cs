using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Gateway.Automation.Gateway
{
    public class GatewayDetails
    {
#pragma warning disable CA2227 // Collection properties should be read only
        public IList<Product> Products { get; set; }

        public IList<Api> Apis { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
