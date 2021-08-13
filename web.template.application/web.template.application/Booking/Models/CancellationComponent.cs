namespace Web.Template.Application.Booking.Models
{
    /// <summary>
    /// a component that we wish to cancel
    /// </summary>
    public class CancellationComponent
    {
        /// <summary>
        /// Gets or sets the cancellation cost.
        /// </summary>
        /// <value>
        /// The cancellation cost.
        /// </value>
        public decimal CancellationCost { get; set; }

        /// <summary>
        /// Gets or sets the component booking identifier.
        /// </summary>
        /// <value>
        /// The component booking identifier.
        /// </value>
        public int ComponentBookingId { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }
    }
}