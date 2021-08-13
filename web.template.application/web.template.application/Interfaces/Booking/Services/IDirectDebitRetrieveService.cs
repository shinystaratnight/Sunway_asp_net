namespace Web.Template.Application.Interfaces.Booking.Services
{
    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// A service for retrieving bookings via connect
    /// </summary>
    public interface IDirectDebitRetrieveService
    {
        /// <summary>
        /// Retrieves the direct debits.
        /// </summary>
        /// <returns>An IDirectDebitRetrieveReturn</returns>
        IDirectDebitRetrieveReturn RetrieveDirectDebits();
    }
}