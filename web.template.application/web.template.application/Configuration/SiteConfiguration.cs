namespace Web.Template.Application.Configuration
{
    
    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Map;
    using Web.Template.Application.Interfaces.SocialMedia;

    /// <summary>
    /// Site configuration wrapper class
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Configuration.ISiteConfiguration" />
    public class SiteConfiguration : ISiteConfiguration
    {
        /// <summary>
        /// Gets or sets the booking journey configuration.
        /// </summary>
        /// <value>The booking journey configuration.</value>
        public IBookingJourneyConfiguration BookingJourneyConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the cms configuration.
        /// </summary>
        /// <value>The cms configuration.</value>
        public ICmsConfiguration CmsConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the date configuration.
        /// </summary>
        /// <value>
        /// The date configuration.
        /// </value>
        public IDateConfiguration DateConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the flight configuration.
        /// </summary>
        /// <value>
        /// The flight configuration.
        /// </value>
        public IFlightConfiguration FlightConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the google map configuration.
        /// </summary>
        /// <value>The google map configuration.</value>
        public IMapConfiguration MapConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the origin URL.
        /// </summary>
        /// <value>The origin URL.</value>
        public string OriginUrl { get; set; }

        /// <summary>
        /// Gets or sets the pricing configuration.
        /// </summary>
        /// <value>
        /// The pricing configuration.
        /// </value>
        public IPricingConfiguration PricingConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the search configuration.
        /// </summary>
        /// <value>
        /// The search configuration.
        /// </value>
        public ISearchConfiguration SearchConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the star rating configuration.
        /// </summary>
        /// <value>The star rating configuration.</value>
        public IStarRatingConfiguration StarRatingConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the trade configuration.
        /// </summary>
        /// <value>The trade configuration.</value>
        public ITradeConfiguration TradeConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the twitter configuration.
        /// </summary>
        /// <value>The twitter configuration.</value>
        public ITwitterConfiguration TwitterConfiguration { get; set; }
    }
}