namespace Web.Template.Application.Booking.Factories
{
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Booking.Factories;
    using Web.Template.Application.Interfaces.Booking.Models;
    using Web.Template.Application.IVectorConnect.Requests;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// A class for building a pre cancel connect request
    /// </summary>
    public class PreCancelRequestFactory : IPreCancelRequestFactory
    {
        /// <summary>
        /// The login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory loginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreCancelRequestFactory"/> class.
        /// </summary>
        /// <param name="loginDetailsFactory">The login details factory.</param>
        public PreCancelRequestFactory(IConnectLoginDetailsFactory loginDetailsFactory)
        {
            this.loginDetailsFactory = loginDetailsFactory;
        }

        /// <summary>
        /// Creates the specified cancellation model.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>a connect request</returns>
        public iVectorConnectRequest Create(ICancellationModel cancellationModel)
        {
            iVectorConnectRequest searchBookingsRequest = new ivci.PreCancelRequest() { LoginDetails = this.loginDetailsFactory.Create(HttpContext.Current), BookingReference = cancellationModel.BookingReference };
            return searchBookingsRequest;
        }
    }
}