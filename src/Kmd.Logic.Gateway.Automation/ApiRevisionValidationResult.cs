using System.Text;

namespace Kmd.Logic.Gateway.Automation
{
    public class ApiRevisionValidationResult : ItemValidationResultBase
    {
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("* Revision");
            sb.Append(base.ToString());
            return sb.ToString();
        }
    }
}
