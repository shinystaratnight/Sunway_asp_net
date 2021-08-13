namespace Web.Template.Application.Interfaces.Booking.Services
{
    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// Service responsible for talking to connects booking search
    /// </summary>
    public interface IBookingSearchService
    {
        /// <summary>
        /// Retrieves the booking.
        /// </summary>
        /// <param name="searchbookingsModel">The search bookings model.</param>
        /// <returns>
        /// A booking search return containing the bookings we searched for
        /// </returns>
        IBookingSearchReturn SearchBookings(ISearchBookingsModel searchbookingsModel);
    }
}