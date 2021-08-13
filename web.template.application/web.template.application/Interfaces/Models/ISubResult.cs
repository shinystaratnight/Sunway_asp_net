namespace Web.Template.Application.Interfaces.Models
{
    /// <summary>
    /// Sub Result interface
    /// </summary>
    public interface ISubResult
    {
        /// <summary>
        ///     Gets or sets the booking token.
        /// </summary>
        /// <value>
        ///     The booking token, the unique identifier connect uses for the result
        /// </value>
        string BookingToken { get; set; }

        /// <summary>
        /// Gets or sets the component token.
        /// </summary>
        /// <value>
        /// The component token.
        /// </value>
        int ComponentToken { get; set; }
    }
}