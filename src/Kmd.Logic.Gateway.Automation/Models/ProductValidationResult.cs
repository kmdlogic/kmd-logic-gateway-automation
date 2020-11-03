using System;

namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ProductValidationResult : ItemValidationResultBase
    {
        public string Name { get; set; }

        public Guid? ProductId { get; set; }
    }
}
