[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Web.TradeMMB.ReactConfig), "Configure")]

namespace Web.TradeMMB
{
    using System.Net;
    using System.Web;

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
            ReactSiteConfiguration.Configuration
                .SetLoadBabel(false)
                .SetAllowMsieEngine(false)
                .AddScriptWithoutTransform("~/assets/js/vendor.bundle.js")
                .AddScriptWithoutTransform("~/assets/sunway/js/server.js");
        }
    }
}