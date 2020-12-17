using System.Collections.Generic;

namespace Kmd.Logic.Gateway.Automation
{
    public class PoliciesValidationResult
    {
        public RateLimitPolicyValidationResult RateLimitPolicy { get; set; }

        public IEnumerable<CustomPolicyValidationResult> CustomPolicies { get; set; }
    }
}
