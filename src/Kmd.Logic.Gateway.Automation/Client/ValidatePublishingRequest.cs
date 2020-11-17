using System;
using System.Collections.Generic;

namespace Kmd.Logic.Gateway.Automation.Client
{
    internal class ValidatePublishingRequest : IDisposable
    {
        public ValidatePublishingRequest(Guid providerId, IEnumerable<ApiValidationModel> apis, IEnumerable<ProductValidationModel> products)
        {
            this.ProviderId = providerId;
            this.Apis = apis;
            this.Products = products;
        }

        public Guid ProviderId { get; }

        public IEnumerable<ApiValidationModel> Apis { get; }

        public IEnumerable<ProductValidationModel> Products { get; }

        public void Dispose()
        {
            if (this.Apis != null)
            {
                foreach (var api in this.Apis)
                {
                    api.OpenApiSpec?.Dispose();
                }
            }
        }
    }
}
