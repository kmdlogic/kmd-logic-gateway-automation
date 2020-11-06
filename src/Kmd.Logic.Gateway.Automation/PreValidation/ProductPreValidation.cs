﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.Gateway;

namespace Kmd.Logic.Gateway.Automation.PreValidation
{
    public class ProductPreValidation : EntityPreValidationBase, IValidation
    {
        public ProductPreValidation(string folderPath)
           : base(folderPath)
        {
        }

        public Task<GatewayValidationResult> ValidateAsync(GatewayDetails gatewayDetails)
        {
            var isValidationSuccess = true;
            if (gatewayDetails != null)
            {
                var duplicateProducts = gatewayDetails.Products.GroupBy(x => x.Name).Any(x => x.Count() > 1);
                if (duplicateProducts)
                {
                    this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Duplicate product names exist" });
                    isValidationSuccess = false;
                }

                foreach (var product in gatewayDetails.Products)
                {
                    if (string.IsNullOrEmpty(product.Name))
                    {
                        this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Product name does not exist" });
                        isValidationSuccess = false;
                    }

                    if (!this.ValidateFile(GatewayFileType.Logo, product.Logo, product.Name, nameof(product.Logo)))
                    {
                        isValidationSuccess = false;
                    }

                    if (!this.ValidateFile(GatewayFileType.Document, product.Documentation, product.Name, nameof(product.Documentation)))
                    {
                        isValidationSuccess = false;
                    }
                }
            }

            var publishResult = new GatewayValidationResult()
            {
                IsError = isValidationSuccess,
                ValidationResults = this.ValidationResults,
            };
            return Task.FromResult(publishResult);
        }
    }
}
