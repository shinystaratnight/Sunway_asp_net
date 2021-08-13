namespace Web.TradeMMB
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        /// <summary>
        ///     Registers the routes.
        /// </summary>
        /// <param name="routes">The routes.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "WidgetRoute", 
                "Widget/{controller}/{action}/{widget}", 
                new { controller = "Page", action = "Render" });

            routes.MapRoute(
                "Results", 
                "results/{*SearchParams}", 
                new { controller = "Page", action = "Setup", pagePath = "results" });

            routes.MapRoute(
                "PropertyDetails", 
                "details/{*SearchParams}", 
                new { controller = "Page", action = "Setup", pagePath = "details" });

            routes.MapRoute(
                "Extras",
                "extras/{*SearchParams}",
                new { controller = "Page", action = "Setup", pagePath = "extras" });

            routes.MapRoute(
                "Payment",
                "payment/{*SearchParams}", 
                new { controller = "Page", action = "Setup", pagePath = "payment" });

            routes.MapRoute(
                name: "Default", 
                url: "{*pagePath}", 
                defaults: new { controller = "Page", action = "Setup", pagePath = "/", guid = string.Empty });
        }
    }
}