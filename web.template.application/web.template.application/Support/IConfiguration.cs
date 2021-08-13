namespace Web.Template.Application.Support
{
    using Web.Template.Application.Net.IVectorConnect;

    /// <summary>
    /// Class that manages application configuration.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Gets the default country code.
        /// </summary>
        /// <value>
        /// The default country code.
        /// </value>
        string DefaultCountryCode { get; }

        /// <summary>
        /// Gets the document generator URL.
        /// </summary>
        /// <value>
        /// The document generator URL.
        /// </value>
        string DocumentGeneratorUrl { get; }

        /// <summary>
        /// Gets the ip redirect URL.
        /// </summary>
        /// <value>
        /// The ip redirect URL.
        /// </value>
        string IpLookupServiceUrl { get; }

        /// <summary>
        /// Gets the i vector connect handler.
        /// </summary>
        /// <value>
        /// The i vector connect handler.
        /// </value>
        string IVectorConnectHandler { get; }

        /// <summary>
        /// Gets a value indicating whether [set user from cookie].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [set user from cookie]; otherwise, <c>false</c>.
        /// </value>
        bool SetUserFromCookie { get; }

        /// <summary>
        /// Gets the site builder URL.
        /// </summary>
        /// <value>The site builder URL.</value>
        string SiteBuilderUrl { get; }

        /// <summary>
        /// Gets a value indicating whether [use ip redirect].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use ip redirect]; otherwise, <c>false</c>.
        /// </value>
        bool UseIpRedirect { get; }
    }
}