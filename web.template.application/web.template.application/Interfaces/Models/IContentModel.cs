namespace Web.Template.Application.Interfaces.Models
{
    /// <summary>
    ///     Interface for content models returned by the content repository
    /// </summary>
    public interface IContentModel
    {
        /// <summary>
        ///     Gets or sets the content.
        /// </summary>
        /// <value>
        ///     The content.
        /// </value>
        string ContentJSON { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IContentModel" /> is success.
        /// </summary>
        /// <value>
        ///     <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        bool Success { get; set; }
    }
}