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
    /// <seealso cref="Web.Template.Application.Interfaces.Booking.Factories.ICancelRequestFactory" />
    public class CancelRequestFactory : ICancelRequestFactory
    {
        /// <summary>
        /// The login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory loginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelRequestFactory"/> class.
        /// </summary>
        /// <param name="loginDetailsFactory">The login details factory.</param>
        public CancelRequestFactory(IConnectLoginDetailsFactory loginDetailsFactory)
        {
            this.loginDetailsFactory = loginDetailsFactory;
        }

        /// <summary>
        /// Creates the specified cancellation model.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>a connect request populated with the supplied values</returns>
        public iVectorConnectRequest Create(ICancellationModel cancellationModel)
        {
            iVectorConnectRequest searchBookingsRequest = new ivci.CancelRequest()
                                                              {
                                                                  LoginDetails = this.loginDetailsFactory.Create(HttpContext.Current), 
                                                                  BookingReference = cancellationModel.BookingReference, 
                                                                  CancellationCost = cancellationModel.Cost, 
                                                                  CancellationToken = cancellationModel.Token
                                                              };
            return searchBookingsRequest;
        }
    }
}