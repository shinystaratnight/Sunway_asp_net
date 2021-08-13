namespace Web.Template.Application.Configuration
{
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Entities.Payment;

    /// <summary>
    /// Base configuration for the site that can be serialized into
    /// </summary>
    public class SiteBaseConfiguration
    {
        /// <summary>
        /// Gets or sets the brand identifier.
        /// </summary>
        /// <value>
        /// The brand identifier.
        /// </value>
        public int BrandId { get; set; }

        /// <summary>
        /// Gets or sets the default currency.
        /// </summary>
        /// <value>
        /// The default currency.
        /// </value>
        public Currency DefaultCurrency { get; set; }

        /// <summary>
        /// Gets or sets the default language.
        /// </summary>
        /// <value>
        /// The default language.
        /// </value>
        public Language DefaultLanguage { get; set; }

        /// <summary>
        /// Gets or sets the default page.
        /// </summary>
        /// <value>The default page.</value>
        public string DefaultPage { get; set; }

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        /// <value>
        /// The environment.
        /// </value>
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }
}