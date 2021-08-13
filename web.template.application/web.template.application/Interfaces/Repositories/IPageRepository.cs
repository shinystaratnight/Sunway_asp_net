namespace Web.Template.Application.Interfaces.Repositories
{
    using System.Collections.Generic;

    using Web.Template.Application.PageDefinition;

    /// <summary>
    /// The Interface for the Page repository
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Repositories.IRepository{Web.SiteBuilder.Domain.PageDefinition.Page,System.String}" />
    public interface IPageRepository : IRepository<Page, string>
    {
        /// <summary>
        /// Finds all pages
        /// </summary>
        /// <returns>A list of all Pages in the repo</returns>
        List<Page> FindAll();

        /// <summary>
        /// Finds the by URL.
        /// </summary>
        /// <param name="url">The URL that you want the page for</param>
        /// <returns>A Page</returns>
        Page FindByUrl(string url);
    }
}