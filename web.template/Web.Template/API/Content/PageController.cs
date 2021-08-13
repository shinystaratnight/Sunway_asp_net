namespace Web.Template.API.Content
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
            if (url == "homepage")
            {
                url = "/";
            }

            Page pageModel = this.pageService.GetPageByURL(url);
            var name = System.Text.RegularExpressions.Regex.Replace(pageModel.Name, "([a-z])([A-Z])", "$1 $2");
	        List<Widget> widgets = pageModel.Sections.SelectMany(s => s.Widgets).ToList();

            return new PageViewModel() { EntityInformations = pageModel.EntityInformations, EntityType = pageModel.EntityType, PageURL = pageModel.Url, PageName = name, Widgets = widgets, MetaInformation = pageModel.MetaInformation};
        }

        [Route("api/page/all")]
        public List<SiteSearchPageViewModel> GetAllPages()
        {
            var allPages =  this.pageService.GetAll().Where(p => p.Published);
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