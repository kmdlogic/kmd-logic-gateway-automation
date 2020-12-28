using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Kmd.Logic.Gateway.Automation.Tool.OutputFormatters
{
    internal class JsonOutputFormatter : IOutputFormatter
    {
        private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

        public void PrintResults(ValidationResult validationResult)
        {
            Console.WriteLine(JsonSerializer.Serialize(validationResult, typeof(ValidationResult), this.jsonSerializerOptions));
        }

        public void PrintResults(IEnumerable<GatewayAutomationResult> gatewayAutomationResults)
        {
            Console.WriteLine(JsonSerializer.Serialize(gatewayAutomationResults, typeof(IEnumerable<GatewayAutomationResult>), this.jsonSerializerOptions));
        }
    }
}
