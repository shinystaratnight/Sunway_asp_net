[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Web.Template.ReactConfig), "Configure")]

namespace Web.Template
{
    using React;

    /// <summary>
    /// React configuration
    /// </summary>
    public static class ReactConfig
    {
        /// <summary>
        /// Configures this instance.
        /// </summary>
        public static void Configure()
        {
            // If you want to use server-side rendering of React components, 
            // add all the necessary JavaScript files here. This includes 
            // your components as well as all of their dependencies.
            // See http://reactjs.net/ for more information. Example:
            // ReactSiteConfiguration.Configuration
            // .AddScript("~/Scripts/First.jsx")
            // .AddScript("~/Scripts/Second.jsx");

            // If you use an external build too (for example, Babel, Webpack,
            // Browserify or Gulp), you can improve performance by disabling 
            // ReactJS.NET's version of Babel and loading the pre-transpiled 
            // scripts. Example:
            // ReactSiteConfiguration.Configuration
            // .SetLoadBabel(false)
            // .AddScriptWithoutTransform("~/Scripts/bundle.server.js")
            ReactSiteConfiguration.Configuration
                .SetLoadBabel(false)
                .SetAllowMsieEngine(false)
                .AddScriptWithoutTransform("~/assets/js/vendor.bundle.js")
                .AddScriptWithoutTransform("~/assets/sunway/js/server.js");
        }
    }
}
