using System.Collections.Generic;
using System.Linq;

namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ItemValidationResultBase
    {
        public ValidationStatus Status { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public override string ToString()
        {
            var result = string.Empty;
            if (this.Errors.Any())
            {
                result += "Errors:\n";
                foreach (var error in this.Errors)
                {
                    result += $"\t- {error}\n";
                }
            }
            else
            {
                result = $"* Status: {this.Status}\n";
            }

            return result;
        }
    }
}
