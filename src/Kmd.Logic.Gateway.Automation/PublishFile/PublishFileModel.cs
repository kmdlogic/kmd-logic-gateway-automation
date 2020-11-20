using System.Collections.Generic;

namespace Kmd.Logic.Gateway.Automation.PublishFile
{
    internal class PublishFileModel
    {
        public IEnumerable<Product> Products { get; set; } = new List<Product>();

        public IEnumerable<Api> Apis { get; set; } = new List<Api>();
    }
}
