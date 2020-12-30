using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kmd.Logic.Gateway.Automation.Tool.OutputFormatters
{
    internal class JsonOutputFormatter : IOutputFormatter
    {
        private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        public void PrintResults(ValidationResult validationResult)
        {
            Console.WriteLine(JsonConvert.SerializeObject(validationResult.ToJson(), Formatting.Indented, this.jsonSerializerSettings));
        }

        public void PrintResults(IEnumerable<GatewayAutomationResult> gatewayAutomationResults)
        {
            Console.WriteLine(JsonConvert.SerializeObject(gatewayAutomationResults, Formatting.Indented, this.jsonSerializerSettings));
        }
    }
}
