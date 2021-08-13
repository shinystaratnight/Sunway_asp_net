namespace Web.Template.Application.Interfaces.Booking.Factories
{
    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// defines a class for building connect cancel requests
    /// </summary>
    public interface ICancelRequestFactory
    {
        /// <summary>
        /// Creates the specified cancellation model.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>A connect cancellation request populated with values from the model passed in.</returns>
        iVectorConnectRequest Create(ICancellationModel cancellationModel);
    }
}