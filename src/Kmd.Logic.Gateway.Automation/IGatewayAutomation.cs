using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Logic.Gateway.Automation
{
    public interface IGatewayAutomation
    {
        Task<IEnumerable<GatewayAutomationResult>> PublishAsync(string folderPath);

        Task<ValidationResult> ValidateAsync(string folderPath);
    }
}
