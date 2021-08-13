namespace Web.Template.Application.PageDefinition
{
    using System.Collections.Generic;

    /// <summary>
    /// A class representing a section of a template page.
    /// </summary>
    public class Section
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name of the section E.g. Sidebar
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the widgets.
        /// </summary>
        /// <value>
        /// The collection of widgets that will appear in this section e.g. Search Tool
        /// </value>
        public ICollection<Widget> Widgets { get; set; }
    }
}