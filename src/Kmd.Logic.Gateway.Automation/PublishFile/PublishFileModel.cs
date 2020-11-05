using System.Collections.Generic;

namespace Kmd.Logic.Gateway.Automation.PublishFile
{
    public class PublishFileModel
    {
        public IEnumerable<Product> Products { get; set; }

        public IEnumerable<Api> Apis { get; set; }
    }
}
