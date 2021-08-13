namespace Web.Template.Application.Booking.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// Object returned from a booking search, will contain whether the search was successful, any warnings raised and the bookings themselves
    /// </summary>
    public class BookingSearchReturn : IBookingSearchReturn
    {
        /// <summary>
        /// Gets or sets the bookings.
        /// </summary>
        /// <value>
        /// The bookings.
        /// </value>
        public List<IBookingSearchResult> Bookings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BookingSearchReturn"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        public List<string> Warnings { get; set; }
    }
}