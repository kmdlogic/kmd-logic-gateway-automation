using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kmd.Logic.Gateway.Automation.Tool.OutputFormatters
{
    internal class JsonOutputFormatter : IOutputFormatter
    {
        private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new ValidationResultContractResolver() };

        public void PrintResults(ValidationResult validationResult)
        {
            Console.WriteLine(JsonConvert.SerializeObject(validationResult, Formatting.Indented, this.jsonSerializerSettings));
        }

        public void PrintResults(IEnumerable<GatewayAutomationResult> gatewayAutomationResults)
        {
            Console.WriteLine(JsonConvert.SerializeObject(gatewayAutomationResults, Formatting.Indented, this.jsonSerializerSettings));
        }
    }
}
