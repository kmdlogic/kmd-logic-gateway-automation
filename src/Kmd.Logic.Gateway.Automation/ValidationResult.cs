using System.Collections.Generic;
using System.Linq;

namespace Kmd.Logic.Gateway.Automation
{
    public class ValidationResult
    {
        public ValidationResult(IEnumerable<GatewayAutomationResult> errors)
        {
            this.IsError = true;
            this.Errors = errors;
            this.ValidatePublishingResult = ValidatePublishingResult.Empty();
        }

        public ValidationResult(ValidatePublishingResult validatePublishingResult)
        {
            this.IsError = false;
            this.Errors = Enumerable.Empty<GatewayAutomationResult>();
            this.ValidatePublishingResult = validatePublishingResult;
        }

        public bool IsError { get; }

        public IEnumerable<GatewayAutomationResult> Errors { get; }

        public ValidatePublishingResult ValidatePublishingResult { get; }
    }
}
