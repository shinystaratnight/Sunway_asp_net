namespace Web.Template.Application.Interfaces.Services
{
    using System.Collections.Generic;

    using Web.Template.Application.PageDefinition;

    /// <summary>
    ///     Interface for the service used to return pages from the site builder
    /// </summary>
    public interface IPageService
    {
        /// <summary>
        ///     Gets the page by URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>A page</returns>
        Page GetPageByURL(string url);

        /// <summary>
        /// Gets all pages.
        /// </summary>
        /// <returns></returns>
        List<Page> GetAll();
    }
}