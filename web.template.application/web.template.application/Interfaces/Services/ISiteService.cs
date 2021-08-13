namespace Web.Template.Application.Interfaces.Services
{
    using System.Web;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.User;

    /// <summary>
    /// Interface ISiteService
    /// </summary>
    public interface ISiteService
    {
        /// <summary>
        /// Gets the site.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The site</returns>
        ISite GetSite(HttpContext context);
    }
}