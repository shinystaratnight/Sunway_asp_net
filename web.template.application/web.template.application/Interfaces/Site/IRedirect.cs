namespace Web.Template.Application.Interfaces.Site
{
    /// <summary>
    ///     a model used to contain information about a redirect
    /// </summary>
    public interface IRedirect
    {
        /// <summary>
        ///     Gets or sets the URL.
        /// </summary>
        /// <value>
        ///     The URL.
        /// </value>
        string Url { get; set; }

        /// <summary>
        ///     Gets or sets the redirect URL.
        /// </summary>
        /// <value>
        ///     The redirect URL.
        /// </value>
        string RedirectUrl { get; set; }

        /// <summary>
        ///     Gets or sets the status code.
        /// </summary>
        /// <value>
        ///     The status code.
        /// </value>
        int StatusCode { get; set; }

        /// <summary>
        ///     Gets or sets the redirect identifier.
        /// </summary>
        /// <value>
        ///     The redirect identifier.
        /// </value>
        int RedirectId { get; set; }
    }
}