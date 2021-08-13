namespace Web.TradeMMB.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Intuitive;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.SiteBuilder;
    using Web.Template.Application.Interfaces.User;
    using Web.Template.Application.PageDefinition;
    using Web.Template.Application.PageDefinition.Enums;
    using Web.Template.Application.SiteBuilderService.Models;
    using Web.TradeMMB.Models.Application;

    /// <summary>
    ///     The Pagebuilder controller handles the custom pages made by the sitebuilder
    /// </summary>
    /// <seealso cref="System.Web.Mvc.Controller" />
    public class PageController : Controller
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
        ///     The page service
        /// </summary>
        private readonly IPageService pageService;

        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteBuilderService siteBuilderService;

        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// The tracking affiliate service
        /// </summary>
        private readonly ITrackingAffiliateService trackingAffiliateService;

        /// <summary>
        /// The user service
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageController" /> class.
        /// </summary>
        /// <param name="pageService">The page service.</param>
        /// <param name="contentService">The content service.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="siteService">The site service.</param>
        /// <param name="trackingAffiliateService">The tracking affiliate service.</param>
        /// <param name="siteBuilderService">The site builder service.</param>
        /// <param name="logWriter">The log writer.</param>
        public PageController(
            IPageService pageService,
            IContentService contentService,
            IUserService userService,
            ISiteService siteService,
            ITrackingAffiliateService trackingAffiliateService,
            ISiteBuilderService siteBuilderService,
            ILogWriter logWriter)
        {
            this.pageService = pageService;
            this.contentService = contentService;
            this.userService = userService;
            this.siteService = siteService;
            this.trackingAffiliateService = trackingAffiliateService;
            this.siteBuilderService = siteBuilderService;
            this.logWriter = logWriter;
        }

        /// <summary>
        /// Checks the authentication.
        /// </summary>
        /// <param name="userSession">The user session.</param>
        /// <param name="page">The page.</param>
        /// <returns>Returns true if the user is authorized</returns>
        public bool CheckAuthentication(IUserSession userSession, Page page)
        {
            var authorized = true;

            switch (page.Access)
            {
                case AccessLevel.TradeLoggedIn:
                    authorized = (userSession.TradeSession != null && userSession.TradeSession.LoggedIn)
                                 || userSession.AdminMode;
                    break;
            }

            return authorized;
        }

        /// <summary>
        /// Setups the specified page path.
        /// </summary>
        /// <param name="pagePath">The page path.</param>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="clearcache">if set to <c>true</c> [clear cache].</param>
        /// <returns>
        /// Returns a page, that has been constructed by the sitebuilder
        /// </returns>
        public async Task<ActionResult> Setup(string pagePath, string guid = "", bool clearcache = false)
        {
            if (clearcache)
            {
                Intuitive.Functions.ClearCache();
            }

            try
            {
                Page pageModel = this.pageService.GetPageByURL(pagePath);
                IUserSession userSession = this.userService.GetUser(System.Web.HttpContext.Current);

                if (pageModel.Access != AccessLevel.Public && !this.CheckAuthentication(userSession, pageModel))
                {
                    return this.Redirect("/?warning=login");
                }

                pageModel.AdminMode = userSession.AdminMode;

                ReduxState state = new ReduxState { entities = new Dictionary<string, object>() };

                ISite site = this.siteService.GetSite(System.Web.HttpContext.Current);
                pageModel.SiteBaseUrl = site.Url;
                pageModel.SiteName = site.Name;

#if DEBUG
                pageModel.SiteBaseUrl = "http://localhost:64351";
#endif

                EntityModel themeModel = this.siteBuilderService.GetEntity(site.Name, "Theme", "default", "live");
                dynamic data = JObject.Parse(themeModel.Model);
                pageModel.FontScript = data.Typography.FontScript;
                pageModel.FontSource = data.Typography.FontSource;

                // Get tracking Affiliates
                pageModel.TrackingAffiliates = await this.trackingAffiliateService.SetupTrackingAffiliates();

                foreach (Section section in pageModel.Sections)
                {
                    foreach (Widget widget in section.Widgets)
                    {
                        widget.ContentService = this.contentService;
                        widget.UserService = this.userService;
                        widget.SiteService = this.siteService;
                        widget.Page = new PageViewModel()
                        {
                            EntityInformations = pageModel.EntityInformations,
                            EntityType = pageModel.EntityType,
                            PageURL = pageModel.Url,
                            PageName = pageModel.Name,
                            MetaInformation = pageModel.MetaInformation
                        };
                        widget.SetupContent();

                        if (widget.ClientSideRender && widget.ServerSideRender && !string.IsNullOrEmpty(widget.Context))
                        {
                            var siteName = widget.Shared ? "Shared" : widget.Site.Name;
                            EntityModel entityModel = this.siteBuilderService.GetEntity(
                                siteName,
                                widget.Name,
                                widget.Context,
                                "live");

                            ReduxEntityModel entity = new ReduxEntityModel
                            {
                                jsonSchema = entityModel.JsonSchema,
                                model = widget.ContentJSON,
                                isFetching = false,
                                isLoaded = true,
                                name = widget.Name,
                                context = widget.Context,
                                lastModifiedDate = entityModel.LastModifiedDate,
                                lastModifiedUser = entityModel.LastModifiedUser,
                                status = entityModel.Status,
                                type = entityModel.Type
                            };

                            string key = $"{widget.Name}-{entity.context}";
                            state.entities.Add(key, entity);
                        }
                    }
                }

                string stateJSON = JsonConvert.SerializeObject(state, Formatting.None);
                pageModel.PreLoadedState = stateJSON;

                return this.View(pageModel.Template, pageModel);
            }
            catch (Exception ex)
            {
                this.logWriter.Write("PagebuilderController", "SetupError", ex.ToString());
            }

            return null;
        }
    }
}