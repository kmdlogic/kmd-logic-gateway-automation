using System;
using System.Collections.Generic;

namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ValidatePublishingRequest
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
    }
}
