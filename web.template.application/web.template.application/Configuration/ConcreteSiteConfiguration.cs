namespace Web.Template.Application.Configuration
{
    using Web.Template.Application.Maps;
    using Web.Template.Application.SocialMedia;

    /// <summary>
    /// Concrete version of the SiteConfiguration class so it can be serialized
    /// </summary>
    public class ConcreteSiteConfiguration
    {
        /// <summary>
        /// Gets or sets the booking journey configuration.
        /// </summary>
        /// <value>The booking journey configuration.</value>
        public ConcreteBookingJourneyConfiguration BookingJourneyConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the map configuration.
        /// </summary>
        /// <value>
        /// The map configuration.
        /// </value>
        public CmsConfiguration CmsConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the date configuration.
        /// </summary>
        /// <value>
        /// The date configuration.
        /// </value>
        public DateConfiguration DateConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the flight configuration.
        /// </summary>
        /// <value>
        /// The flight configuration.
        /// </value>
        public FlightConfiguration FlightConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the map configuration.
        /// </summary>
        /// <value>
        /// The map configuration.
        /// </value>
        public GoogleMapsConfiguration MapConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the pricing configuration.
        /// </summary>
        /// <value>
        /// The pricing configuration.
        /// </value>
        public PricingConfiguration PricingConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the search configuration.
        /// </summary>
        /// <value>
        /// The search configuration.
        /// </value>
        public SearchConfiguration SearchConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the twitter configuration.
        /// </summary>
        /// <value>
        /// The twitter configuration.
        /// </value>
        public TwitterConfiguration TwitterConfiguration { get; set; }
    }
}