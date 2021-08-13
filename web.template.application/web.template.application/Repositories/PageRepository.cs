namespace Web.Template.Application.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;

    using Newtonsoft.Json;

    using Web.Template.Application.Exceptions;
    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.PageDefinition;
    using Web.Template.Application.Interfaces.Repositories;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.PageDefinition;

    /// <summary>
    /// A concrete implementation of IPageRepository used to retrieve template pages by URL
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Repositories.IPageRepository" />
    public class PageRepository : IPageRepository
    {
        /// <summary>
        /// The entity page configuration
        /// </summary>
        private readonly IEntityPageConfiguration entityPageConfiguration;

        /// <summary>
        /// A list of pages linked to entities e.g. geographies or product attribute
        /// </summary>
        private readonly List<EntityPage> entityPages;

        /// <summary>
        /// The pages
        /// </summary>
        private readonly List<Page> pages;

        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageRepository" /> class.
        /// </summary>
        /// <param name="entityPageConfiguration">The entity page configuration.</param>
        /// <param name="siteService">The site service.</param>
        public PageRepository(IEntityPageConfiguration entityPageConfiguration, ISiteService siteService)
        {
            this.entityPageConfiguration = entityPageConfiguration;
            this.siteService = siteService;

            ISite site = this.siteService.GetSite(HttpContext.Current);
            this.entityPages = this.entityPageConfiguration.Configure(site.Name);

            var path = HttpContext.Current.Server.MapPath("~/pagedef");
            if (site.CustomerSpecificPageDef)
            {
                var siteName = site.Name.ToLower();
                path = $"{path}/{siteName}";
            }

            this.pages = this.GetPages(path);

            if (!site.CustomerSpecificPageDef)
            {
                this.GetOverridePages(site, path);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageRepository" /> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="entityPageConfiguration">The entity page configuration.</param>
        /// <param name="siteService">The site service.</param>
        public PageRepository(string path, IEntityPageConfiguration entityPageConfiguration, ISiteService siteService)
        {
            this.entityPageConfiguration = entityPageConfiguration;
            this.siteService = siteService;

            ISite site = this.siteService.GetSite(HttpContext.Current);
            this.entityPages = this.entityPageConfiguration.Configure(site.Name);

            string[] filePaths = Directory.GetFiles(path);

            this.pages = new List<Page>();
            foreach (var file in filePaths)
            {
                string json;
                using (var r = new StreamReader(file))
                {
                    json = r.ReadToEnd();
                }

                var page = JsonConvert.DeserializeObject<Page>(json);
                this.pages.Add(page);
            }
        }

        /// <summary>
        /// Finds all pages
        /// </summary>
        /// <returns>
        /// A list of all Pages in the repo
        /// </returns>
        public List<Page> FindAll()
        {
            var allPages = new List<Page>();
            allPages.AddRange(this.pages);
            if (this.entityPages != null)
            {
                allPages.AddRange(this.entityPages.Select(entityPage => new Page()
                                                                            {
                                                                                Url = entityPage.Url,
                                                                                Title = entityPage.Title,
                                                                                EntityType = entityPage.EntityType,
                                                                                EntityInformations = entityPage.EntityInformations,
                                                                                Published = entityPage.Published
                                                                            }));
            }

            return allPages;
        }

        /// <summary>
        /// Finds the by URL.
        /// </summary>
        /// <param name="url">The URL that you want the page for</param>
        /// <returns>
        /// A Page
        /// </returns>
        /// <exception cref="PageNotFoundException">Page Not present in the page repository.</exception>
        /// <exception cref="System.Exception">Page Not found</exception>
        public Page FindByUrl(string url)
        {
            IEnumerable<Page> enumerable;

            IEnumerable<Page> matchedPages = from s in this.pages where s.Url == url select s;

            enumerable = matchedPages as IList<Page> ?? matchedPages.ToList();
            if (!enumerable.Any())
            {
                IEnumerable<EntityPage> matchedEntityPages = this.entityPages?.Where(x => string.Equals(x.Url, url, StringComparison.InvariantCultureIgnoreCase));

                if (matchedEntityPages != null && matchedEntityPages.Any())
                {
                    matchedPages = this.pages.Where(x => matchedEntityPages.Any(y => string.Equals(y.EntityType, x.EntityType, StringComparison.InvariantCultureIgnoreCase)));

                    if (matchedPages.Any())
                    {
                        foreach (Page matchedPage in matchedPages)
                        {
                            matchedPage.EntityInformations = matchedEntityPages.FirstOrDefault().EntityInformations;
                        }
                    }
                }

                enumerable = matchedPages as IList<Page> ?? matchedPages.ToList();

                if (!enumerable.Any())
                {
                    return this.pages.FirstOrDefault(p => p.Name == "404");
                }
            }

            return enumerable.FirstOrDefault();
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// An object of type T
        /// </returns>
        /// <exception cref="System.NotImplementedException">Not implemented yet</exception>
        public Page Get(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the override pages.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="path">The path of the pages.</param>
        private void GetOverridePages(ISite site, string path)
        {
            var siteName = site.Name.ToLower();
            var overridePath = $"{path}/{siteName}";

            List<Page> customPages = this.GetPages(overridePath);

            foreach (var page in customPages)
            {
                var index = this.pages.FindIndex(p => p.Name.Equals(page.Name, StringComparison.Ordinal));
                if (index != -1)
                {
                    this.pages[index] = page;
                }
                else
                {
                    this.pages.Add(page);
                }
            }
        }

        /// <summary>
        /// Gets the pages.
        /// </summary>
        /// <param name="path">The path where the pages are located.</param>
        /// <returns>The list of pages</returns>
        private List<Page> GetPages(string path)
        {
            var pageList = new List<Page>();

            if (Directory.Exists(path))
            {
                string[] filePaths = Directory.GetFiles(path);

                foreach (var file in filePaths)
                {
                    string json;
                    using (var r = new StreamReader(file))
                    {
                        json = r.ReadToEnd();
                    }

                    var page = JsonConvert.DeserializeObject<Page>(json);
                    pageList.Add(page);
                }
            }
            return pageList;
        }
    }
}