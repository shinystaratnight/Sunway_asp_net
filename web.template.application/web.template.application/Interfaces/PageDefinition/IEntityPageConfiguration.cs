namespace Web.Template.Application.Interfaces.PageDefinition
{
    using System.Collections.Generic;

    using Web.Template.Application.PageDefinition;

    /// <summary>
    /// Class responsible for generating a list of entity pages.
    /// </summary>
    public interface IEntityPageConfiguration
    {
        /// <summary>
        /// Configures this instance.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns>A list of entity pages.</returns>
        List<EntityPage> Configure(string site);
    }
}