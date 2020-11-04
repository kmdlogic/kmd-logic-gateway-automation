using System.Collections.Generic;
using System.Linq;

namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ItemValidationResultBase
    {
        public ValidationStatus Status { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public virtual string ToString(string name)
        {
            var result = string.Empty;
            if (this.Errors.Any())
            {
                foreach (var error in this.Errors)
                {
                    result += $"[{name} | Error] {error}\n";
                }
            }
            else
            {
                result = $"[{name}] {this.Status}\n";
            }

            return result;
        }
    }
}
