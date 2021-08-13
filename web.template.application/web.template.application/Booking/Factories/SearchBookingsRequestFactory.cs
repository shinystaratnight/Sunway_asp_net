namespace Web.Template.Application.Booking.Factories
{
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Booking.Factories;
    using Web.Template.Application.Interfaces.Booking.Models;
    using Web.Template.Application.IVectorConnect.Requests;

    /// <summary>
    /// class responsible for building search booking requests
    /// </summary>
    /// <seealso cref="IGetBookingDetailsRequestFactory" />
    public class SearchBookingsRequestFactory : ISearchBookingsRequestFactory
    {
        /// <summary>
        /// The login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory loginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchBookingsRequestFactory"/> class.
        /// </summary>
        /// <param name="loginDetailsFactory">The login details factory.</param>
        public SearchBookingsRequestFactory(IConnectLoginDetailsFactory loginDetailsFactory)
        {
            this.loginDetailsFactory = loginDetailsFactory;
        }



        /// <summary>
        /// Creates the specified search bookings model.
        /// </summary>
        /// <param name="searchBookingsModel">The search bookings model.</param>
        /// <returns>
        /// a connect request
        /// </returns>
        public iVectorConnectRequest Create(ISearchBookingsModel searchBookingsModel)
        {
            iVectorConnectRequest searchBookingsRequest = new iVectorConnectInterface.SearchBookingsRequest()
                                                              {
                                                                  LoginDetails = this.loginDetailsFactory.Create(HttpContext.Current),
                                                                  TradeReference = searchBookingsModel.TradeReference,
                                                                  TradeContactID = searchBookingsModel.TradeContactId,
                                                                  TradeID =  searchBookingsModel.TradeId
                                                              };
            return searchBookingsRequest;
        }
    }
}