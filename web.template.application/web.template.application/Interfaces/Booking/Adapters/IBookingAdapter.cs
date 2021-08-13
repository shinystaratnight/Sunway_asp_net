namespace Web.Template.Application.Interfaces.Booking.Adapters
{
    using Web.Template.Application.Interfaces.Booking.Models;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// An interface defining a class that converts into a booking
    /// </summary>
    public interface IBookingAdapter
    {
        /// <summary>
        /// Creates the booking from get booking details response.
        /// </summary>
        /// <param name="bookingDetailsResponse">The booking details response.</param>
        /// <returns>A booking</returns>
        IBooking CreateBookingFromGetBookingDetailsResponse(ivci.GetBookingDetailsResponse bookingDetailsResponse);
    }
}