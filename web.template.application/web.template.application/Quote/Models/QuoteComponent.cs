namespace Web.Template.Application.Quote.Models
{
    using Web.Template.Application.Interfaces.Quote;
    using Web.Template.Application.Interfaces.Quote.Models;

    /// <summary>
    /// Class QuoteComponent.
    /// </summary>
    public class QuoteComponent : IQuoteComponent
    {
        /// <summary>
        /// Gets or sets the type of the component.
        /// </summary>
        /// <value>The type of the component.</value>
        public string ComponentType { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        /// <value>The reference.</value>
        public string Reference { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
    }
}
