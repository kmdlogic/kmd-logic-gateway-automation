using System;

namespace Kmd.Logic.Gateway.Automation
{
    /// <summary>
    /// Provide the configuration options for using the Gateway service.
    /// </summary>
    public sealed class GatewayOptions
    {
        /// <summary>
        /// Gets or sets the Logic Gateway service.
        /// </summary>
        /// <remarks>
        /// This option should not be overridden except for testing purposes.
        /// </remarks>
        public Uri GatewayServiceUri { get; set; } = new Uri("https://gateway.kmdlogic.io/gateway/v1");

        /// <summary>
        /// Gets or sets the Logic Subscription.
        /// </summary>
        public Guid SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the Logic Provider Id.
        /// </summary>
        public Guid? ProviderId { get; set; }
    }
}