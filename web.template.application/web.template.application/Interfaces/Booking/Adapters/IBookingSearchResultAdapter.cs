namespace Web.Template.Application.Interfaces.Booking.Adapters
{
    using iVectorConnectInterface;

    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// Adapts a connect booking search result into our domain object
    /// </summary>
    public interface IBookingSearchResultAdapter
    {
        /// <summary>
        /// Creates the booking search result.
        /// </summary>
        /// <param name="ivcBooking">The connect booking.</param>
        /// <returns>
        /// a booking search result
        /// </returns>
        IBookingSearchResult CreateBookingSearchResult(SearchBookingsResponse.Booking ivcBooking);
    }
}