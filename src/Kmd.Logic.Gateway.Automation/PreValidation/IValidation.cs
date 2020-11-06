using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.PublishFile;

namespace Kmd.Logic.Gateway.Automation.PreValidation
{
    internal interface IValidation
    {
        Task<GatewayValidationResult> ValidateAsync(GatewayDetails gatewayDetails);
    }
}
