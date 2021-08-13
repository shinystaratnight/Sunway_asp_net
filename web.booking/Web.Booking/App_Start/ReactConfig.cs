[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Web.Booking.ReactConfig), "Configure")]

namespace Web.Booking
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
            ReactSiteConfiguration.Configuration.SetLoadBabel(false)
                .SetAllowMsieEngine(false)
                .AddScriptWithoutTransform("~/assets/js/vendor.bundle.js")
                .AddScriptWithoutTransform("~/assets/sunway/js/server.js")
                .AddScriptWithoutTransform("~/assets/sunwayb2c/js/server.js");
        }
    }
}