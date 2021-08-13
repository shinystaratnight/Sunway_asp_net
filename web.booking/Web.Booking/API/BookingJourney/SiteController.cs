namespace Web.Booking.API.BookingJourney
{
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Http;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Data.Site;
    using Web.Template.Domain.Entities.Site;

    /// <summary>
    ///     Controller used to expose to the user interface operations concerning users such as logging in
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class SiteController : ApiController
    {
        /// <summary>
        /// The user service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// The website repository
        /// </summary>
        private readonly IWebsiteRepository websiteRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteController" /> class.
        /// </summary>
        /// <param name="siteService">The site service.</param>
        /// <param name="websiteRepository">The website repository.</param>
        public SiteController(ISiteService siteService, IWebsiteRepository websiteRepository)
        {
            this.siteService = siteService;
            this.websiteRepository = websiteRepository;
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <returns>The User Session.</returns>
        [Route("api/site")]
        public ISite GetSite()
        {
            return this.siteService.GetSite(HttpContext.Current);
        }

        [Route("api/websites")]
        [HttpGet]
        public List<CmsWebsite> Getwebsites()
        {
            return this.websiteRepository.GetAll();
        }

    }
}