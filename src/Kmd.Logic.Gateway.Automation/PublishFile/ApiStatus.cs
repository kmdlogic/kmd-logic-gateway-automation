using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Gateway.Automation.PublishFile
{
    public enum ApiStatus
    {
        /// <summary>
        /// preview api
        /// </summary>
        Preview,

        /// <summary>
        /// Active Api
        /// </summary>
        Active,

        /// <summary>
        /// Deprecated Api
        /// </summary>
        Deprecated,
    }
}
