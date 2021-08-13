namespace Web.Template
{
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    ///     The Route configuration, use to configure routes for MVC
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        ///     Registers the routes.
        /// </summary>
        /// <param name="routes">The routes.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("WidgetRoute", "Widget/{controller}/{action}/{widget}", new { controller = "PageBuilder", action = "Render" });

            routes.MapRoute("HotelResultsRoute", "results/hotel/{*SearchParams}", new { controller = "PageBuilder", action = "Setup", pagePath = "results/hotel" });
            routes.MapRoute("FlightResultsRoute", "results/flight/{*SearchParams}", new { controller = "PageBuilder", action = "Setup", pagePath = "results/flight" });
            routes.MapRoute("FlightPlusHotelResultsRoute", "results/flightplushotel/{*SearchParams}", new { controller = "PageBuilder", action = "Setup", pagePath = "results/flightplushotel" });

            routes.MapRoute("Payment", "payment", new { controller = "PageBuilder", action = "Setup", pagePath = "payment" });

            routes.MapRoute(name: "Default", url: "{*pagePath}", defaults: new { controller = "PageBuilder", action = "Setup", pagePath = "/", guid = string.Empty });
        }
    }
}