namespace Web.Template.Application.Site
{
    using Interfaces.Site;

    /// <summary>
    /// Redirect class used to represent a redirect from one page to another
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Site.IRedirect" />
    public class Redirect : IRedirect
    {
        /// <summary>
        /// Gets or sets the URL that we are redirecting from.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the URL that we are redirecting to
        /// </summary>
        /// <value>
        /// The redirect URL.
        /// </value>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets or sets the status code of the redirect.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the redirect identifier.
        /// </summary>
        /// <value>
        /// The redirect identifier.
        /// </value>
        public int RedirectId { get; set; }
    }
}