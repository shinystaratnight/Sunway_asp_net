namespace Web.Template.Application.Support
{
    using System;
    using System.Web.Configuration;

    using Web.Template.Application.Net.IVectorConnect;

    /// <summary>
    /// Configure settings for the site
    /// </summary>
    /// <seealso cref="Web.Template.Application.Support.IConfiguration" />
    public class Configuration : IConfiguration
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Configuration" /> class.
        /// </summary>
        public Configuration()
        {
            this.UseIpRedirect = Convert.ToBoolean(WebConfigurationManager.AppSettings["UseIpRedirect"]);

            this.IpLookupServiceUrl = WebConfigurationManager.AppSettings["IpRedirectUrl"];
            this.DefaultCountryCode = WebConfigurationManager.AppSettings["DefaultCountryCode"];

            this.DocumentGeneratorUrl = WebConfigurationManager.AppSettings["DocumentGeneratorUrl"];
            if (string.IsNullOrEmpty(this.DocumentGeneratorUrl))
            {
                this.DocumentGeneratorUrl = "http://Docgen.ivector.co.uk";
            }

            this.SiteBuilderUrl = WebConfigurationManager.AppSettings["SiteBuilderUrl"];
            this.SetUserFromCookie = Convert.ToBoolean(WebConfigurationManager.AppSettings["SetUserFromCookie"]);
        }

        /// <summary>
        /// Gets the default country code.
        /// </summary>
        /// <value>
        /// The default country code.
        /// </value>
        public string DefaultCountryCode { get; }

        /// <summary>
        /// Gets the document generator URL.
        /// </summary>
        /// <value>
        /// The document generator URL.
        /// </value>
        public string DocumentGeneratorUrl { get; }

        /// <summary>
        /// Gets the ip redirect URL.
        /// </summary>
        /// <value>
        /// The ip redirect URL.
        /// </value>
        public string IpLookupServiceUrl { get; }

        /// <summary>
        /// Gets the connect handler.
        /// </summary>
        /// <value>
        /// The connect handler.
        /// </value>
        public string IVectorConnectHandler => "ivectorconnect.ashx";

        /// <summary>
        /// Gets a value indicating whether [set user from cookie].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [set user from cookie]; otherwise, <c>false</c>.
        /// </value>
        public bool SetUserFromCookie { get; }

        /// <summary>
        /// Gets the site builder URL.
        /// </summary>
        /// <value>The site builder URL.</value>
        public string SiteBuilderUrl { get; }

        /// <summary>
        /// Gets a value indicating whether [use ip redirect].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use ip redirect]; otherwise, <c>false</c>.
        /// </value>
        public bool UseIpRedirect { get; }
    }
}