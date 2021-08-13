namespace Web.Template.Application.Tracking
{
    using Web.Template.Application.Interfaces.Tracking;

    /// <summary>
    /// Sets the Tracking Affiliate Service values
    /// </summary>
    public class TrackingAffiliate : ITrackingAffiliate
    {
        /// <summary>
        /// Gets or sets the accommodation token override.
        /// </summary>
        /// <value>
        /// The accommodation token override.
        /// </value>
        public string AccomTokenOverride { get; set; }

        /// <summary>
        /// Gets or sets the type of the booking.
        /// </summary>
        /// <value>
        /// The type of the booking.
        /// </value>
        public string BookingType { get; set; }

        /// <summary>
        /// Gets or sets the booking type i ds.
        /// </summary>
        /// <value>
        /// The booking type i ds.
        /// </value>
        public string BookingTypeIDs { get; set; }

        /// <summary>
        /// Gets or sets the brand identifier.
        /// </summary>
        /// <value>
        /// The brand identifier.
        /// </value>
        public int BrandId { get; set; }

        /// <summary>
        /// Gets or sets the CMS website identifier.
        /// </summary>
        /// <value>
        /// The CMS website identifier.
        /// </value>
        public int CmsWebsiteId { get; set; }

        /// <summary>
        /// Gets or sets the confirmation script.
        /// </summary>
        /// <value>
        /// The confirmation script.
        /// </value>
        public string ConfirmationScript { get; set; }

        /// <summary>
        /// Gets or sets the flight and accommodation token override.
        /// </summary>
        /// <value>
        /// The flight and accommodation token override.
        /// </value>
        public string FlightAndAccomTokenOverride { get; set; }

        /// <summary>
        /// Gets or sets the flight token override.
        /// </summary>
        /// <value>
        /// The flight token override.
        /// </value>
        public string FlightTokenOverride { get; set; }

        /// <summary>
        /// Gets or sets the landing page script.
        /// </summary>
        /// <value>
        /// The landing page script.
        /// </value>
        public string LandingPageScript { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the pages.
        /// </summary>
        /// <value>
        /// The pages.
        /// </value>
        public string Pages { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public string Position { get; set; }

        /// <summary>
        /// Gets or sets the query string identifier.
        /// </summary>
        /// <value>
        /// The query string identifier.
        /// </value>
        public string QueryStringIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the script.
        /// </summary>
        /// <value>
        /// The script.
        /// </value>
        public string Script { get; set; }

        /// <summary>
        /// Gets or sets the secure.
        /// </summary>
        /// <value>
        /// The secure.
        /// </value>
        public int Secure { get; set; }

        /// <summary>
        /// Gets or sets the secure script.
        /// </summary>
        /// <value>
        /// The secure script.
        /// </value>
        public string SecureScript { get; set; }

        /// <summary>
        /// Gets or sets the tracking affiliate identifier.
        /// </summary>
        /// <value>
        /// The tracking affiliate identifier.
        /// </value>
        public int TrackingAffiliateId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets how many days the token is valid.
        /// </summary>
        /// <value>
        /// The number of days the token will be valid for days.
        /// </value>
        public int ValidForDays { get; set; } = 0;
    }
}