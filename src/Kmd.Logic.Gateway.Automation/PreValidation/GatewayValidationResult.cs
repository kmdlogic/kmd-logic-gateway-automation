using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Gateway.Automation
{
   public class GatewayValidationResult
    {
        public bool IsError { get; set; }

        public IEnumerable<PublishResult> PublishResults { get; set; }
   }
}
