namespace Web.Template.Application.Services
{
    using System.Web;
    using System.Xml;

    using SiteBuilder.Domain.Poco.Return;

    using Web.Template.Application.Content;
    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.SiteBuilder;
    using Web.Template.Application.Interfaces.User;

    /// <summary>
    /// Content service used to return content to the front end
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Services.IContentService" />
    public class ContentService : IContentService
    {
        /// <summary>
        /// The site
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// The site builder service
        /// </summary>
        private readonly ISiteBuilderService siteBuilderService;

        /// <summary>
        /// The user service
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentService" /> class.
        /// </summary>
        /// <param name="contentModelFactory">The content model factory.</param>
        /// <param name="siteBuilderService">The site builder service.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="siteService">The site service.</param>
        public ContentService(IContentModelFactory contentModelFactory, ISiteBuilderService siteBuilderService,
            IUserService userService, ISiteService siteService)
        {
            this.siteBuilderService = siteBuilderService;
            this.userService = userService;
            this.siteService = siteService;
        }

        /// <summary>
        /// Gets the type of the CMS content for object.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// a content model.
        /// </returns>
        public IContentModel GetCMSContentForObjectType(string objectType, int id)
        {
            IContentModel contentModel = new ContentModel();

            IUserSession userSession = this.userService.GetUser(HttpContext.Current);
            var cacheKey = $"{objectType}|{id}|{userSession.SelectedLanguage.CultureCode}".ToLower();

            if (HttpContext.Current.Cache[cacheKey] != null)
            {
                contentModel.ContentJSON = HttpContext.Current.Cache[cacheKey].ToString();
                contentModel.Success = true;
            }
            else
            {
                string content = this.GetCMSJsonModel(objectType, id);
                if (!string.IsNullOrEmpty(content))
                {
                    HttpContext.Current.Cache.Insert(cacheKey, content);
                    contentModel.ContentJSON = content;
                    contentModel.Success = true;
                }
            }

            return contentModel;
        }

        /// <summary>
        /// Gets the CMS XML model.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// an xml document
        /// </returns>
        public XmlDocument GetCmsxmlModel(string objectType, int id)
        {
            ISite site = this.siteService.GetSite(HttpContext.Current);
            string url = $"{site.IvectorConnectBaseUrl}cms/{objectType}/{id}";

            XmlDocument returnXML = new XmlDocument();
            if (HttpContext.Current.Cache[url] != null)
            {
                returnXML = (XmlDocument)HttpContext.Current.Cache[url];
            }
            else
            {
                returnXML = this.SendWebRequest(url);
                HttpContext.Current.Cache.Insert(url, returnXML, null, System.DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }

            return returnXML;
        }

        /// <summary>
        /// Gets the content associated with the provided widget and context.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="widget">The widget.</param>
        /// <param name="context">The context.</param>
        /// <returns>A content model</returns>
        public IContentModel GetContentForContext(string site, string widget, string context)
        {
            IContentModel contentModel = new ContentModel();

            string content = this.siteBuilderService.GetModel(site, widget, context, "live")?.Content;
            contentModel.ContentJSON = content;
            contentModel.Success = true;

            return contentModel;
        }

        /// <summary>
        /// Gets the models for an entity type.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity type.</param>
        /// <returns>A list of models</returns>
        public ContentReturn GetModels(string site, string entity)
        {
            return this.siteBuilderService.GetModels(site, entity);
        }

        /// <summary>
        /// Gets the CMS json model.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// the content as JSON
        /// </returns>
        private string GetCMSJsonModel(string objectType, int id)
        {
            ISite site = this.siteService.GetSite(HttpContext.Current);
            string url = $"{site.IvectorConnectBaseUrl}cms/{objectType}/{id}";

            XmlDocument responseXML = this.SendWebRequest(url);
            string jsonContent = Newtonsoft.Json.JsonConvert.SerializeXmlNode(responseXML);

            return jsonContent;
        }

        /// <summary>
        /// Sends the web request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// an XML document
        /// </returns>
        private XmlDocument SendWebRequest(string url)
        {
            var request = new Intuitive.Net.WebRequests.Request()
                              {
                                  EndPoint = url, 
                                  SOAP = false, 
                                  ContentType = Intuitive.Net.WebRequests.ContentType.Application_x_www_form_urlencoded, 
                                  Method = Intuitive.Net.WebRequests.eRequestMethod.GET, 
                                  UseGZip = true, 
                                  TimeoutInSeconds = 30
                              };
            request.Send();

            XmlDocument responseXML = request.ResponseXML;
            return responseXML;
        }
    }
}