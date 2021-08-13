namespace Web.Template.Application.PageDefinition
{
    /// <summary>
    /// class representing a page tied to a custom entity.
    /// </summary>
    public class CustomEntityPage
    {
        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        public string Entity { get; set; }

        /// <summary>
        /// Gets or sets the URL prefix.
        /// </summary>
        /// <value>
        /// The URL prefix.
        /// </value>
        public string UrlPrefix { get; set; }
    }
}