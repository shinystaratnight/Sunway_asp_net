namespace Web.Template.Application.Booking.Factories
{
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Booking.Factories;
    using Web.Template.Application.IVectorConnect.Requests;

    /// <summary>
    /// Class Responsible for building trade login requests.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Trade.Adaptor.ITradeLoginRequestFactory" />
    public class GetBookingDetailsRequestFactory : IGetBookingDetailsRequestFactory
    {
        /// <summary>
        /// The login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory loginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetBookingDetailsRequestFactory" /> class.
        /// </summary>
        /// <param name="loginDetailsFactory">The login details factory.</param>
        public GetBookingDetailsRequestFactory(IConnectLoginDetailsFactory loginDetailsFactory)
        {
            this.loginDetailsFactory = loginDetailsFactory;
        }

        /// <summary>
        /// Creates the specified booking reference.
        /// </summary>
        /// <param name="bookingReference">The booking reference.</param>
        /// <returns>a connect request</returns>
        public iVectorConnectRequest Create(string bookingReference)
        {
            iVectorConnectRequest getBookingDetailsRequest = new iVectorConnectInterface.GetBookingDetailsRequest() { LoginDetails = this.loginDetailsFactory.Create(HttpContext.Current), BookingReference = bookingReference };
            return getBookingDetailsRequest;
        }
    }
}