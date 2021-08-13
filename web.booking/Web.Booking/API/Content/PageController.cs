using System.Xml;
using Intuitive;
using Intuitive.WebControls;

namespace Web.Booking.API.Content
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.PageDefinition;

    /// <summary>
    ///     The API to CMS Content to be called from a front end widget when additional content is needed
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class PageController : ApiController
    {
        /// <summary>
        /// The page service
        /// </summary>
        private readonly IPageService pageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageController"/> class.
        /// </summary>
        /// <param name="pageService">The page service.</param>
        public PageController(IPageService pageService)
        {
            this.pageService = pageService;
        }

        /// <summary>
        /// Gets the page by URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Returns the page view model of given URL</returns>
        [Route("api/page/{*url}")]
        public PageViewModel GetPageByURL(string url)
        {
            var pageUrl = url;

            if (url == "homepage")
            {
                pageUrl = "/";
            }

            if (url.Contains("results/"))
            {
                pageUrl = "results";
            }

            if (url.Contains("details/"))
            {
                pageUrl = "details";
            }

            if (url.Contains("extras/"))
            {
                pageUrl = "extras";
            }

            if (url.Contains("payment/"))
            {
                pageUrl = "payment";
            }

            if (url.Contains("conditions/"))
            {
                pageUrl = "conditions";
            }

            if (url.Contains("confirmation/"))
            {
                pageUrl = "confirmation";
            }

            if (url.Contains("quote/"))
            {
                pageUrl = "quote";
            }

            if (url.Contains("offsitepayment/"))
            {
                pageUrl = "offsitepayment";
            }

            Page pageModel = this.pageService.GetPageByURL(pageUrl);

            List<Widget> widgets = pageModel.Sections.SelectMany(s => s.Widgets).ToList();

            return new PageViewModel() { EntityInformations = pageModel.EntityInformations, EntityType = pageModel.EntityType, PageURL = pageModel.Url, PageName = pageModel.Name, Widgets = widgets, MetaInformation = pageModel.MetaInformation };
        }

        /// <summary>
        /// Gets all pages.
        /// </summary>
        /// <returns>List of pages.</returns>
        [Route("api/page/all")]
        public List<SiteSearchPageViewModel> GetAllPages()
        {
            var allPages = this.pageService.GetAll();
            var siteSearchPages = new List<SiteSearchPageViewModel>();

            foreach (var page in allPages)
            {
                var siteSearchPage = new SiteSearchPageViewModel()
                {
                    Title = page.Title,
                    Url = page.Url,
                    PageEntityInformations = page.EntityInformations
                };

                siteSearchPages.Add(siteSearchPage);
            }

            return siteSearchPages.Where(p => p.Title != null && p.Url != null).ToList();
        }
    }
}