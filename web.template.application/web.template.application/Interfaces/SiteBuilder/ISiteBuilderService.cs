namespace Web.Template.Application.Interfaces.SiteBuilder
{
    using global::SiteBuilder.Domain.Poco.Return;

    using Web.Template.Application.Interfaces.User;
    using Web.Template.Application.SiteBuilderService.Models;

    /// <summary>
    /// Interface defining communication with the site builder.
    /// </summary>
    public interface ISiteBuilderService
    {
        /// <summary>
        /// Deletes the content.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        /// <param name="appendSitePrefix">if set to <c>true</c> [append site prefix].</param>
        /// <returns>The result</returns>
        string DeleteContent(string site, string entity, string context, bool appendSitePrefix = true);

        /// <summary>
        /// Gets the content status.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        /// <returns>A Context object, containing whether there is live or draft content available for the given Context</returns>
        ContentReturnContext GetContentStatus(string site, string entity, string context);

        /// <summary>
        /// Gets the entities.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="type">The type.</param>
        /// <returns>a model Containing all entities</returns>
        EntitiesReturn GetEntities(string site, string type = "");

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>An entity model</returns>
        EntityModel GetEntity(string site, string entity, string context, string mode);

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>The model</returns>
        ContentDetailsReturn GetModel(string site, string entity, string context, string mode);

        /// <summary>
        /// Gets the models.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>The models.</returns>
        ContentReturn GetModels(string site, string entity);

        /// <summary>
        /// Images the search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>A list of images that match the search criteria</returns>
        string ImageSearch(string search, int width, int height);

        /// <summary>
        /// Publishes the content of the draft.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        /// <param name="appendSitePrefix">if set to <c>true</c> [append site prefix].</param>
        /// <returns>The response from the site builder</returns>
        string PublishDraftContent(string site, string entity, string context, bool appendSitePrefix = true);

        /// <summary>
        /// Updates the model.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        /// <param name="publish">if set to <c>true</c> [publish].</param>
        /// <returns>The response from the site builder</returns>
        string UpdateModel(string site, string entity, string context, string model, bool publish);

        /// <summary>
        /// Users the login.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The login result</returns>
        ISession UserLogin(string username, string password);

        /// <summary>
        /// Adds the context bridge.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="siteName">Name of the site.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        string AddContextBridge(ContextBridgeRequest model, string siteName);

        /// <summary>
        /// Modifies the context bridge.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="siteName">Name of the site.</param>
        /// <returns></returns>
        string ModifyContextBridge(ContextBridgeRequest model, string siteName);

        /// <summary>
        /// Deletes the context bridge.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="siteName">Name of the site.</param>
        /// <returns></returns>
        string DeleteContextBridge(ContextBridgeRequest model, string siteName);
    }
}