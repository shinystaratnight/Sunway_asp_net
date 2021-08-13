namespace Web.Template
{
    using System.Web.Mvc;

    /// <summary>
    ///     Configures global filters, any global filters should be here.
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        ///     Registers the global filters.
        /// </summary>
        /// <param name="filters">The filters.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}