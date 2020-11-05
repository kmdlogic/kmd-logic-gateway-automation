﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kmd.Logic.Gateway.Automation.Models
{
    public class ItemValidationResultBase
    {
        public ValidationStatus Status { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (this.Errors.Any())
            {
                sb.AppendLine("Errors:");
                foreach (var error in this.Errors)
                {
                    sb.AppendLine($"\t- {error}");
                }
            }
            else
            {
                sb.AppendLine($"* Status: {this.Status}");
            }

            return sb.ToString();
        }
    }
}
