namespace Web.Template.Application.Interfaces.Booking.Factories
{
    using iVectorConnectInterface.Interfaces;

    /// <summary>
    /// a factory for building a booking details request
    /// </summary>
    public interface IGetBookingDetailsRequestFactory
    {
        /// <summary>
        /// Creates the specified booking reference.
        /// </summary>
        /// <param name="bookingReference">The booking reference.</param>
        /// <returns>a connect request</returns>
        iVectorConnectRequest Create(string bookingReference);
    }
}