﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ValidatePublishingResponse
    {
        public IEnumerable<string> Errors { get; set; }

        public IEnumerable<ApiValidationResult> Apis { get; set; }

        public IEnumerable<ProductValidationResult> Products { get; set; }

        public bool IsSuccess { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (this.Errors.Any())
            {
                foreach (var error in this.Errors)
                {
                    sb.AppendLine($"[Validate publishing error] {error}");
                }
            }

            foreach (var api in this.Apis)
            {
                sb.AppendLine(api.ToString());
                sb.AppendLine();
            }

            foreach (var product in this.Products)
            {
                sb.AppendLine(product.ToString());
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
