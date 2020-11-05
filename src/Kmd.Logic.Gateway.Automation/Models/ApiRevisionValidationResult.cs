using System;
using System.Text;

namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ApiRevisionValidationResult : ItemValidationResultBase
    {
        public Guid? ApiRevisionId { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("* Revision");
            sb.Append(this.ApiRevisionId.HasValue ? $"\t* Revision ID: {this.ApiRevisionId.Value}\n" : string.Empty);
            sb.Append(base.ToString());
            return sb.ToString();
        }
    }
}
