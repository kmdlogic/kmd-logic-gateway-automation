using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Gateway.Automation.Tool
{
    public enum OutputFormat
    {
        /// <summary>
        /// Default format for displaying the results.
        /// Results are displayed as a plain list.
        /// </summary>
        List,

        /// <summary>
        /// Results are displayed in json format.
        /// </summary>
        Json,
    }
}
