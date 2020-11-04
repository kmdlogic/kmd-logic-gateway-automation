using System;

namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ApiRevisionValidationResult : ItemValidationResultBase
    {
        public Guid? ApiRevisionId { get; set; }

        public override string ToString(string name)
        {
            name += this.ApiRevisionId.HasValue
                ? $"Revision: {this.ApiRevisionId.Value}"
                : "Revision";
            return base.ToString(name);
        }
    }
}
