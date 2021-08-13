namespace Web.Booking
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
                new { controller = "PageBuilder", action = "Render" });

            routes.MapRoute(
                "Results",
                "results/{*SearchParams}",
                new { controller = "PageBuilder", action = "Setup", pagePath = "results" });

            routes.MapRoute(
                "PropertyDetails",
                "details/{*SearchParams}",
                new { controller = "PageBuilder", action = "Setup", pagePath = "details" });

            routes.MapRoute(
                "Extras",
                "extras/{*SearchParams}",
                new { controller = "PageBuilder", action = "Setup", pagePath = "extras" });

            routes.MapRoute(
                "Payment",
                "payment/{*SearchParams}",
                new { controller = "PageBuilder", action = "Setup", pagePath = "payment" });

            routes.MapRoute(
                "Conditions",
                "conditions/{*SearchParams}",
                new { controller = "PageBuilder", action = "Setup", pagePath = "conditions" });

            routes.MapRoute(
                "QuoteRetrieve",
                "quote/{*QuoteReference}",
                new { controller = "PageBuilder", action = "Setup", pagePath = "quote" });

            routes.MapRoute(
                "3DSecure",
                "3dsecure/{action}",
                new { controller = "ThreeDSecure" });

            routes.MapRoute(
                "OffsitePayment",
                "offsitepayment/PaymentResponse",
                new { controller = "OffsitePayment", action = "PaymentResponse" });

            routes.MapRoute(
                "OffsitePaymentPost",
                "offsitepayment/PaymentResponsePost",
                new { controller = "OffsitePayment", action = "PaymentResponsePost" });

            routes.MapRoute(
                "Offsite",
                "offsitepayment/{*SearchParams}",
                new { controller = "OffsitePayment", action = "Submit" });

            routes.MapRoute(
                "Default",
                "{*pagePath}",
                new { controller = "PageBuilder", action = "Setup", pagePath = "/", guid = string.Empty });
        }
    }
}