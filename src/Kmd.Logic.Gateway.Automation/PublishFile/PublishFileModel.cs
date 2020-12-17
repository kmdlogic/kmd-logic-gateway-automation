using System.Collections.Generic;

namespace Kmd.Logic.Gateway.Automation.PublishFile
{
    internal class PublishFileModel
    {
        private IEnumerable<Product> products;

        public IEnumerable<Product> Products
        {
            get { return this.products ?? new List<Product>(); }
            set { this.products = value; }
        }

        private IEnumerable<Api> apis;

        public IEnumerable<Api> Apis
        {
            get { return this.apis ?? new List<Api>(); }
            set { this.apis = value; }
        }
    }
}
