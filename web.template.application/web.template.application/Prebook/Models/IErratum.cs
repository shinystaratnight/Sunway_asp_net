namespace Web.Template.Application.Prebook.Models
{
    using Web.Template.Application.Enum;

    /// <summary>
    /// interface defining a single piece of Errata
    /// </summary>
    public interface IErratum
    {
        /// <summary>
        /// Gets or sets the type of the component.
        /// </summary>
        /// <value>The type of the component.</value>
        ComponentType ComponentType { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        string Subject { get; set; }
    }
}