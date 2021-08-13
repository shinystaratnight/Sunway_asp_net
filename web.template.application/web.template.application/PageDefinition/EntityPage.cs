namespace Web.Template.Application.PageDefinition
{
    using System.Collections.Generic;

    /// <summary>
    /// Class representing a page tied to an domain entity.
    /// </summary>
    public class EntityPage
    {
        /// <summary>
        /// Gets or sets the URL tokens.
        /// </summary>
        /// <value>
        /// The URL tokens.
        /// </value>
        public List<PageEntityInformation> EntityInformations { get; set; }

        /// <summary>
        /// Gets or sets the actual page URL.
        /// </summary>
        /// <value>
        /// The actual page URL.
        /// </value>
        public string EntityType { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [user accessible].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [user accessible]; otherwise, <c>false</c>.
        /// </value>
        public bool Published { get; set; }
    }
}