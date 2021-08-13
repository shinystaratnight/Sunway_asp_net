namespace Web.Template.Application.Interfaces.Booking.Services
{
    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// Service for talking to connect and viewing and sending booking documentation
    /// </summary>
    public interface IBookingDocumentationService
    {
        /// <summary>
        /// Retrieves the booking.
        /// </summary>
        /// <param name="docsModel">The docs model.</param>
        /// <returns>a booking documentation return</returns>
        IBookingDocumentationReturn SendDocumentation(IBookingDocumentationModel docsModel);

        /// <summary>
        /// Retrieves the booking.
        /// </summary>
        /// <param name="docsModel">The docs model.</param>
        /// <returns>
        /// a booking documentation return
        /// </returns>
        IBookingDocumentationReturn ViewDocumentation(IBookingDocumentationModel docsModel);
    }
}