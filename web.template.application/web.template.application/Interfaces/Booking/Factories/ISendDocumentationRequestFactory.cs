namespace Web.Template.Application.Interfaces.Booking.Factories
{
    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// Defines a factory for building connect send documentation requests
    /// </summary>
    public interface ISendDocumentationRequestFactory
    {
        /// <summary>
        /// Creates the specified docs model.
        /// </summary>
        /// <param name="docsModel">The docs model.</param>
        /// <returns>a connect send documentation request populated from the model passed in</returns>
        iVectorConnectRequest Create(IBookingDocumentationModel docsModel);
    }
}