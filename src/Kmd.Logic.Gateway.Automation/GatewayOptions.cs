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
        public Uri GatewayServiceUri { get; set; } = new Uri("https://kmd-logic-api-shareddev-webapp.azurewebsites.net");

        /// <summary>
        /// Gets or sets the Logic Subscription.
        /// </summary>t
        public Guid SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the Logic Provider Id.
        /// </summary>
        public Guid? ProviderId { get; set; }
    }
}