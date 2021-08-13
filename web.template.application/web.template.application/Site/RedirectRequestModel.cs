namespace Web.Template.Application.Site
{
    /// <summary>
    ///  model passed into the controller for redirect changes
    /// </summary>
    public class RedirectRequestModel
    {
        /// <summary>
        /// Gets or sets the url to redirect from.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the url to redirect to.
        /// </summary>
        /// <value>
        /// The redirect URL.
        /// </value>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets or sets the url to redirect to.
        /// </summary>
        /// <value>
        /// The redirect URL.
        /// </value>
        public int RedirectId { get; set; }

        /// <summary>
        /// Gets or sets the name of the site.
        /// </summary>
        /// <value>
        /// The name of the site.
        /// </value>
        public string SiteName { get; set; }
    }
}