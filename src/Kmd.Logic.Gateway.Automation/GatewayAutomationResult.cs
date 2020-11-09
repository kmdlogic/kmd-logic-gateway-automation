using System;

namespace Kmd.Logic.Gateway.Automation
{
    public class GatewayAutomationResult
    {
        public ResultCode ResultCode { get; set; }

        public string Message { get; set; }

        public Guid? EntityId { get; set; }

        public bool IsError { get; set; }

        public override string ToString()
        {
            var entityMessage = this.EntityId.HasValue ? $", Entity Id: {this.EntityId.Value}" : string.Empty;

            if (this.IsError)
            {
                return $"Error Code: {this.ResultCode}, Message: {this.Message}{entityMessage}";
            }

            return $"Result Code: {this.ResultCode}, Message: {entityMessage}";
        }
    }
}
