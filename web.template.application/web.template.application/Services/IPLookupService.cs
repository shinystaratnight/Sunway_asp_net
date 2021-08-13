namespace Web.Template.Application.Services
{
    using System;
    using System.Web;
    using System.Xml;

    using Intuitive;

    using Web.Template.Application.Interfaces.Logging;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Support;

    /// <summary>
    /// Service to look up the users country code from their IP.
    /// </summary>
    /// <seealso cref="IIpLookupService" />
    public class IpLookupService : IIpLookupService
    {
        /// <summary>
        /// The configuration
        /// </summary>
        private IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="IpLookupService"/> class.
        /// </summary>
        /// <param name="logWriter">The log writer.</param>
        /// <param name="configuration">The configuration.</param>
        public IpLookupService(ILogWriter logWriter, IConfiguration configuration)
        {
            this.LogWriter = logWriter;
            this.configuration = configuration;
        }

        /// <summary>
        /// Gets the client IP address.
        /// </summary>
        /// <value>
        /// The client IP address.
        /// </value>
        private string ClientIpAddress
        {
            get
            {
#if DEBUG
                return "82.108.7.146";
#endif
                return HttpContext.Current.Request.UserHostAddress;
            }
        }

        /// <summary>
        /// Gets the log writer.
        /// </summary>
        /// <value>
        /// The log writer.
        /// </value>
        private ILogWriter LogWriter { get; }

        /// <summary>
        /// Gets the client country code.
        /// </summary>
        /// <returns>
        /// a country code that the user is from
        /// </returns>
        public string GetClientCountryCode()
        {
            var clientCountryCode = string.Empty;
            var ipAddress = this.ClientIpAddress;

            try
            {
                var ipLookupResponse = Intuitive.Net.WebRequests.GetResponse($"{this.configuration.IpLookupServiceUrl}?ip={ipAddress}", string.Empty);
                XmlDocument ipLookupResponseXml = new XmlDocument();
                ipLookupResponseXml.LoadXml(ipLookupResponse);

                if (Convert.ToBoolean(XMLFunctions.SafeNodeValue(ipLookupResponseXml, "//ReturnStatus/Success")))
                {
                    clientCountryCode = XMLFunctions.SafeNodeValue(ipLookupResponseXml, "//IPAddressResponse/GeoIPLocation/CountryCode");
                }
            }
            catch (Exception ex)
            {
                this.LogWriter.Write("IPLookup", "Error", ex.ToString());
            }

            return clientCountryCode;
        }
    }
}