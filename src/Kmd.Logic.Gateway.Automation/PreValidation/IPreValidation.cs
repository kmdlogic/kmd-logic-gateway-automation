using System.Collections.Generic;
using Kmd.Logic.Gateway.Automation.PublishFile;

namespace Kmd.Logic.Gateway.Automation.PreValidation
{
    internal interface IPreValidation
    {
        IEnumerable<GatewayAutomationResult> ValidateAsync(PublishFileModel publishFileModel);
    }
}
