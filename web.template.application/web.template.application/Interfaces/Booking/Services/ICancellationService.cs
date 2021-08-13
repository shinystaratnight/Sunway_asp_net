namespace Web.Template.Application.Interfaces.Booking.Services
{
    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// cancellation service responsible for handling all requests to cancel bookings and components
    /// </summary>
    public interface ICancellationService
    {
        /// <summary>
        /// Cancels the components.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>a component cancellation return</returns>
        IComponentCancellationReturn CancelComponents(IComponentCancellationModel cancellationModel);

        /// <summary>
        /// The request that comes before the main cancel booking.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>a cancellation return</returns>
        ICancellationReturn PreCancelBooking(ICancellationModel cancellationModel);

        /// <summary>
        /// The request that comes before the main cancel component.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>a component cancellation return</returns>
        IComponentCancellationReturn PreCancelComponents(IComponentCancellationModel cancellationModel);

        /// <summary>
        /// Requests the booking cancellation.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>a cancellation return</returns>
        ICancellationReturn RequestBookingCancellation(ICancellationModel cancellationModel);
    }
}