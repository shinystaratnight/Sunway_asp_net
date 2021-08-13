namespace Web.Template.Application.Services
{
    using System;
    using System.Web;

    using Web.Template.Application.Configuration;
    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.SiteBuilderService;
    using Web.Template.Application.Support;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;

    /// <summary>
    /// Class SiteService.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Services.ISiteService" />
    public class SiteService : ISiteService
    {
        /// <summary>
        /// The cache lock object
        /// </summary>
        private static readonly object CacheLockObject = new object();

        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// The site builder request
        /// </summary>
        private readonly ISiteBuilderRequest siteBuilderRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteService" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="siteBuilderRequest">The site builder request.</param>
        public SiteService(
            IConfiguration configuration,
            ISiteBuilderRequest siteBuilderRequest)
        {
            this.configuration = configuration;
            this.siteBuilderRequest = siteBuilderRequest;
        }

        /// <summary>
        /// Gets the site.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The site</returns>
        public ISite GetSite(HttpContext context)
        {
            var request = context.Request;
            string baseUrl = $"{request.Url.Scheme}://{request.Url.Authority}{request.ApplicationPath?.TrimEnd('/')}/";

            var site = HttpRuntime.Cache[baseUrl] as Site;
            if (site == null)
            {
                lock (CacheLockObject)
                {
                    site = HttpRuntime.Cache[baseUrl] as Site
                           ?? new Site(this.configuration, this.siteBuilderRequest, context);
                    HttpRuntime.Cache.Insert(baseUrl, site, null, DateTime.Now.AddHours(12), TimeSpan.Zero);
                }
            }

            return site;
        }
    }
}