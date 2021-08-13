namespace Web.Template.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Xml;

    using Intuitive;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using SiteBuilder.Domain.Models.Model;

    using Web.Template.Application.Interfaces.Logging;
    using Web.Template.Application.Interfaces.PageDefinition;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.SiteBuilder;
    using Web.Template.Application.Interfaces.User;
    using Web.Template.Application.PageDefinition;
    using Web.Template.Application.PageDefinition.Enums;
    using Web.Template.Application.SiteBuilderService.Models;
    using Web.Template.Models.Application;

    using Formatting = Newtonsoft.Json.Formatting;
    using ISite = Web.Template.Application.Interfaces.Configuration.ISite;

    /// <summary>
    ///     The Pagebuilder controller handles the custom pages made by the sitebuilder
    /// </summary>
    /// <seealso cref="System.Web.Mvc.Controller" />
    public class PageBuilderController : Controller
    {
        /// <summary>
        /// The content service
        /// </summary>
        private readonly IContentService contentService;

        /// <summary>
        ///     The page service
        /// </summary>
        private readonly IPageService pageService;

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
        /// The site service
        /// </summary>
        private readonly ISiteBuilderService siteBuilderService;

        /// <summary>
        /// The redirect service
        /// </summary>
        private readonly IRedirectService redirectService;

        /// <summary>
        /// The log writer
        /// </summary>
        private readonly ILogWriter logWriter;


        /// <summary>
        /// Initializes a new instance of the <see cref="PageBuilderController" /> class.
        /// </summary>
        /// <param name="pageService">The page service.</param>
        /// <param name="contentService">The content service.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="siteService">The site service.</param>
        /// <param name="trackingAffiliateService">The tracking affiliate service.</param>
        /// <param name="siteBuilderService">The site builder service.</param>
        /// <param name="logWriter">The log writer.</param>
        /// <param name="redirectService">The redirect service.</param>
        public PageBuilderController(
            IPageService pageService,
            IContentService contentService,
            IUserService userService,
            ISiteService siteService,
            ITrackingAffiliateService trackingAffiliateService,
            ISiteBuilderService siteBuilderService,
            ILogWriter logWriter, IRedirectService redirectService)
        {
            this.pageService = pageService;
            this.contentService = contentService;
            this.userService = userService;
            this.siteService = siteService;
            this.trackingAffiliateService = trackingAffiliateService;
            this.siteBuilderService = siteBuilderService;
            this.logWriter = logWriter;
            this.redirectService = redirectService;
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
            ActionResult actionResult;
            if (HandleRedirect(pagePath, out actionResult)) return actionResult;


            if (clearcache)
            {
                Intuitive.Functions.ClearCache();
            }

            try
            {
                IPage pageModel = this.pageService.GetPageByURL(pagePath);
                IUserSession userSession = this.userService.GetUser(System.Web.HttpContext.Current);

                ISite site = this.siteService.GetSite(System.Web.HttpContext.Current);
                pageModel.SiteName = site.Name;
                pageModel.TwitterHandle = site.SiteConfiguration.TwitterConfiguration.TwitterHandle;

                if (pageModel.Access != AccessLevel.Public && !this.CheckAuthentication(userSession, pageModel))
                {
                    return this.Redirect($"{site.DefaultPage}?warning=login");
                }
                pageModel.AdminMode = userSession.AdminMode;

                ReduxState state = new ReduxState { entities = new Dictionary<string, object>() };
                dynamic data = this.GetThemeData(site);

                pageModel.FontScript = data.Typography.FontScript;
                pageModel.FontSource = data.Typography.FontSource;
                
                // Get tracking Affiliates
                pageModel.TrackingAffiliates = await this.trackingAffiliateService.SetupTrackingAffiliates();

                foreach (Section section in pageModel.Sections)
                {
                    foreach (Widget widget in section.Widgets)
                    {
                        SetupWidget(widget, pageModel, userSession, state);
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

        /// <summary>
        /// Setups the widget.
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <param name="pageModel">The page model.</param>
        /// <param name="userSession">The user session.</param>
        /// <param name="state">The state.</param>
        private void SetupWidget(Widget widget, IPage pageModel, IUserSession userSession, ReduxState state)
        {
            widget.ContentService = this.contentService;
            widget.UserService = this.userService;
            widget.SiteService = this.siteService;
            widget.Page = new PageViewModel()
            {
                EntityInformations = pageModel.EntityInformations,
                EntityType = pageModel.EntityType,
                PageURL = pageModel.Url,
                PageName = System.Text.RegularExpressions.Regex.Replace(pageModel.Name, "([a-z])([A-Z])", "$1 $2"),
                MetaInformation = pageModel.MetaInformation
            };
            widget.SetupContent();

            if (!userSession.AdminMode && widget.ClientSideRender && widget.ServerSideRender && !string.IsNullOrEmpty(widget.Context))
            {
                var siteName = widget.Shared ? "Shared" : widget.Site.Name;
                EntityModel entityModel = this.siteBuilderService.GetEntity(siteName,
                    widget.Name,
                    widget.ContentContext ?? widget.Context,
                    "live");

                ReduxEntityModel entity = new ReduxEntityModel
                {
                    jsonSchema = entityModel.JsonSchema,
                    model = widget.ContentJSON,
                    isFetching = false,
                    isLoaded = true,
                    name = widget.Name,
                    context = widget.ContentContext ?? widget.Context,
                    lastModifiedDate = entityModel.LastModifiedDate,
                    lastModifiedUser = entityModel.LastModifiedUser,
                    status = entityModel.Status,
                    type = entityModel.Type
                };


                string key = $"{widget.Name}-{entity.context}";
                state.entities.Add(key, entity);
            }
        }

        /// <summary>
        /// checks if there are any loaded redirects for the requested path if so recommends where to redirec to.
        /// </summary>
        /// <param name="pagePath">The page path.</param>
        /// <param name="actionResult">The action result.</param>
        /// <returns>wheter a redirect exists or not</returns>
        private bool HandleRedirect(string pagePath, out ActionResult actionResult)
        {

            var redirects = this.redirectService.GetRedirects();
            var redirectExists = false;

            if (redirects.Exists(r => r.Url == "/" + pagePath))
            {
                actionResult = RedirectPermanent(redirects.FirstOrDefault(r => r.Url == "/" + pagePath)?.RedirectUrl);
                redirectExists = true;
            }
            else
            {
                actionResult = null;
            }
            
            return redirectExists;
        }

        private dynamic GetThemeData(ISite site)
        {
            var themeKey = $"site_websites_{site.Name}";
            var cachekey = Intuitive.AsyncCache.Controller<dynamic>.GenerateKey(themeKey);

            dynamic data = Intuitive.AsyncCache.Controller<dynamic>.GetCache(
                cachekey,
                600,
                () =>
                    {
                        EntityModel themeModel = this.siteBuilderService.GetEntity(site.Name, "Theme", "default", "live");
                        dynamic content = JObject.Parse(themeModel.Model);
                        return content;
                    });
            return data;
        }

        /// <summary>
        /// Checks the authentication.
        /// </summary>
        /// <param name="userSession">The user session.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public bool CheckAuthentication(IUserSession userSession, IPage page)
        {
            var authorized = true;

            switch (page.Access)
            {
                case AccessLevel.TradeLoggedIn:
                    authorized = (userSession.TradeSession != null && userSession.TradeSession.LoggedIn) || userSession.AdminMode;
                    break;
            }

            return authorized;

        }

    }
}