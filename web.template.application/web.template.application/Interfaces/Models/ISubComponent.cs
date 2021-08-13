namespace Web.Template.Application.Interfaces.Models
{
    /// <summary>
    /// Interface representing a subcomponent
    /// </summary>
    public interface ISubComponent
    {
        /// <summary>
        /// Gets or sets the booking token.
        /// </summary>
        /// <value>
        /// The booking token.
        /// </value>
        string BookingToken { get; set; }

        /// <summary>
        /// Gets or sets the component token.
        /// </summary>
        /// <value>
        /// The component token.
        /// </value>
        int ComponentToken { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        /// <value>
        /// The total price.
        /// </value>
        decimal TotalPrice { get; set; }
    }
}