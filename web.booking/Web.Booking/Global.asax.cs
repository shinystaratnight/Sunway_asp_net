namespace Web.Booking
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Web.Booking.IoC;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Support;
    using Web.Template.Data.Site;

    /// <summary>
    ///     The start of the application, configures the application.
    /// </summary>
    /// <seealso cref="System.Web.HttpApplication" />
    public class ApplicationRoot : HttpApplication
    {
        /// <summary>
        /// Application_s the post authorize request.
        /// </summary>
        protected void Application_PostAuthorizeRequest()
        {
            HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }

        /// <summary>
        /// Handles the Start event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            this.SetupRoutes();
            AutofacRoot.Setup();
        }

        /// <summary>
        /// Handles the Start event of the Session control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Session_Start(object sender, EventArgs e)
        {
            var websiteRepository = DependencyResolver.Current.GetService<IWebsiteRepository>();
            var defaultSite = websiteRepository.GetDefaultSite();

            var configuration = DependencyResolver.Current.GetService<IConfiguration>();

            var userService = DependencyResolver.Current.GetService<IUserService>();

            if (configuration.SetUserFromCookie)
            {
                userService.SetUserFromCookie(defaultSite);
            }
            else
            {
                userService.NewUser(defaultSite);
            }

            if (configuration.UseIpRedirect)
            {
                var ipLookupService = DependencyResolver.Current.GetService<IIpLookupService>();
                var clientCountryCode = ipLookupService.GetClientCountryCode();

                var localWebsite = websiteRepository.GetWebsiteByCountryCode(clientCountryCode);
                if (localWebsite != null)
                {
                    userService.SetSelectedCmsWebsite(localWebsite);
                }
            }
        }

        /// <summary>
        ///     Setups the routes.
        /// </summary>
        private void SetupRoutes()
        {
            ViewEngines.Engines.Clear();
            var engine = new ExtendedViewEngine();
            engine.AddViewLocationFormat("~/Views/PageTemplates/{0}.cshtml");
            engine.AddPartialViewLocationFormat("~/Views/Widgets/{0}.cshtml");
            engine.AddPartialViewLocationFormat("~/bin/Views/{0}.cshtml");

            engine.AddPartialViewLocationFormat("~/Views/SubWidgets/{0}.cshtml");
            ViewEngines.Engines.Add(engine);
        }

        /// <summary>
        ///     Extended view engine allows us to specify custom paths that razor will look for views in.
        /// </summary>
        /// <seealso cref="System.Web.Mvc.RazorViewEngine" />
        public class ExtendedViewEngine : RazorViewEngine
        {
            /// <summary>
            ///     Adds the partial view location format.
            /// </summary>
            /// <param name="paths">The paths.</param>
            public void AddPartialViewLocationFormat(string paths)
            {
                var existingPaths = new List<string>(this.PartialViewLocationFormats);
                existingPaths.Add(paths);

                this.PartialViewLocationFormats = existingPaths.ToArray();
            }

            /// <summary>
            ///     Adds the view location format.
            /// </summary>
            /// <param name="paths">The paths.</param>
            public void AddViewLocationFormat(string paths)
            {
                var existingPaths = new List<string>(this.ViewLocationFormats);
                existingPaths.Add(paths);

                this.ViewLocationFormats = existingPaths.ToArray();
            }
        }
    }
}