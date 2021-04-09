using System.Collections.Generic;
using System.Linq;
using Kmd.Logic.Gateway.Automation.PublishFile;

namespace Kmd.Logic.Gateway.Automation.PreValidation
{
    internal class ProductsPreValidation : EntityPreValidationBase, IPreValidation
    {
        public ProductsPreValidation(string folderPath)
           : base(folderPath)
        {
        }

        public IEnumerable<GatewayAutomationResult> ValidateAsync(PublishFileModel publishFileModel)
        {
            if (publishFileModel != null && publishFileModel.Products.Any())
            {
                var duplicateProducts = publishFileModel.Products.GroupBy(x => x.Name).Any(x => x.Count() > 1);
                if (duplicateProducts)
                {
                    this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"Duplicate product names exist" });
                }

                var duplicateProductKey = publishFileModel.Products.GroupBy(x => x.Key).Any(x => x.Key != null && x.Count() > 1);
                if (duplicateProductKey)
                {
                    this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"Duplicate product keys exist" });
                }

                foreach (var product in publishFileModel.Products)
                {
                    var productPrefix = $"Product: {product.Name}";
                    if (string.IsNullOrEmpty(product.Name))
                    {
                        this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{productPrefix}] {nameof(product.Name)} does not exist" });
                    }

                    this.ValidateFile(FileType.Logo, product.Logo, product.Name, nameof(product.Logo));
                    this.ValidateFile(FileType.Document, product.Documentation, product.Name, nameof(product.Documentation));

                    this.ValidatePolicies(product, productPrefix);
                }
            }

            return this.ValidationResults;
        }

        private void ValidatePolicies(Product product, string productPrefix)
        {
            if (product.CustomPolicies != null)
            {
                foreach (var customPolicy in product.CustomPolicies)
                {
                    var policyPrefix = string.IsNullOrEmpty(customPolicy.Name)
                        ? $"{productPrefix}, Custom policy"
                        : $"{productPrefix}, Custom policy: {customPolicy.Name}";
                    this.ValidateFile(FileType.CustomPolicyXml, customPolicy.XmlFile, policyPrefix, nameof(customPolicy.XmlFile));
                }
            }
        }
    }
}
