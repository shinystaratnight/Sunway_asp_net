namespace Web.Template.Application.Booking.Models
{
    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// Cancellation model passed into cancellation requests
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Booking.Models.ICancellationModel" />
    public class CancellationModel : ICancellationModel
    {
        /// <summary>
        /// Gets or sets the booking reference.
        /// </summary>
        /// <value>
        /// The booking reference.
        /// </value>
        public string BookingReference { get; set; }

        /// <summary>
        /// Gets or sets the cost.
        /// </summary>
        /// <value>
        /// The cost.
        /// </value>
        public decimal Cost { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; }
    }
}