namespace Web.Template.Data.Site
{
    using System.Collections.Generic;

    using Web.Template.Domain.Entities.Site;

    /// <summary>
    /// Interface defining the behavior of a website repository.
    /// </summary>
    public interface IWebsiteRepository
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>A list of all websites</returns>
        List<CmsWebsite> GetAll();

        /// <summary>
        /// Gets the default site.
        /// </summary>
        /// <returns>A single website that is the default</returns>
        CmsWebsite GetDefaultSite();

        /// <summary>
        /// Gets the website by iso code.
        /// </summary>
        /// <param name="countryCode">The iso code.</param>
        /// <returns>A single website that matches the provided country Code</returns>
        CmsWebsite GetWebsiteByCountryCode(string countryCode);

        /// <summary>
        /// Gets the name of the website by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A single website that matches the provided name</returns>
        CmsWebsite GetWebsiteByName(string name);
    }
}