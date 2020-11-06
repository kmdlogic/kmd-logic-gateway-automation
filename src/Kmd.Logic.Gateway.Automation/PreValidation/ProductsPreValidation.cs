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

        public ValidationResult ValidateAsync(PublishFileModel publishFileModel)
        {
            if (publishFileModel != null)
            {
                var duplicateProducts = publishFileModel.Products.GroupBy(x => x.Name).Any(x => x.Count() > 1);
                if (duplicateProducts)
                {
                    this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"Duplicate product names exist" });
                }

                foreach (var product in publishFileModel.Products)
                {
                    if (string.IsNullOrEmpty(product.Name))
                    {
                        this.ValidationResults.Add(new PublishResult { IsError = true, ResultCode = ResultCode.InvalidInput, Message = $"[Product: {product.Name}] Product name does not exist" });
                    }

                    this.ValidateFile(FileType.Logo, product.Logo, product.Name, nameof(product.Logo));
                    this.ValidateFile(FileType.Document, product.Documentation, product.Name, nameof(product.Documentation));
                }
            }

            return new ValidationResult(this.ValidationResults);
        }
    }
}
