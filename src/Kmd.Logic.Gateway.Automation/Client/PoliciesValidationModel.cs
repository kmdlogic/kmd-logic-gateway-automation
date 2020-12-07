using System.Collections.Generic;

namespace Kmd.Logic.Gateway.Automation.Client
{
    internal class PoliciesValidationModel
    {
        public PoliciesValidationModel(IEnumerable<CustomPolicyValidationModel> customPolicies, RateLimitPolicyValidationModel rateLimitPolicy)
        {
            this.CustomPolicies = customPolicies;
            this.RateLimitPolicy = rateLimitPolicy;
        }

        public IEnumerable<CustomPolicyValidationModel> CustomPolicies { get; }

        public RateLimitPolicyValidationModel RateLimitPolicy { get; }
    }
}
