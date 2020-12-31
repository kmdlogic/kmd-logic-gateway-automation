using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Gateway.Automation.Tool.OutputFormatters
{
    internal class ListOutputFormatter : IOutputFormatter
    {
        public void PrintResults(ValidationResult validationResult)
        {
            if (validationResult.IsError)
            {
                foreach (var error in validationResult.Errors)
                {
                    Console.WriteLine(error.ToString());
                }
            }
            else
            {
                Console.WriteLine(validationResult.ValidatePublishingResult.ToString());
            }
        }

        public void PrintResults(IEnumerable<GatewayAutomationResult> gatewayAutomationResults)
        {
            foreach (var result in gatewayAutomationResults)
            {
                Console.WriteLine(result.ToString());
            }
        }
    }
}
