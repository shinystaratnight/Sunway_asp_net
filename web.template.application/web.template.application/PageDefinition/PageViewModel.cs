namespace Web.Template.Application.PageDefinition
{
    using System.Collections.Generic;

    /// <summary>
    /// The Page View Model
    /// </summary>
    public class PageViewModel
    {
        /// <summary>
        /// Gets or sets the entity information.
        /// </summary>
        /// <value>
        /// List of entity information.
        /// </value>
        public List<PageEntityInformation> EntityInformations { get; set; }

        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public string EntityType { get; set; }

        /// <summary>
        /// Gets or sets the meta information.
        /// </summary>
        /// <value>
        /// The meta information.
        /// </value>
        public PageMetaInformation MetaInformation { get; set; }

        /// <summary>
        /// Gets or sets the name of the page.
        /// </summary>
        /// <value>
        /// The name of the page.
        /// </value>
        public string PageName { get; set; }

        /// <summary>
        /// Gets or sets the page URL.
        /// </summary>
        /// <value>
        /// The page URL.
        /// </value>
        public string PageURL { get; set; }

        /// <summary>
        /// Gets or sets the widgets.
        /// </summary>
        /// <value>
        /// The widgets.
        /// </value>
        public List<Widget> Widgets { get; set; }
    }
}