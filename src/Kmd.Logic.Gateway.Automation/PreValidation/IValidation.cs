using System.Collections.Generic;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.Gateway;

namespace Kmd.Logic.Gateway.Automation
{
    public interface IValidation
    {
        Task<bool> ValidateAsync(GatewayDetails gatewayDetails);

        public IEnumerable<PublishResult> PublishResults { get; }
    }
}
