using System.Collections.Generic;
using System.Linq;

namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ValidatePublishingResponse
    {
        public IEnumerable<string> Errors { get; set; }

        public IEnumerable<ApiValidationResult> Apis { get; set; }

        public IEnumerable<ProductValidationResult> Products { get; set; }

        public bool IsSuccess { get; set; }

        public override string ToString()
        {
            var result = string.Empty;

            if (this.Errors.Any())
            {
                foreach (var error in this.Errors)
                {
                    result += $"[Validate publishing error] {error}\n";
                }

                result += "\n";
            }

            foreach (var api in this.Apis)
            {
                result += api.ToString();
                result += "\n";
            }

            foreach (var product in this.Products)
            {
                result += product.ToString();
                result += "\n";
            }

            return result;
        }
    }
}
