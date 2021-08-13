namespace Web.Template.Application.PageDefinition
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Xml;

    using Intuitive;

    using Newtonsoft.Json;

    using SiteBuilder.Domain.Poco.Return;

    using Web.Template.Application.Helper;
    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Logging;
    using Web.Template.Application.Interfaces.PageDefinition;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.SiteBuilder;
    using Web.Template.Application.Support;

    /// <summary>
    /// Class responsible for configuring entity pages.
    /// </summary>
    /// <seealso cref="IEntityPageConfiguration" />
    public class EntityPageConfiguration : IEntityPageConfiguration
    {
        /// <summary>
        /// The content service
        /// </summary>
        private readonly IContentService contentService;

        /// <summary>
        /// The log writer
        /// </summary>
        private readonly ILogWriter logWriter;

        /// <summary>
        /// The offers service
        /// </summary>
        private readonly IOffersService offersService;

        /// <summary>
        /// The site builder service
        /// </summary>
        private readonly ISiteBuilderService siteBuilderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPageConfiguration" /> class.
        /// </summary>
        /// <param name="offersService">The offers service.</param>
        /// <param name="siteBuilderService">The site builder service.</param>
        /// <param name="contentService">The content service.</param>
        /// <param name="logWriter">The log writer.</param>
        public EntityPageConfiguration(IOffersService offersService, ISiteBuilderService siteBuilderService, IContentService contentService, ILogWriter logWriter)
        {
            this.offersService = offersService;
            this.siteBuilderService = siteBuilderService;
            this.contentService = contentService;
            this.logWriter = logWriter;
        }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <value>
        /// The cache key.
        /// </value>
        private string CacheKey => "entitypageconfig";

        /// <summary>
        /// Configures this instance.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns>A list of Entity Pages</returns>
        public List<EntityPage> Configure(string site)
        {
            var cachekey = Intuitive.AsyncCache.Controller<List<EntityPage>>.GenerateKey(this.CacheKey);
           List<EntityPage> entityPages = Intuitive.AsyncCache.Controller<List<EntityPage>>.GetCache(cachekey, 60, () => this.GenerateEntityPages(site));

            return entityPages;
        }

        /// <summary>
        /// URLs the safe string.
        /// </summary>
        /// <param name="stringToEncode">The string to encode.</param>
        /// <returns>The string you passed in, made lowercase with spaces replaced</returns>
        private static string UrlSafeString(string stringToEncode)
        {
            return stringToEncode.ToLower().Replace(" ", "-").Replace("'", string.Empty).Replace("*", string.Empty).Replace(",", string.Empty).Replace(".", string.Empty).Replace(":", string.Empty).Replace("&", "and").Replace("/", "or").Replace("é", "e");
        }

        private static string UrlUnsafeString(string stringToEncode)
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(stringToEncode.ToLower().Replace("-and", " &").Replace("-", " "));
        }

        /// <summary>
        /// Generates the custom entity pages.
        /// </summary>
        /// <param name="entityPages">The entity pages.</param>
        /// <param name="site">The site.</param>
        private void GenerateCustomEntityPages(List<EntityPage> entityPages, string site)
        {
            var locationBase = HttpContext.Current.Server.MapPath("\\Sites\\");
            var location = $"{locationBase}{site}\\customEntityPages.json";
            StreamReader r = new StreamReader(location);
            string json = r.ReadToEnd();
            List<CustomEntityPage> customEntityPages = JsonConvert.DeserializeObject<List<CustomEntityPage>>(json);

            foreach (CustomEntityPage customEntityPage in customEntityPages)
            {
                this.GenerateSitebuilderEntityPage(entityPages, site, customEntityPage.Entity, customEntityPage.UrlPrefix);
            }
        }

        /// <summary>
        /// Generates the entity pages.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns>A list of entity pages</returns>
        private List<EntityPage> GenerateEntityPages(string site)
        {
            var entityPages = new List<EntityPage>();

            try
            {
                XmlDocument contentXml = this.contentService.GetCmsxmlModel("EntityPages", 1);
                entityPages = XmlHelpers.XmlToGenericList<EntityPage>(contentXml);
                
                this.GenerateSpecialOfferPages(entityPages);
                this.GenerateCustomEntityPages(entityPages, site);
            }
            catch (Exception ex)
            {
                this.logWriter.Write("EntityPage Configuration", "error", ex.ToString());
            }

            return entityPages;
        }

        /// <summary>
        /// Generates the sitebuilder entity page.
        /// </summary>
        /// <param name="entityPages">The entity pages.</param>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="urlPrefix">The URL prefix.</param>
        private void GenerateSitebuilderEntityPage(List<EntityPage> entityPages, string site, string entity, string urlPrefix)
        {
            ContentReturn contentReturn = this.siteBuilderService.GetModels(site, entity);

            if (contentReturn != null)
            {
                foreach (ContentReturnContext context in contentReturn.Contexts)
                {
                    string entityInformationValue = urlPrefix;
                    var url = UrlSafeString(context.Name);
                    var value = UrlUnsafeString(context.Name);
                    if (urlPrefix != string.Empty)
                    {
                        url = UrlSafeString(urlPrefix) + "/" + url;
                        entityInformationValue = entityInformationValue.Capitalise();
                    }

                    var page = new EntityPage()
                                   {
                                        EntityType = entity.ToLower(),
                                        Url = url,
                                        EntityInformations = new List<PageEntityInformation>(),
                                        Title = value
                                   };

                    if (urlPrefix != string.Empty)
                    {
                        var baseEntityInformation = new PageEntityInformation() { Id = 0, Name = "sitebuilder-entity", Value = entityInformationValue, UrlSafeValue = UrlSafeString(entity) };
                        page.EntityInformations.Add(baseEntityInformation);
                    }

                    var entitySpecificInformation = new PageEntityInformation() { Id = 1, Name = entity.ToLower(), Value = value, UrlSafeValue = context.Name };
                    page.EntityInformations.Add(entitySpecificInformation);
                    entityPages.Add(page);
                }
            }
        }

        /// <summary>
        /// Generates the special offer pages.
        /// </summary>
        /// <param name="entityPages">The entity pages.</param>
        private void GenerateSpecialOfferPages(List<EntityPage> entityPages)
        {
            entityPages.AddRange(this.offersService.GetOfferPages());
        }
    }
}