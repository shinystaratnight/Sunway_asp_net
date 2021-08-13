namespace Web.Template.Application.Interfaces.Booking.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// A model returned from a booking retrieve, contains the booking, whether it was successful and any warnings raised in the process
    /// </summary>
    public interface IBookingRetrieveReturn
    {
        /// <summary>
        /// Gets or sets the trade session.
        /// </summary>
        /// <value>
        /// The trade session.
        /// </value>
        IBooking Booking { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [login successful].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [login successful]; otherwise, <c>false</c>.
        /// </value>
        bool RetrieveSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        List<string> Warnings { get; set; }
    }
}