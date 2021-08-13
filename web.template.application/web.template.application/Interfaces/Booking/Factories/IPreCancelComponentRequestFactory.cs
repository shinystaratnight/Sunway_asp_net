namespace Web.Template.Application.Interfaces.Booking.Factories
{
    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// Defines a class that builds connect pre cancel component requests.
    /// </summary>
    public interface IPreCancelComponentRequestFactory
    {
        /// <summary>
        /// Creates the specified cancellation model.
        /// </summary>
        /// <param name="componentCancellationModel">The component cancellation model.</param>
        /// <returns>
        /// A connect pre cancel component request with values populated by the passed in model
        /// </returns>
        iVectorConnectRequest Create(IComponentCancellationModel componentCancellationModel);
    }
}