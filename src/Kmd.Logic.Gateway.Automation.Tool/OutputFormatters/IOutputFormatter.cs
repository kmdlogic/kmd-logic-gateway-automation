using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Gateway.Automation.Tool.OutputFormatters
{
    internal interface IOutputFormatter
    {
        void PrintResults(ValidationResult validationResult);

        void PrintResults(IEnumerable<GatewayAutomationResult> gatewayAutomationResults);
    }
}
