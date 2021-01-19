using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Kmd.Logic.Gateway.Automation.Tool.OutputFormatters
{
    public static class GatewayAutomationResultsExtension
    {
        public static JObject ToJson(this IEnumerable<GatewayAutomationResult> gatewayAutomationResults)
        {
            dynamic result = new JObject();
            result.Results = new JArray();
            if (gatewayAutomationResults != null)
            {
                foreach (var gatewayAutomationResultItem in gatewayAutomationResults)
                {
                    dynamic resultItemJson = new JObject();

                    if (gatewayAutomationResultItem.EntityId.HasValue)
                    {
                        resultItemJson.EntityId = gatewayAutomationResultItem.EntityId.Value;
                    }

                    if (!string.IsNullOrEmpty(gatewayAutomationResultItem.EntityName))
                    {
                        resultItemJson.Name = gatewayAutomationResultItem.EntityName;
                    }

                    var resultCodeName = Enum.GetName(typeof(ResultCode), gatewayAutomationResultItem.ResultCode);

                    if (gatewayAutomationResultItem.IsError)
                    {
                        resultItemJson.ErrorCode = resultCodeName;
                    }
                    else
                    {
                        resultItemJson.ResultCode = resultCodeName;
                    }

                    if (!string.IsNullOrEmpty(gatewayAutomationResultItem.Message))
                    {
                        resultItemJson.Message = gatewayAutomationResultItem.Message;
                    }

                    result.Results.Add(resultItemJson);
                }
            }

            return result;
        }
    }
}
