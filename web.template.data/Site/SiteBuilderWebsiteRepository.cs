namespace Web.Template.Data.Site
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using SiteBuilder.Domain.Poco.Return;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.SiteBuilder;
    using Web.Template.Domain.Entities.Site;

    /// <summary>
    /// Website repository responsible for giving access to websites, uses site builder
    /// </summary>
    /// <seealso cref="Web.Template.Data.Site.IWebsiteRepository" />
    public class WebsiteRepository : IWebsiteRepository
    {
        /// <summary>
        /// The cache lock object
        /// </summary>
        private static readonly object CacheLockObject = new object();

        /// <summary>
        /// The site builder service
        /// </summary>
        private readonly ISiteBuilderService siteBuilderService;

        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteRepository" /> class.
        /// </summary>
        /// <param name="siteBuilderService">The site builder service.</param>
        /// <param name="siteService">The site service.</param>
        public WebsiteRepository(ISiteBuilderService siteBuilderService, ISiteService siteService)
        {
            this.siteBuilderService = siteBuilderService;
            this.siteService = siteService;
            this.Collection = this.Setup();
        }

        /// <summary>
        /// Gets the cachekey.
        /// </summary>
        /// <value>
        /// The cachekey.
        /// </value>
        private string Cachekey => "websiteRepository";

        /// <summary>
        /// Gets or sets the collection.
        /// </summary>
        /// <value>
        /// The collection.
        /// </value>
        private List<CmsWebsite> Collection { get; set; }

        /// <summary>
        /// Gets all websites.
        /// </summary>
        /// <returns>a list of all websites</returns>
        public List<CmsWebsite> GetAll()
        {
            return this.Collection;
        }

        /// <summary>
        /// Gets the default site.
        /// </summary>
        /// <returns></returns>
        public CmsWebsite GetDefaultSite()
        {
            return this.Collection.FirstOrDefault(w => w.Default);
        }

        /// <summary>
        /// Gets the website by country code.
        /// </summary>
        /// <param name="countryCode">The country code.</param>
        /// <returns>a single website that matches the code provided</returns>
        public CmsWebsite GetWebsiteByCountryCode(string countryCode)
        {
            return this.Collection.FirstOrDefault(w => w.CountryCode.ToLower() == countryCode.ToLower());
        }

        /// <summary>
        /// Gets the website which matches the name passed in.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A single website that matches the name passed in</returns>
        public CmsWebsite GetWebsiteByName(string name)
        {
            return this.Collection.FirstOrDefault(w => w.Name.ToLower() == name.ToLower());
        }

        /// <summary>
        /// Retrieves from site builder.
        /// </summary>
        /// <returns>A list of websites retrieved from the siteBuilder</returns>
        private List<CmsWebsite> RetrieveFromSiteBuilder()
        {
            ISite site = this.siteService.GetSite(HttpContext.Current);
            ContentReturn models = this.siteBuilderService.GetModels(site.Name, "CmsWebsite");

            var websites = new List<CmsWebsite>();

            foreach (var model in models.Contexts)
            {
                try
                {
                    var cmsWebsite = Newtonsoft.Json.JsonConvert.DeserializeObject<CmsWebsite>(model.ContentValue);
                    websites.Add(cmsWebsite);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return websites;
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of Booking documentation</returns>
        private List<CmsWebsite> Setup()
        {
            var websites = HttpRuntime.Cache[this.Cachekey] as List<CmsWebsite>;

            if (websites == null)
            {
                lock (CacheLockObject)
                {
                    websites = HttpRuntime.Cache[this.Cachekey] as List<CmsWebsite>;
                    if (websites == null)
                    {
                        websites = this.RetrieveFromSiteBuilder();
                        HttpRuntime.Cache.Insert(
                            this.Cachekey, 
                            websites, 
                            null, 
                            DateTime.Now.AddHours(12), 
                            TimeSpan.Zero);
                    }
                }
            }

            return websites;
        }
    }
}