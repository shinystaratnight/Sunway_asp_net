namespace Web.Template.Application.Interfaces.Configuration
{
    using Web.Template.Application.Interfaces.Map;
    using Web.Template.Application.Interfaces.SocialMedia;

    /// <summary>
    ///  Site configuration wrapper interface
    /// </summary>
    public interface ISiteConfiguration
    {
        /// <summary>
        /// Gets or sets the booking journey configuration.
        /// </summary>
        /// <value>The booking journey configuration.</value>
        IBookingJourneyConfiguration BookingJourneyConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the date configuration.
        /// </summary>
        /// <value>
        /// The date configuration.
        /// </value>
        IDateConfiguration DateConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the flight configuration.
        /// </summary>
        /// <value>
        /// The flight configuration.
        /// </value>
        IFlightConfiguration FlightConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the map configuration.
        /// </summary>
        /// <value>The map configuration.</value>
        IMapConfiguration MapConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the origin URL.
        /// </summary>
        /// <value>The origin URL.</value>
        string OriginUrl { get; set; }

        /// <summary>
        /// Gets or sets the pricing configuration.
        /// </summary>
        /// <value>
        /// The pricing configuration.
        /// </value>
        IPricingConfiguration PricingConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the search configuration.
        /// </summary>
        /// <value>
        /// The search configuration.
        /// </value>
        ISearchConfiguration SearchConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the star rating configuration.
        /// </summary>
        /// <value>The star rating configuration.</value>
        IStarRatingConfiguration StarRatingConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the trade configuration.
        /// </summary>
        /// <value>The trade configuration.</value>
        ITradeConfiguration TradeConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the twitter configuration.
        /// </summary>
        /// <value>The twitter configuration.</value>
        ITwitterConfiguration TwitterConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the CMS configuration
        /// </summary>
        /// <value>The CMS configuration.</value>
        ICmsConfiguration CmsConfiguration { get; set; }
    }
}