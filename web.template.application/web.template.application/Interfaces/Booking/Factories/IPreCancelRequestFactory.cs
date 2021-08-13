namespace Web.Template.Application.Interfaces.Booking.Factories
{
    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// Defines a class that builds connect pre cancel requests.
    /// </summary>
    public interface IPreCancelRequestFactory
    {
        /// <summary>
        /// Creates the specified cancellation model.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>A connect pre cancel request with values populated from the model passed in.</returns>
        iVectorConnectRequest Create(ICancellationModel cancellationModel);
    }
}