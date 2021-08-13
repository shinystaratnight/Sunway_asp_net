namespace Web.Template.Application.Interfaces.Booking.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Booking.Models;

    /// <summary>
    /// Defines an object returned from a booking search, will contain whether the search was successful, any warnings raised and the bookings themselves
    /// </summary>
    public interface IBookingSearchReturn
    {
        /// <summary>
        /// Gets or sets the bookings.
        /// </summary>
        /// <value>
        /// The bookings.
        /// </value>
        List<IBookingSearchResult> Bookings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BookingSearchReturn"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        bool Success { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        List<string> Warnings { get; set; }
    }
}