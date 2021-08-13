namespace Web.Template.Application.PageDefinition
{
    /// <summary>
    /// Class to define the entity field on page definition json
    /// </summary>
    public class PageDefEntity
    {
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public PageDefEntitySource Source { get; set; }

        /// <summary>
        /// Gets or sets the type in the data source.
        /// e.g. PropertySummary or PropertyFull
        /// </summary>
        /// <value>
        /// The type of the source.
        /// </value> 
        public string SourceType { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }
    }
}