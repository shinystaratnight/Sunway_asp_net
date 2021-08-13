namespace Web.Template.Domain.Entities.Site
{
    /// <summary>
    /// class representing a website
    /// </summary>
    public class CmsWebsite
    {
        /// <summary>
        /// Gets or sets the content suffix.
        /// </summary>
        /// <value>
        /// The content suffix.
        /// </value>
        public string ContentSuffix { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the currency identifier.
        /// </summary>
        /// <value>
        /// The currency identifier.
        /// </value>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CmsWebsite"/> is default.
        /// </summary>
        /// <value>
        ///   <c>true</c> if default; otherwise, <c>false</c>.
        /// </value>
        public bool Default { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }
}