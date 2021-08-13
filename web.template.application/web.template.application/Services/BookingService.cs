namespace Web.Template.Application.Services
{
    using Web.Template.Application.Interfaces.Booking.Models;
    using Web.Template.Application.Interfaces.Booking.Services;
    using Web.Template.Application.Interfaces.Services;

    /// <summary>
    /// Booking service used to carry out operations on a booking
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Services.IBookingService" />
    public class BookingService : IBookingService
    {
        /// <summary>
        /// The booking documentation service
        /// </summary>
        private readonly IBookingDocumentationService bookingDocumentationService;

        /// <summary>
        /// The booking retrieve service
        /// </summary>
        private readonly IBookingRetrieveService bookingRetrieveService;

        /// <summary>
        /// The booking search service
        /// </summary>
        private readonly IBookingSearchService bookingSearchService;

        /// <summary>
        /// The cancellation service
        /// </summary>
        private readonly ICancellationService cancellationService;

        /// <summary>
        /// The direct debit service
        /// </summary>
        private readonly IDirectDebitRetrieveService directDebitService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingService" /> class.
        /// </summary>
        /// <param name="bookingRetrieveService">The booking retrieve service.</param>
        /// <param name="bookingSearchService">The booking search service.</param>
        /// <param name="bookingDocumentationService">The booking documentation service.</param>
        /// <param name="cancellationService">The cancellation service.</param>
        /// <param name="directDebitService">The direct debit service.</param>
        public BookingService(IBookingRetrieveService bookingRetrieveService,
                                IBookingSearchService bookingSearchService,
                                IBookingDocumentationService bookingDocumentationService,
                                ICancellationService cancellationService,
                                IDirectDebitRetrieveService directDebitService)
        {
            this.bookingRetrieveService = bookingRetrieveService;
            this.bookingSearchService = bookingSearchService;
            this.bookingDocumentationService = bookingDocumentationService;
            this.cancellationService = cancellationService;
            this.directDebitService = directDebitService;
        }

        /// <summary>
        /// Cancels the booking.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>
        /// A cancellation Return stating whether the attempt has been successful
        /// </returns>
        public ICancellationReturn CancelBooking(ICancellationModel cancellationModel)
        {
            ICancellationReturn cancellationReturn = this.cancellationService.RequestBookingCancellation(cancellationModel);
            return cancellationReturn;
        }

        /// <summary>
        /// Cancels the booking.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>A component Cancellation model</returns>
        public IComponentCancellationReturn CancelComponent(IComponentCancellationModel cancellationModel)
        {
            IComponentCancellationReturn cancellationReturn = this.cancellationService.CancelComponents(cancellationModel);
            return cancellationReturn;
        }

        /// <summary>
        /// Gets the booking.
        /// </summary>
        /// <param name="bookingReference">The booking reference.</param>
        /// <returns>A booking retrieve return</returns>
        public IBookingRetrieveReturn GetBooking(string bookingReference)
        {
            IBookingRetrieveReturn bookingReturn = this.bookingRetrieveService.RetrieveBooking(bookingReference);
            return bookingReturn;
        }

        /// <summary>
        ///  a request that comes before the main cancel booking request.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>
        /// A cancellation Return, containing the cancellation cost and a token to be passed to the cancel booking
        /// </returns>
        public ICancellationReturn PreCancelBooking(ICancellationModel cancellationModel)
        {
            ICancellationReturn cancellationReturn = this.cancellationService.PreCancelBooking(cancellationModel);
            return cancellationReturn;
        }

        /// <summary>
        /// a request that comes before the main cancel component request.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>A cancellation return with details such as the amount it will cost to cancel the component and a token to be passed to the next request</returns>
        public IComponentCancellationReturn PreCancelComponent(IComponentCancellationModel cancellationModel)
        {
            IComponentCancellationReturn cancellationReturn = this.cancellationService.PreCancelComponents(cancellationModel);
            return cancellationReturn;
        }

        /// <summary>
        /// Searches the bookings.
        /// </summary>
        /// <param name="searchBookingsModel">The search bookings model.</param>
        /// <returns>
        /// A booking search return with a list of bookings that meet the search criteria
        /// </returns>
        public IBookingSearchReturn SearchBookings(ISearchBookingsModel searchBookingsModel)
        {
            IBookingSearchReturn bookingSearchReturn = this.bookingSearchService.SearchBookings(searchBookingsModel);

            return bookingSearchReturn;
        }

        /// <summary>
        /// Sends the booking documentation.
        /// </summary>
        /// <param name="docModel">The document model.</param>
        /// <returns>
        /// A booking document return, that tells you whether the attempt to send the docs has been successful
        /// </returns>
        public IBookingDocumentationReturn SendBookingDocumentation(IBookingDocumentationModel docModel)
        {
            IBookingDocumentationReturn docReturn = this.bookingDocumentationService.SendDocumentation(docModel);
            return docReturn;
        }

        /// <summary>
        /// Views the booking documentation.
        /// </summary>
        /// <param name="docModel">The document model.</param>
        /// <returns>
        /// A booking document return, containing a list of paths to the documents generated
        /// </returns>
        public IBookingDocumentationReturn ViewBookingDocumentation(IBookingDocumentationModel docModel)
        {
            IBookingDocumentationReturn docReturn = this.bookingDocumentationService.ViewDocumentation(docModel);
            return docReturn;
        }

        /// <summary>
        /// Retrieves the direct debits.
        /// </summary>
        /// <returns></returns>
        public IDirectDebitRetrieveReturn RetrieveDirectDebits()
        {
            IDirectDebitRetrieveReturn directDebitReturn = this.directDebitService.RetrieveDirectDebits();

            return directDebitReturn;
        }
    }
}