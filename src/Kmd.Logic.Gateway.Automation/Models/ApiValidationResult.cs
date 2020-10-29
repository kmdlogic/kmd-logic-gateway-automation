using System;
using System.Collections.Generic;

namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ApiValidationResult : ItemValidationResultBase
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string Version { get; set; }

        public Guid? ApiId { get; set; }

        public IEnumerable<ApiRevisionValidationResult> Revisions { get; set; }
    }
}
