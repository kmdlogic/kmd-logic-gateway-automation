﻿using System.Collections.Generic;

namespace Kmd.Logic.Gateway.Automation.PublishFile
{
    public class Api
    {
        public string Name { get; set; }

        public IEnumerable<ApiVersion> ApiVersions { get; set; }

        public string Path { get; set; }
    }
}
