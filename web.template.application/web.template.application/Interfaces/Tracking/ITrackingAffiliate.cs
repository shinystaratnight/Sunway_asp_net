namespace Web.Template.Application.Interfaces.Tracking
{
    /// <summary>
    /// Tracking Affiliate Interface
    /// </summary>
    public interface ITrackingAffiliate
    {
        /// <summary>
        /// Gets or sets the accommodation token override.
        /// </summary>
        /// <value>
        /// The accommodation token override.
        /// </value>
        string AccomTokenOverride { get; set; }

        /// <summary>
        /// Gets or sets the type of the booking.
        /// </summary>
        /// <value>
        /// The type of the booking.
        /// </value>
        string BookingType { get; set; }

        /// <summary>
        /// Gets or sets the booking type i ds.
        /// </summary>
        /// <value>
        /// The booking type i ds.
        /// </value>
        string BookingTypeIDs { get; set; }

        /// <summary>
        /// Gets or sets the brand identifier.
        /// </summary>
        /// <value>
        /// The brand identifier.
        /// </value>
        int BrandId { get; set; }

        /// <summary>
        /// Gets or sets the CMS website identifier.
        /// </summary>
        /// <value>
        /// The CMS website identifier.
        /// </value>
        int CmsWebsiteId { get; set; }

        /// <summary>
        /// Gets or sets the confirmation script.
        /// </summary>
        /// <value>
        /// The confirmation script.
        /// </value>
        string ConfirmationScript { get; set; }

        /// <summary>
        /// Gets or sets the flight and accommodation token override.
        /// </summary>
        /// <value>
        /// The flight and accommodation token override.
        /// </value>
        string FlightAndAccomTokenOverride { get; set; }

        /// <summary>
        /// Gets or sets the flight token override.
        /// </summary>
        /// <value>
        /// The flight token override.
        /// </value>
        string FlightTokenOverride { get; set; }

        /// <summary>
        /// Gets or sets the landing page script.
        /// </summary>
        /// <value>
        /// The landing page script.
        /// </value>
        string LandingPageScript { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the pages.
        /// </summary>
        /// <value>
        /// The pages.
        /// </value>
        string Pages { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        string Position { get; set; }

        /// <summary>
        /// Gets or sets the query string identifier.
        /// </summary>
        /// <value>
        /// The query string identifier.
        /// </value>
        string QueryStringIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the script.
        /// </summary>
        /// <value>
        /// The script.
        /// </value>
        string Script { get; set; }

        /// <summary>
        /// Gets or sets the secure.
        /// </summary>
        /// <value>
        /// The secure.
        /// </value>
        int Secure { get; set; }

        /// <summary>
        /// Gets or sets the secure script.
        /// </summary>
        /// <value>
        /// The secure script.
        /// </value>
        string SecureScript { get; set; }

        /// <summary>
        /// Gets or sets the tracking affiliate identifier.
        /// </summary>
        /// <value>
        /// The tracking affiliate identifier.
        /// </value>
        int TrackingAffiliateId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        string Type { get; set; }

        /// <summary>
        /// Gets or sets how many days the token is valid.
        /// </summary>
        /// <value>
        /// The number of days the token will be valid for days.
        /// </value>
        int ValidForDays { get; set; }
    }
}