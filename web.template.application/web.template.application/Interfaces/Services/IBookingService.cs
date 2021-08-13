namespace Web.Template.Application.Interfaces.Services
{
    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// Booking service responsible for interactions with iVector bookings
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Cancels the booking.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>A cancellation Return stating whether the attempt has been successful</returns>
        ICancellationReturn CancelBooking(ICancellationModel cancellationModel);

        /// <summary>
        /// Cancels the component.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>
        /// a component cancellation return containing information about the component we have attempted to cancel and whether it was successful
        /// </returns>
        IComponentCancellationReturn CancelComponent(IComponentCancellationModel cancellationModel);

        /// <summary>
        /// Gets the booking.
        /// </summary>
        /// <param name="bookingReference">The booking reference.</param>
        /// <returns>A booking retrieve return containing the booking requested</returns>
        IBookingRetrieveReturn GetBooking(string bookingReference);

        /// <summary>
        /// a request that must be carried out prior to a cancellation request, returns the cancellation cost and a token that needs to be passed through to the next request
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>A cancellation Return, containing the cancellation cost and a token to be passed to the cancel booking</returns>
        ICancellationReturn PreCancelBooking(ICancellationModel cancellationModel);

        /// <summary>
        /// Returns information concerning the cancellation of a component.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>a component cancellation return</returns>
        IComponentCancellationReturn PreCancelComponent(IComponentCancellationModel cancellationModel);

        /// <summary>
        /// Searches the bookings.
        /// </summary>
        /// <param name="searchBookingsModel">The search bookings model.</param>
        /// <returns>A booking search return with a list of bookings that meet the search criteria</returns>
        IBookingSearchReturn SearchBookings(ISearchBookingsModel searchBookingsModel);

        /// <summary>
        /// Sends the booking documentation.
        /// </summary>
        /// <param name="docModel">The document model.</param>
        /// <returns>A booking document return, that tells you whether the attempt to send the docs has been successful</returns>
        IBookingDocumentationReturn SendBookingDocumentation(IBookingDocumentationModel docModel);

        /// <summary>
        /// Views the booking documentation.
        /// </summary>
        /// <param name="docModel">The document model.</param>
        /// <returns>A booking document return, containing a list of paths to the documents generated</returns>
        IBookingDocumentationReturn ViewBookingDocumentation(IBookingDocumentationModel docModel);

        /// <summary>
        /// Retrieves the direct debits.
        /// </summary>
        /// <returns></returns>
        IDirectDebitRetrieveReturn RetrieveDirectDebits();
    }
}