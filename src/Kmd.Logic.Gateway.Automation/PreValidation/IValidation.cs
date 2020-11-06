using System.Collections.Generic;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.Gateway;

namespace Kmd.Logic.Gateway.Automation
{
    public interface IValidation
    {
        Task<GatewayValidationResult> ValidateAsync(GatewayDetails gatewayDetails);
    }
}
