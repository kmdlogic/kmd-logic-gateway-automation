using System;

namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ApiRevisionValidationResult : ItemValidationResultBase
    {
        public Guid? ApiRevisionId { get; set; }

        public override string ToString()
        {
            var result = "* Revision\n";
            result += this.ApiRevisionId.HasValue ? $"\t* Revision ID: {this.ApiRevisionId.Value}\n" : string.Empty;
            result += base.ToString();
            result += "\n";
            return result;
        }
    }
}
