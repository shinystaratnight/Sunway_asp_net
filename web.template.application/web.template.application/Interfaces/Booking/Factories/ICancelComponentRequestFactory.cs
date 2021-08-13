namespace Web.Template.Application.Interfaces.Booking.Factories
{
    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// a cancel component request factory
    /// </summary>
    public interface ICancelComponentRequestFactory
    {
        /// <summary>
        /// Creates the specified cancellation model.
        /// </summary>
        /// <param name="componentCancellationModel">The component cancellation model.</param>
        /// <returns>a cancel component connect request</returns>
        iVectorConnectRequest Create(IComponentCancellationModel componentCancellationModel);
    }
}