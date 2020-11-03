using System;
using System.Collections.Generic;

namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ItemValidationResultBase
    {
        public ValidationStatus Status { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
}
