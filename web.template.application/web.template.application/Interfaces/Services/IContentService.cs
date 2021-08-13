namespace Web.Template.Application.Interfaces.Services
{
    using System.Xml;

    using global::SiteBuilder.Domain.Poco.Return;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    ///     Content Service used to return content to the front end
    /// </summary>
    public interface IContentService
    {
        /// <summary>
        /// Gets the type of the CMS content for object.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>CMS content model</returns>
        IContentModel GetCMSContentForObjectType(string objectType, int id);

        /// <summary>
        /// Gets the CMSXML model.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>an xml document</returns>
        XmlDocument GetCmsxmlModel(string objectType, int id);

        /// <summary>
        /// Gets the content associated with the provided widget and context.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="widget">The widget.</param>
        /// <param name="context">The context.</param>
        /// <returns>A content model</returns>
        IContentModel GetContentForContext(string site, string widget, string context);

        /// <summary>
        /// Gets the models.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>A list of models</returns>
        ContentReturn GetModels(string site, string entity);
    }
}