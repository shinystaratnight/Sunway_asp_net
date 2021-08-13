namespace Web.Template.Application.Booking.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// A model returned from a booking retrieve, contains the booking, whether it was successful and any warnings raised in the process
    /// </summary>
    /// <seealso cref="IBookingRetrieveReturn" />
    public class BookingRetrieveReturn : IBookingRetrieveReturn
    {
        /// <summary>
        /// Gets or sets the trade session.
        /// </summary>
        /// <value>
        /// The trade session.
        /// </value>
        public IBooking Booking { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [login successful].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [login successful]; otherwise, <c>false</c>.
        /// </value>
        public bool RetrieveSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        public List<string> Warnings { get; set; }
    }
}