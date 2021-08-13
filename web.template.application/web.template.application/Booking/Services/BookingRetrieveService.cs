namespace Web.Template.Application.Booking.Adapters
{
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Booking.Adapters;
    using Web.Template.Application.Interfaces.Booking.Factories;
    using Web.Template.Application.Interfaces.Booking.Models;
    using Web.Template.Application.Interfaces.Booking.Services;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Net.IVectorConnect;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class responsible for talking to connect regarding retrieving bookings
    /// </summary>
    /// <seealso cref="IBookingRetrieveService" />
    public class BookingRetrieveService : IBookingRetrieveService
    {
        /// <summary>
        /// The booking adapter
        /// </summary>
        private readonly IBookingAdapter bookingAdapter;

        /// <summary>
        /// The trade login request factory
        /// </summary>
        private readonly IGetBookingDetailsRequestFactory bookingDetailsRequestFactory;

        /// <summary>
        /// The booking retrieve return
        /// </summary>
        private readonly IBookingRetrieveReturn bookingRetrieveReturn;

        /// <summary>
        /// The connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory connectRequestFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingRetrieveService"/> class.
        /// </summary>
        /// <param name="connectRequestFactory">The connect request factory.</param>
        /// <param name="bookingDetailsRequestFactory">The booking details request factory.</param>
        /// <param name="bookingRetrieveReturn">The booking retrieve return.</param>
        /// <param name="bookingAdapter">The booking adapter.</param>
        public BookingRetrieveService(IIVectorConnectRequestFactory connectRequestFactory, IGetBookingDetailsRequestFactory bookingDetailsRequestFactory, IBookingRetrieveReturn bookingRetrieveReturn, IBookingAdapter bookingAdapter)
        {
            this.connectRequestFactory = connectRequestFactory;
            this.bookingDetailsRequestFactory = bookingDetailsRequestFactory;
            this.bookingRetrieveReturn = bookingRetrieveReturn;
            this.bookingAdapter = bookingAdapter;
        }

        /// <summary>
        /// Retrieves the booking
        /// </summary>
        /// <param name="bookingReference">a booking reference</param>
        /// <returns>
        /// A booking retrieve return
        /// </returns>
        public IBookingRetrieveReturn RetrieveBooking(string bookingReference)
        {
            this.bookingRetrieveReturn.RetrieveSuccessful = false;

            iVectorConnectRequest requestBody = this.bookingDetailsRequestFactory.Create(bookingReference);
            this.bookingRetrieveReturn.Warnings = requestBody.Validate();

            if (this.bookingRetrieveReturn.Warnings.Count == 0)
            {
                this.GetResponse(requestBody);
            }

            return this.bookingRetrieveReturn;
        }

        /// <summary>
        /// Gets the response, and passes it on to the bookingAdaptor
        /// </summary>
        /// <param name="requestBody">The request body.</param>
        private void GetResponse(iVectorConnectRequest requestBody)
        {
            IIVectorConnectRequest ivcRequest = this.connectRequestFactory.Create(requestBody, HttpContext.Current);
            ivci.GetBookingDetailsResponse bookingDetailsResponse = ivcRequest.Go<ivci.GetBookingDetailsResponse>();

            this.bookingRetrieveReturn.RetrieveSuccessful = bookingDetailsResponse.ReturnStatus.Success;
            this.bookingRetrieveReturn.Warnings.AddRange(bookingDetailsResponse.ReturnStatus.Exceptions);

            if (this.bookingRetrieveReturn.Warnings.Count == 0)
            {
                this.bookingRetrieveReturn.Booking = this.bookingAdapter.CreateBookingFromGetBookingDetailsResponse(bookingDetailsResponse);
            }
        }
    }
}