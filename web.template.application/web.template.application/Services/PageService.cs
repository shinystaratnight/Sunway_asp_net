namespace Web.Template.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using Intuitive;

    using SiteBuilder.Domain.Poco.Return;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Logging;
    using Web.Template.Application.Interfaces.Repositories;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.SiteBuilder;
    using Web.Template.Application.PageDefinition;
    using Web.Template.Application.Support;

    /// <summary>
    ///     Service used to retrieve pages from the site builder
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Services.IPageService" />
    public class PageService : IPageService
    {
        /// <summary>
        /// The log writer
        /// </summary>
        private readonly ILogWriter logWriter;

        /// <summary>
        ///     The page repository
        /// </summary>
        private readonly IPageRepository pageRepository;

        /// <summary>
        /// The site
        /// </summary>
        private readonly ISite site;

        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteBuilderService siteBuilderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageService" /> class.
        /// </summary>
        /// <param name="pageRepository">The page repository.</param>
        /// <param name="siteBuilderService">The site builder service.</param>
        /// <param name="logWriter">The log writer.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="siteService">The site service.</param>
        public PageService(IPageRepository pageRepository, ISiteBuilderService siteBuilderService, ILogWriter logWriter, IConfiguration configuration, ISiteService siteService)
        {
            this.pageRepository = pageRepository;
            this.siteBuilderService = siteBuilderService;
            this.logWriter = logWriter;
            this.site = siteService.GetSite(HttpContext.Current);
        }

        /// <summary>
        ///     Gets the page by URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        ///     A page
        /// </returns>
        public Page GetPageByURL(string url)
        {
            Page model = this.pageRepository.FindByUrl(url);
            model.MetaInformation = this.GetPageMeta(url);
            return model;
        }

        public List<Page> GetAll()
        {
            return this.pageRepository.FindAll();
        }

        /// <summary>
        /// Gets the page meta.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// an object containing the page meta information
        /// </returns>
        private PageMetaInformation GetPageMeta(string url)
        {
            var metaInfo = new PageMetaInformation();

            try
            {
                var pagepath = url.Replace("/", "-");
                if (pagepath == "-")
                {
                    pagepath = "homepage";
                }

                ContentDetailsReturn detailsReturn = this.siteBuilderService.GetModel(this.site.Name, "Page", pagepath, "Live");

                if (detailsReturn?.Content != null)
                {
                    metaInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<PageMetaInformation>(detailsReturn.Content);
                }

                if (url == "/")
                {
                    url = string.Empty;
                }

                metaInfo.CanonicalUrl = $"{this.site.Url}/{url}";
            }
            catch (Exception ex)
            {
                this.logWriter.Write("PageBuilderController", "Error in GetPageMeta", ex.ToString());
            }

            return metaInfo;
        }
    }
}