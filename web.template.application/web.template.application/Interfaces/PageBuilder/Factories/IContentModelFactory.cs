namespace Web.Template.Application.Interfaces.PageBuilder.Factories
{
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    ///     Interface for content model factory
    /// </summary>
    public interface IContentModelFactory
    {
        /// <summary>
        ///     Creates the type of the model by.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <returns>A content model</returns>
        IContentModel CreateModelByType(string modelType);
    }
}