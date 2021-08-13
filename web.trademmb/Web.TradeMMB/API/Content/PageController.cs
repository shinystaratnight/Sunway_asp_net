namespace Web.TradeMMB.API.Content
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

        /// <summary>
        /// Gets the page by URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Returns the page view model of given URL</returns>
        [Route("api/page/{*url}")]
        public PageViewModel GetPageByURL(string url)
        {
            if (string.IsNullOrEmpty(url) || url == "homepage")
            {
                url = "/";
            }

            if (url.Contains("results/"))
            {
                url = "results";
            }

            if (url.Contains("details/"))
            {
                url = "details";
            }

            if (url.Contains("extras/"))
            {
                url = "extras";
            }

            if (url.Contains("payment/"))
            {
                url = "payment";
            }

            if (url.Contains("confirmation/"))
            {
                url = "confirmation";
            }

            Page pageModel = this.pageService.GetPageByURL(url);

            List<Widget> widgets = pageModel.Sections.SelectMany(s => s.Widgets).ToList();

            return new PageViewModel()
            {
                EntityInformations = pageModel.EntityInformations,
                EntityType = pageModel.EntityType,
                PageURL = pageModel.Url,
                PageName = pageModel.Name,
                Widgets = widgets,
                MetaInformation = pageModel.MetaInformation
            };
        }
    }
}