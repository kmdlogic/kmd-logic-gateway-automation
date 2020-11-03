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
            var entityMessage = this.EntityId.HasValue ? $", Entity Id : {this.EntityId.Value}" : string.Empty;
            return this.IsError
                ? $"Error Code : {this.ResultCode}, Message : {this.Message}{entityMessage}"
                : $"Result Code : {this.ResultCode}{entityMessage}";
        }
    }
}
