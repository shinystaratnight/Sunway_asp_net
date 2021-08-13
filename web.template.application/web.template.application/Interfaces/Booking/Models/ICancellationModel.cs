namespace Web.Template.Application.Interfaces.Booking.Models
{
    /// <summary>
    ///  Cancellation model passed into cancellation requests
    /// </summary>
    public interface ICancellationModel
    {
        /// <summary>
        /// Gets or sets the booking reference.
        /// </summary>
        /// <value>
        /// The booking reference.
        /// </value>
        string BookingReference { get; set; }

        /// <summary>
        /// Gets or sets the cost.
        /// </summary>
        /// <value>
        /// The cost.
        /// </value>
        decimal Cost { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        string Token { get; set; }
    }
}