using System.Collections.Generic;

namespace Kmd.Logic.Gateway.Automation.Models
{
    internal class ValidatePublishingResponse
    {
        public IEnumerable<string> Errors { get; set; }

        public IEnumerable<ApiValidationResult> Apis { get; set; }

        public IEnumerable<ProductValidationResult> Products { get; set; }

        public bool IsSuccess { get; set; }
    }
}
