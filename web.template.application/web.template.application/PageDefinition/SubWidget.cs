namespace Web.Template.Application.PageDefinition
{
    using Web.Template.Application.PageDefinition.Enums;

    /// <summary>
    /// A class representing a subwidget, a widget that is part of a larger widget.
    /// </summary>
    public class SubWidget
    {
        /// <summary>
        /// Gets or sets the access.
        /// </summary>
        /// <value>
        /// The Access level that a user must have for this widget to appear on a page.
        /// </value>
        public AccessLevel Access { get; set; }

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
        /// The name of the subwidget e.g. Markdown
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>
        /// The view that this instance of the widget will be using e.g. default.
        /// </value>
        public string View { get; set; }
    }
}