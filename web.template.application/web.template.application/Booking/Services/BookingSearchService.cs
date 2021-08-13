namespace Web.Template.Application.Booking.Services
{
    using System.Collections.Generic;
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
    public class BookingSearchService : IBookingSearchService
    {
        /// <summary>
        /// The booking adapter
        /// </summary>
        private readonly IBookingSearchResultAdapter bookingSearchResultAdapter;

        /// <summary>
        /// The booking retrieve return
        /// </summary>
        private readonly IBookingSearchReturn bookingSearchReturn;

        /// <summary>
        /// The connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory connectRequestFactory;

        /// <summary>
        /// The trade login request factory
        /// </summary>
        private readonly ISearchBookingsRequestFactory searchBookingsRequestFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingSearchService"/> class.
        /// </summary>
        /// <param name="connectRequestFactory">The connect request factory.</param>
        /// <param name="searchBookingsRequestFactory">The search bookings request factory.</param>
        /// <param name="bookingSearchReturn">The booking search return.</param>
        /// <param name="bookingSearchResultAdapter">The booking search result adapter.</param>
        public BookingSearchService(
            IIVectorConnectRequestFactory connectRequestFactory, 
            ISearchBookingsRequestFactory searchBookingsRequestFactory, 
            IBookingSearchReturn bookingSearchReturn, 
            IBookingSearchResultAdapter bookingSearchResultAdapter)
        {
            this.connectRequestFactory = connectRequestFactory;
            this.searchBookingsRequestFactory = searchBookingsRequestFactory;
            this.bookingSearchReturn = bookingSearchReturn;
            this.bookingSearchResultAdapter = bookingSearchResultAdapter;
        }

        /// <summary>
        /// Gets the response, and passes it on to the bookingAdaptor
        /// </summary>
        /// <param name="requestBody">The request body.</param>
        public void GetResponse(iVectorConnectRequest requestBody)
        {
            IIVectorConnectRequest ivcRequest = this.connectRequestFactory.Create(requestBody, HttpContext.Current);
            ivci.SearchBookingsResponse searchBookingsResponse = ivcRequest.Go<ivci.SearchBookingsResponse>();

            this.bookingSearchReturn.Success = searchBookingsResponse.ReturnStatus.Success;
            this.bookingSearchReturn.Warnings.AddRange(searchBookingsResponse.ReturnStatus.Exceptions);

            if (this.bookingSearchReturn.Warnings.Count == 0)
            {
                if (searchBookingsResponse.Bookings.Count > 0)
                {
                    this.bookingSearchReturn.Bookings = new List<IBookingSearchResult>();
                    foreach (ivci.SearchBookingsResponse.Booking booking in searchBookingsResponse.Bookings)
                    {
                        this.bookingSearchReturn.Bookings.Add(this.bookingSearchResultAdapter.CreateBookingSearchResult(booking));
                    }
                }
                else
                {
                    this.bookingSearchReturn.Warnings.Add("Search Returned no bookings");
                }
            }
        }

        /// <summary>
        /// Retrieves the booking.
        /// </summary>
        /// <param name="searchbookingsModel">The search bookings model.</param>
        /// <returns>A booking search return.</returns>
        public IBookingSearchReturn SearchBookings(ISearchBookingsModel searchbookingsModel)
        {
            this.bookingSearchReturn.Success = false;

            iVectorConnectRequest requestBody = this.searchBookingsRequestFactory.Create(searchbookingsModel);
            this.bookingSearchReturn.Warnings = requestBody.Validate();

            if (this.bookingSearchReturn.Warnings.Count == 0)
            {
                this.GetResponse(requestBody);
            }

            return this.bookingSearchReturn;
        }
    }
}