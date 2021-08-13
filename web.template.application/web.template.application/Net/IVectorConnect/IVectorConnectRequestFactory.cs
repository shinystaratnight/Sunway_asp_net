namespace Web.Template.Application.Net.IVectorConnect
{
    using System.Threading;
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Logging;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Support;

    /// <summary>
    ///     Creates instances of iVectorConnect request classes
    /// </summary>
    public class IVectorConnectRequestFactory : IIVectorConnectRequestFactory
    {
        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        ///     The web request logger
        /// </summary>
        private readonly IWebRequestLogger webRequestLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IVectorConnectRequestFactory" /> class.
        /// </summary>
        /// <param name="webRequestLogger">The web request logger.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="siteService">The site service.</param>
        public IVectorConnectRequestFactory(IWebRequestLogger webRequestLogger, IConfiguration configuration, ISiteService siteService)
        {
            this.webRequestLogger = webRequestLogger;
            this.configuration = configuration;
            this.siteService = siteService;
        }

        /// <summary>
        /// Creates an instance of an IVectorConnectRequest.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="context">The context.</param>
        /// <returns>An instance of an IVectorConnectRequest class</returns>
        public IIVectorConnectRequest Create(iVectorConnectRequest request, HttpContext context)
        {
           
            return new IVectorConnectRequest(this.configuration.IVectorConnectHandler, "Post", request, FormatType.XML, this.webRequestLogger, this.siteService, context);
        }

        /// <summary>
        /// Creates an instance of an IVectorConnectRequest.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="url">The URL.</param>
        /// <param name="formatType">Type of the format.</param>
        /// <param name="context">The context.</param>
        /// <returns>An instance of an IVectorConnectRequest class</returns>
        public IIVectorConnectRequest Create(iVectorConnectRequest request, string url, FormatType formatType, HttpContext context)
        {
            return new IVectorConnectRequest(url, "Post", request, formatType, this.webRequestLogger, this.siteService, context);
        }

        /// <summary>
        /// Creates an instance of an IVectorConnectRequest.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        /// An instance of an IVectorConnectRequest class
        /// </returns>
        public IIVectorConnectRequest Create(iVectorConnectRequest request, CancellationToken token)
        {
            return new IVectorConnectRequest(this.configuration.IVectorConnectHandler, "Post", request, FormatType.XML, this.webRequestLogger, this.siteService, token);
        }
    }
}