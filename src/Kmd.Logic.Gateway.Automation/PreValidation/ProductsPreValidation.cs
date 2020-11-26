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

                foreach (var product in publishFileModel.Products)
                {
                    var productPrefix = $"Product: {product.Name}";
                    if (string.IsNullOrEmpty(product.Name))
                    {
                        this.ValidationResults.Add(new GatewayAutomationResult { IsError = true, ResultCode = ResultCode.ValidationFailed, Message = $"[{productPrefix}] {nameof(product.Name)} does not exist" });
                    }

                    this.ValidateFile(FileType.Logo, product.Logo, product.Name, nameof(product.Logo));
                    this.ValidateFile(FileType.Document, product.Documentation, product.Name, nameof(product.Documentation));

                    if (product.CustomPolicies != null && product.CustomPolicies.Any())
                    {
                        foreach (var customPolicy in product.CustomPolicies)
                        {
                            var policyName = string.IsNullOrEmpty(customPolicy.Name) ? "<no name>" : customPolicy.Name;
                            var policyPrefix = $"{productPrefix}, Custom policy: {policyName}";
                            this.ValidateFile(FileType.CustomPolicyXml, customPolicy.XmlFile, policyPrefix, nameof(customPolicy.XmlFile));
                        }
                    }
                }
            }

            return this.ValidationResults;
        }
    }
}
