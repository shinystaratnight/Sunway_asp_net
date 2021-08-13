namespace Web.Template.Application.Interfaces.Booking.Services
{
    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// A service for retrieving bookings via connect
    /// </summary>
    public interface IBookingRetrieveService
    {
        /// <summary>
        /// Retrieves the booking
        /// </summary>
        /// <param name="bookingReference">The booking reference.</param>
        /// <returns>
        /// A booking retrieve return
        /// </returns>
        IBookingRetrieveReturn RetrieveBooking(string bookingReference);
    }
}