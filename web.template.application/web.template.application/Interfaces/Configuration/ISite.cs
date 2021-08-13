namespace Web.Template.Application.Interfaces.Configuration
{
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Entities.Payment;

    /// <summary>
    /// Interface defining what information we need about a site.
    /// </summary>
    public interface ISite
    {
        /// <summary>
        /// Gets or sets the brand identifier.
        /// </summary>
        /// <value>The brand identifier.</value>
        int BrandId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [customer specific page definition].
        /// </summary>
        /// <value><c>true</c> if [customer specific page definition]; otherwise, <c>false</c>.</value>
        bool CustomerSpecificPageDef { get; set; }

        /// <summary>
        /// Gets or sets the default currency code.
        /// </summary>
        /// <value>The default currency code.</value>
        string DefaultCurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the default language code.
        /// </summary>
        /// <value>The default language code.</value>
        string DefaultLanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the default page.
        /// </summary>
        /// <value>The default page.</value>
        string DefaultPage { get; set; }

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        /// <value>
        /// The environment.
        /// </value>
        string Environment { get; set; }

        /// <summary>
        /// Gets or sets the ivector connect base URL.
        /// </summary>
        /// <value>The ivector connect base URL.</value>
        string IvectorConnectBaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the ivector connect password.
        /// </summary>
        /// <value>The ivector connect password.</value>
        string IvectorConnectPassword { get; set; }

        /// <summary>
        /// Gets or sets the ivector connect username.
        /// </summary>
        /// <value>The ivector connect username.</value>
        string IvectorConnectUsername { get; set; }

        /// <summary>
        /// Gets or sets the ivector connect password.
        /// </summary>
        /// <value>The ivector connect password.</value>
        string IvectorConnectContentPassword { get; set; }

        /// <summary>
        /// Gets or sets the ivector connect username.
        /// </summary>
        /// <value>The ivector connect username.</value>
        string IvectorConnectContentUsername { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        string Url { get; set; }

        /// <summary>
        /// Gets or sets the site configuration.
        /// </summary>
        /// <value>
        /// The site configuration.
        /// </value>
        ISiteConfiguration SiteConfiguration { get; set; }
    }
}