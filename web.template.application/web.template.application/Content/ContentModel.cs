namespace Web.Template.Application.Content
{
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    ///     Content model used to return cms content to the front end
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Models.IContentModel" />
    public class ContentModel : IContentModel
    {
        /// <summary>
        ///     Gets or sets the content.
        /// </summary>
        /// <value>
        ///     The content.
        /// </value>
        public string ContentJSON { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="ContentModel" /> is success.
        /// </summary>
        /// <value>
        ///     <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }
    }
}