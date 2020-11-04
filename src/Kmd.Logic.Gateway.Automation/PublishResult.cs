using System;

namespace Kmd.Logic.Gateway.Automation
{
    public class PublishResult
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
                if (this.ResultCode == ResultCode.PublishingValidationFailed)
                {
                    return $"Publishing validation failed\n{this.Message}";
                }

                return $"Error Code: {this.ResultCode}, Message: {this.Message}{entityMessage}";
            }

            if (this.ResultCode == ResultCode.PublishingValidationSuccess)
            {
                return $"Publishing validation succeeded\n{this.Message}";
            }

            return $"Result Code: {this.ResultCode}, Message: {entityMessage}";
        }
    }
}
