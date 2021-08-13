namespace Web.Template.Application.Interfaces.Booking.Factories
{
    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// Defines a class that builds connect search booking requests.
    /// </summary>
    public interface ISearchBookingsRequestFactory
    {
        /// <summary>
        /// Creates the specified search bookings model.
        /// </summary>
        /// <param name="searchBookingsModel">The search bookings model.</param>
        /// <returns>
        /// a connect request
        /// </returns>
        iVectorConnectRequest Create(ISearchBookingsModel searchBookingsModel);
    }
}