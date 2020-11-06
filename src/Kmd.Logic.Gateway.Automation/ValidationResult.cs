using System.Collections.Generic;
using System.Linq;

namespace Kmd.Logic.Gateway.Automation
{
    public class ValidationResult
    {
        public ValidationResult(IEnumerable<PublishResult> validationResults)
        {
            this.ValidationResults = validationResults;
        }

        public bool IsError
        {
            get
            {
                return this.ValidationResults.Any(r => r.IsError == true);
            }
        }

        public IEnumerable<PublishResult> ValidationResults { get; }
    }
}
