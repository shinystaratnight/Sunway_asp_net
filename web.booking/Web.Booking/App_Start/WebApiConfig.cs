namespace Web.Booking
{
    using System.Net.Http.Extensions.Compression.Core.Compressors;
    using System.Web.Http;

    using Microsoft.AspNet.WebApi.Extensions.Compression.Server;

    /// <summary>
    ///     Configures WebAPI including the routing.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        ///     Registers the specified configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            json.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            GlobalConfiguration.Configuration.MessageHandlers.Insert(0, new ServerCompressionHandler(1024, new GZipCompressor(), new DeflateCompressor()));
        }
    }
}