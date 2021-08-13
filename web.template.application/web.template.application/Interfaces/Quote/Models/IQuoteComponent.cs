namespace Web.Template.Application.Interfaces.Quote.Models
{
    /// <summary>
    /// Interface IQuoteComponent
    /// </summary>
    public interface IQuoteComponent
    {
        /// <summary>
        /// Gets or sets the type of the component.
        /// </summary>
        /// <value>The type of the component.</value>
        string ComponentType { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        string Status { get; set; }

        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        /// <value>The reference.</value>
        string Reference { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }
    }
}