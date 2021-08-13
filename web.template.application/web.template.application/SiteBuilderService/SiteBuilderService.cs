namespace Web.Template.Application.SiteBuilderService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using Intuitive;

    using Newtonsoft.Json;

    using SiteBuilder.Domain.Poco.Return;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.SiteBuilder;
    using Web.Template.Application.Interfaces.User;
    using Web.Template.Application.SiteBuilderService.Models;
    using Web.Template.Application.User.Models;

    /// <summary>
    ///     class that handles communication with the site builder.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.SiteBuilder.ISiteBuilderService" />
    public class SiteBuilderService : ISiteBuilderService
    {
        /// <summary>
        ///     The site builder request
        /// </summary>
        private readonly ISiteBuilderRequest siteBuilderRequest;

        /// <summary>
        ///     The site service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        ///     The user service
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SiteBuilderService" /> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="siteBuilderRequest">The site builder request.</param>
        /// <param name="siteService">The site service.</param>
        public SiteBuilderService(IUserService userService, ISiteBuilderRequest siteBuilderRequest, ISiteService siteService)
        {
            this.userService = userService;
            this.siteBuilderRequest = siteBuilderRequest;
            this.siteService = siteService;
        }

        /// <summary>
        ///     Gets or sets the site builder URL.
        /// </summary>
        /// <value>The site builder URL.</value>
        private string SiteBuilderURL => "http://sitebuilder.intuitivesystems.co.uk";

        /// <summary>
        /// Adds the context bridge.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="siteName">Name of the site.</param>
        /// <returns>
        /// The response
        /// </returns>
        public string AddContextBridge(ContextBridgeRequest model, string siteName)
        {
            IUserSession user = this.userService.GetUser(HttpContext.Current);
            string body = JsonConvert.SerializeObject(model);
            var headers = new Dictionary<string, string> { { "Authorization", user.SiteBuilderAuthorization } };

            string url = $"{this.SitebuilderContextBridgeUrl(siteName)}/{model.Entity.ToLower()}/add";
            string addResponse = this.siteBuilderRequest.Send("POST", url, body, headers);

            return addResponse;
        }

        /// <summary>
        ///     Deletes the content.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        /// <param name="appendSitePrefix">The append site prefix.</param>
        /// <returns>The result</returns>
        public string DeleteContent(string site, string entity, string context, bool appendSitePrefix = true)
        {
            IUserSession user = this.userService.GetUser(HttpContext.Current);
            if (user.SelectedCmsWebsite != null && !user.SelectedCmsWebsite.Default && appendSitePrefix)
            {
                context = $"{context}_{user.SelectedCmsWebsite.ContentSuffix}";
            }

            string url = $"{this.SitebuilderEntityUrl(site)}/{entity.ToLower()}/en/{context.ToLower()}";
            var headers = new Dictionary<string, string> { { "Authorization", user.SiteBuilderAuthorization } };

            string responsebody = this.siteBuilderRequest.Send("DELETE", url, headers: headers);
            return responsebody;
        }

        /// <summary>
        /// Deletes the context bridge.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="siteName">Name of the site.</param>
        /// <returns>
        /// the response
        /// </returns>
        public string DeleteContextBridge(ContextBridgeRequest model, string siteName)
        {
            var contextBridgeId = 0;
            IUserSession user = this.userService.GetUser(HttpContext.Current);
            var headers = new Dictionary<string, string> { { "Authorization", user.SiteBuilderAuthorization } };

            contextBridgeId = this.GetContextBridgeID(model.OldContext, siteName, model.Entity);
            if (contextBridgeId == 0)
            {
                contextBridgeId = this.GetContextBridgeID(model.CurrentContext, siteName, model.Entity);
            }

            string url = $"{this.SitebuilderContextBridgeUrl(siteName)}/{model.Entity.ToLower()}/delete/{contextBridgeId}";
            string deleteResponse = this.siteBuilderRequest.Send("DELETE", url, headers: headers);

            return deleteResponse;
        }

        /// <summary>
        ///     Sends a request to the sitebuilder to find the context (which contains whether there is live or draft
        ///     content available), we want to find the correct context from the collection returned.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        /// <returns>A Context object, containing whether there is live or draft content available for the given Context</returns>
        public ContentReturnContext GetContentStatus(string site, string entity, string context)
        {
            // Hit the sitebuilder
            string contextResponse = this.siteBuilderRequest.Send("GET", $"{this.SitebuilderEntityUrl(site)}/{entity}/en");
            var contextModel = JsonConvert.DeserializeObject<ContentReturn>(contextResponse);

            // Work out if we want to look for site specific content
            string siteSpecificContext = string.Empty;
            IUserSession user = this.userService.GetUser(HttpContext.Current);

            if (user.SelectedCmsWebsite != null && !user.SelectedCmsWebsite.Default)
            {
                siteSpecificContext = $"{context}_{user.SelectedCmsWebsite.ContentSuffix}";
            }

            // get the right context, first looking if they have site specific content
            ContentReturnContext contextStatus;
            if (!string.IsNullOrEmpty(siteSpecificContext) && contextModel.Contexts.Any(con => !string.IsNullOrEmpty(con.Name) && con.Name.Equals(siteSpecificContext, StringComparison.CurrentCultureIgnoreCase)))
            {
                contextStatus = contextModel.Contexts.FirstOrDefault(con => con.Name.Equals(siteSpecificContext, StringComparison.CurrentCultureIgnoreCase));
            }
            else
            {
                var contextBridge = GetContextBridgeList(context, site, entity).FirstOrDefault(o => o.CurrentContext == context);
                var contextCompare = contextBridge == null ? context : contextBridge.OldContext;
                contextStatus = contextModel.Contexts.FirstOrDefault(con => con.Name.Equals(contextCompare, StringComparison.CurrentCultureIgnoreCase));
            }

            return contextStatus;
        }

        /// <summary>
        ///     This will send a request to the site builder that returns a list of all entities, if a type is provided
        ///     as a parameter, we will filter that list down to only entities of that type.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="type">The type of entity to filter the response for</param>
        /// <returns>a model Containing all entities</returns>
        public EntitiesReturn GetEntities(string site, string type = "")
        {
            string entityResponse = this.siteBuilderRequest.Send("GET", $"{this.SitebuilderEntityUrl(site)}");
            var entitiesModel = JsonConvert.DeserializeObject<EntitiesReturn>(entityResponse);

            if (type != string.Empty)
            {
                entitiesModel.Entities = entitiesModel.Entities.Where(e => e.Type.ToLower() == type.ToLower()).ToList();
            }

            return entitiesModel;
        }

        /// <summary>
        ///     Queries the sitebuilder, first to find out information about the entity e.g. the Schema, then about the context,
        ///     and finally
        ///     about the correct model.
        ///     The basic rule here is that, if we come in requesting live content, we always want live content, if we come in
        ///     requesting
        ///     draft content we want draft content unless none is available then we'll get live.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>An entity model</returns>
        public EntityModel GetEntity(string site, string entity, string context, string mode)
        {
            EntityModel entityModel;

            IUserSession user = this.userService.GetUser(HttpContext.Current);
            string cacheKey = $"entity-{site}|{entity}|{context}|{user.SelectedLanguage?.CultureCode}".ToLower();

            string siteSpecificCacheKey = string.Empty;
            if (user.SelectedCmsWebsite != null && !user.SelectedCmsWebsite.Default)
            {
                siteSpecificCacheKey = $"entity-{site}|{entity}|{context}_{user.SelectedCmsWebsite.ContentSuffix}|{user.SelectedLanguage?.CultureCode}".ToLower();
            }

            if (!string.IsNullOrEmpty(siteSpecificCacheKey) && HttpContext.Current.Cache[siteSpecificCacheKey] != null && !user.AdminMode)
            {
                entityModel = (EntityModel)HttpContext.Current.Cache[siteSpecificCacheKey];
            }
            else if (HttpContext.Current.Cache[cacheKey] != null && !user.AdminMode)
            {
                entityModel = (EntityModel)HttpContext.Current.Cache[cacheKey];
            }
            else
            {
                // Get the entity
                string entityResponse = this.siteBuilderRequest.Send("GET", $"{this.SitebuilderEntityUrl(site)}/{entity}");
                entityModel = JsonConvert.DeserializeObject<EntityModel>(entityResponse);

                if (entityModel != null)
                {
                    entityModel.Context = context;
                    entityModel.LastModifiedUser = "Glen Huxley";

                    // Get the context
                    ContentReturnContext contentReturnContext = this.GetContentStatus(site, entity, context);
                    entityModel.Status = this.GetStatus(contentReturnContext);
                    if (entityModel.Status == "live")
                    {
                        mode = "live";
                    }

                    // Get the model
                    ContentDetailsReturn modelReturn = this.GetModel(site, entity, context, mode);
                    if (modelReturn != null)
                    {
                        entityModel.Model = modelReturn.Content;
                        entityModel.LastModifiedDate = modelReturn.ActionDate;
                    }

                    // update cache key in case context changed to site specific context
                    if (contentReturnContext != null)
                    {
                        cacheKey = $"entity-{site}|{entity}|{contentReturnContext.Name}|{user.SelectedLanguage?.CultureCode}".ToLower();
                    }

                    this.InsertOrUpdateCache(cacheKey, entityModel);
                }
            }

            return entityModel;
        }

        /// <summary>
        ///     Requests the content Model, containing that cms content.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>The Model</returns>
        public ContentDetailsReturn GetModel(string site, string entity, string context, string mode)
        {
            ContentDetailsReturn modelReturn = null;
            try
            {
                string modelResponse;
                IUserSession user = this.userService.GetUser(HttpContext.Current);

                // 1. check for site specific content either from the cache or from site builder
                string siteSpecificCacheKey = string.Empty;
                if (user.SelectedCmsWebsite != null && !user.SelectedCmsWebsite.Default)
                {
                    siteSpecificCacheKey = $"model-{site}|{entity}|{context}_{user.SelectedCmsWebsite.ContentSuffix}|{user.SelectedLanguage?.CultureCode}".ToLower();
                }

                if (!string.IsNullOrEmpty(siteSpecificCacheKey) && HttpContext.Current.Cache[siteSpecificCacheKey] != null && !user.AdminMode)
                {
                    modelReturn = (ContentDetailsReturn)HttpContext.Current.Cache[siteSpecificCacheKey];
                }
                else if (!string.IsNullOrEmpty(siteSpecificCacheKey))
                {
                    string contextString = $"{context}_{user.SelectedCmsWebsite?.ContentSuffix}";
                    string url = $"{this.SitebuilderEntityUrl(site)}/{entity.ToLower()}/en/{UrlSafeString(contextString.ToLower())}?draft={mode.ToLower() == "draft"}";
                    modelResponse = this.siteBuilderRequest.Send("GET", url);

                    if (!string.IsNullOrEmpty(modelResponse))
                    {
                        modelReturn = JsonConvert.DeserializeObject<ContentDetailsReturn>(modelResponse);
                        this.InsertOrUpdateCache(siteSpecificCacheKey, modelReturn);
                    }
                }

                // 2. if no model check default cache key either from the cache or from sitebuilder
                string cacheKey = $"model-{site}|{entity}|{context}|{user.SelectedLanguage?.CultureCode}".ToLower();
                if (modelReturn == null && HttpContext.Current.Cache[cacheKey] != null && !user.AdminMode)
                {
                    modelReturn = (ContentDetailsReturn)HttpContext.Current.Cache[cacheKey];
                }
                else if (modelReturn == null)
                {
                    string url = $"{this.SitebuilderEntityUrl(site)}/{entity.ToLower()}/en/{UrlSafeString(context.ToLower())}?draft={mode.ToLower() == "draft"}";
                    modelResponse = this.siteBuilderRequest.Send("GET", url);

                    if (!string.IsNullOrEmpty(modelResponse))
                    {
                        modelReturn = JsonConvert.DeserializeObject<ContentDetailsReturn>(modelResponse);
                        this.InsertOrUpdateCache(cacheKey, modelReturn);
                    }
                }
            }
            catch (Exception ex)
            {
                FileFunctions.AddLogEntry("SiteBuilder", "GetModelError", ex.ToString());
            }

            return modelReturn ?? new ContentDetailsReturn();
        }

        /// <summary>
        ///     Gets the models.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>A list of all models and their content for a given entity</returns>
        public ContentReturn GetModels(string site, string entity)
        {
            string url = $"{this.SitebuilderEntityUrl(site)}/{entity.ToLower()}/en/?includecontent=true";

            string modelResponse = this.siteBuilderRequest.Send("GET", url);

            var modelReturn = JsonConvert.DeserializeObject<ContentReturn>(modelResponse);

            return modelReturn;
        }

        /// <summary>
        ///     Images the search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>a list of images matching the search criteria</returns>
        public string ImageSearch(string search, int width, int height)
        {
            ISite site = this.siteService.GetSite(HttpContext.Current);
            string url = $"{this.SiteBuilderBaseUrl(site.Name)}/images/{search}?width={width}&height={height}";

            string imageResponse = this.siteBuilderRequest.Send("GET", url);

            return imageResponse;
        }

        /// <summary>
        /// Modifies the context bridge.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="siteName">Name of the site.</param>
        /// <returns>
        /// The response
        /// </returns>
        public string ModifyContextBridge(ContextBridgeRequest model, string siteName)
        {
            IUserSession user = this.userService.GetUser(HttpContext.Current);
            var headers = new Dictionary<string, string> { { "Authorization", user.SiteBuilderAuthorization } };

            var contextBridgeId = 0;
            contextBridgeId = this.GetContextBridgeID(model.OldContext, siteName, model.Entity);
            if (contextBridgeId == 0)
            {
                contextBridgeId = this.GetContextBridgeID(model.CurrentContext, siteName, model.Entity);
            }

            model.ContextBridgeID = contextBridgeId;
            string body = JsonConvert.SerializeObject(model);

            string url = $"{this.SitebuilderContextBridgeUrl(siteName)}/{model.Entity.ToLower()}/edit";

            //// If a corresponding context does not exist to edit, add a new one instead.
            if (model.ContextBridgeID == 0) 
            {
                url = $"{this.SitebuilderContextBridgeUrl(siteName)}/{model.Entity.ToLower()}/add";
            }

        string editResponse = this.siteBuilderRequest.Send("POST", url, body, headers);

            return editResponse;
        }

        /// <summary>
        ///     Publish moves the draft content to live, and the live content to archive for a given entity and context.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        /// <param name="appendSitePrefix">The append site prefix.</param>
        /// <returns>The response returned from the sitebuilder</returns>
        public string PublishDraftContent(string site, string entity, string context, bool appendSitePrefix = true)
        {
            IUserSession user = this.userService.GetUser(HttpContext.Current);
            if (user.SelectedCmsWebsite != null && !user.SelectedCmsWebsite.Default && appendSitePrefix)
            {
                context = $"{context}_{user.SelectedCmsWebsite.ContentSuffix}";
            }

            string url = $"{this.SitebuilderEntityUrl(site)}/{entity.ToLower()}/en/{context.ToLower()}/publish";
            var headers = new Dictionary<string, string> { { "Authorization", user.SiteBuilderAuthorization } };

            string responsebody = this.siteBuilderRequest.Send("POST", url, headers: headers);
            this.UpdateCachedContent(site, entity, context);

            return responsebody;
        }

        /// <summary>
        ///     Content is cached on the site, when we publish we want to update the cache with the new content.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        public void UpdateCachedContent(string site, string entity, string context)
        {
            IUserSession userSession = this.userService.GetUser(HttpContext.Current);

            string cacheKey = $"entity-{site}|{entity}|{context}|{userSession.SelectedLanguage?.CultureCode}".ToLower();

            EntityModel entityModel = this.GetEntity(site, entity, context, "live");

            this.InsertOrUpdateCache(cacheKey, entityModel);
        }

        /// <summary>
        ///     Puts the content to the sitebuilder, the request used to update cms content.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        /// <param name="publish">The publish.</param>
        /// <returns>The response returned from the sitebuilder containing any warnings</returns>
        public string UpdateModel(string site, string entity, string context, string model, bool publish)
        {
            IUserSession user = this.userService.GetUser(HttpContext.Current);
            if (user.SelectedCmsWebsite != null && !user.SelectedCmsWebsite.Default && !publish)
            {
                context = $"{context}_{user.SelectedCmsWebsite.ContentSuffix}";
            }

            string url = $"{this.SitebuilderEntityUrl(site)}/{entity.ToLower()}/en/{context.ToLower()}?draft=true";

            var headers = new Dictionary<string, string> { { "Authorization", user.SiteBuilderAuthorization } };

            string responsebody = this.siteBuilderRequest.Send("PUT", url, model, headers);

            if (publish)
            {
                return this.PublishDraftContent(site, entity, context, false);
            }

            return responsebody;
        }

        /// <summary>
        ///     Users the login.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The session</returns>
        public ISession UserLogin(string username, string password)
        {
            ISite site = this.siteService.GetSite(HttpContext.Current);

            string url = $"{this.SiteBuilderURL}/login";
            string body = $"grant_type=password&username={username}&password={password}&site={site.Name}";

            string modelResponse = this.siteBuilderRequest.Send("POST", url, body);
            var modelReturn = JsonConvert.DeserializeObject<AuthorizationModel>(modelResponse);

            ISession session = new Session();

            IUserSession user = this.userService.GetUser(HttpContext.Current);

            if (string.IsNullOrEmpty(modelReturn.error))
            {
                user.AdminMode = true;
                var adminSession = new AdminSession { LoggedIn = true, FirstName = modelReturn.firstName, LastName = modelReturn.lastName, Email = username };
                user.AdminSession = adminSession;
                user.SiteBuilderAuthorization = $"{modelReturn.token_type} {modelReturn.access_token}";
                this.userService.SetUser(user, HttpContext.Current);
            }
            else
            {
                session.Warnings = new List<string>();
                session.Warnings.Add(modelReturn.error_description);
            }

            session.UserSession = user;
            return session;
        }

        /// <summary>
        ///     URLs the safe string.
        /// </summary>
        /// <param name="stringToEncode">The string to encode.</param>
        /// <returns>The string you passed in, made lowercase with spaces replaced</returns>
        private static string UrlSafeString(string stringToEncode)
        {
            return stringToEncode.ToLower().Replace(" ", "-").Replace("'", string.Empty).Replace("*", string.Empty).Replace(",", string.Empty).Replace(".", string.Empty).Replace(":", string.Empty).Replace("&", "and").Replace("é", "e").Replace("+", "-");
        }

        /// <summary>
        /// Gets the context bridge identifier.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="siteName">Name of the site.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <returns>
        /// the context bridge id
        /// </returns>
        private int GetContextBridgeID(string context, string siteName, string entityName)
        {
            var models = GetContextBridgeList(context, siteName, entityName);

            var id = 0;

            if (models.Count(m => (m.OldContext == context || m.CurrentContext == context)) > 0)
            {
                id = models.FirstOrDefault(m => m.OldContext == context || m.CurrentContext == context).ContextBridgeID;
            }

            return id;
        }

        /// <summary>
        /// Gets the context bridge list.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="siteName">Name of the site.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <returns>
        /// the context bridge list
        /// </returns>
        private List<ContextBridge> GetContextBridgeList(string context, string siteName, string entityName)
        {
            string url = $"{this.SitebuilderContextBridgeUrl(siteName)}/{entityName.ToLower()}/getall";

            string contexts = this.siteBuilderRequest.Send("GET", url);

            return  JsonConvert.DeserializeObject<List<ContextBridge>>(contexts);

        }

        /// <summary>
        ///     Gets the status.
        /// </summary>
        /// <param name="contextWrapper">The context wrapper.</param>
        /// <returns>The status.</returns>
        private string GetStatus(ContentReturnContext contextWrapper)
        {
            var status = "no content";
            if (contextWrapper != null)
            {
                if (contextWrapper.ContentDraft)
                {
                    status = "draft";
                }
                else if (contextWrapper.Content)
                {
                    status = "live";
                }
            }

            return status;
        }

        /// <summary>
        ///     Inserts the or update cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cacheItem">The cache item.</param>
        private void InsertOrUpdateCache(string key, object cacheItem)
        {
            if (string.IsNullOrEmpty(key) || cacheItem == null)
            {
                return;
            }

            if (HttpContext.Current.Cache[key] != null)
            {
                HttpContext.Current.Cache[key] = cacheItem;
            }
            else
            {
                HttpContext.Current.Cache.Insert(key, cacheItem);
            }
        }

        /// <summary>
        ///     Sites the builder base URL.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns>the Site builder base Url</returns>
        private string SiteBuilderBaseUrl(string site)
        {
            return $"{this.SiteBuilderURL}/sites/{site.ToLower()}";
        }

        /// <summary>
        ///     Site builders the entity URL.
        /// </summary>
        /// <param name="siteName">Name of the site.</param>
        /// <returns>the Site builder Entity Url</returns>
        private string SitebuilderContextBridgeUrl(string siteName)
        {
            ISite site = this.siteService.GetSite(HttpContext.Current);
            return $"{this.SiteBuilderURL}/contextbridge/sites/{siteName.ToLower()}/instances/{site.Environment.ToLower()}/entities";
        }

        /// <summary>
        ///     Site builders the entity URL.
        /// </summary>
        /// <param name="siteName">Name of the site.</param>
        /// <returns>the Site builder Entity Url</returns>
        private string SitebuilderEntityUrl(string siteName)
        {
            ISite site = this.siteService.GetSite(HttpContext.Current);
            return $"{this.SiteBuilderBaseUrl(siteName)}/instances/{site.Environment.ToLower()}/entities";
        }
    }
}