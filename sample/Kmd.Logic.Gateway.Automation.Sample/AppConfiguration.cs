using System;
using Kmd.Logic.Identity.Authorization;

namespace Kmd.Logic.Cpr.Client.Sample
{
    internal class AppConfiguration
    {
        public LogicTokenProviderOptions TokenProvider { get; set; } = new LogicTokenProviderOptions();

        public GatewayOptions Gateway { get; set; } = new GatewayOptions();
    }
}