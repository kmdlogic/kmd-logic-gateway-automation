using System;

namespace Kmd.Logic.Cpr.Client
{
    /// <summary>
    /// Provide the configuration options for using the CPR service.
    /// </summary>
    public sealed class GatewayOptions
    {
        /// <summary>
        /// Gets or sets the Logic CPR service.
        /// </summary>
        /// <remarks>
        /// This option should not be overridden except for testing purposes.
        /// </remarks>
        public Uri GatewayServiceUri { get; set; } = new Uri("https://gateway.kmdlogic.io/cpr/v1");

        /// <summary>
        /// Gets or sets the Logic Subscription.
        /// </summary>
        public Guid SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the Logic CPR Configuration identifier.
        /// </summary>
        public Guid ProviderId { get; set; }
    }
}