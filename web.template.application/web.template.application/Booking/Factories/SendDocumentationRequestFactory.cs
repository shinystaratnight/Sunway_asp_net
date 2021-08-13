namespace Web.Template.Application.Booking.Factories
{
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Booking.Factories;
    using Web.Template.Application.Interfaces.Booking.Models;
    using Web.Template.Application.IVectorConnect.Requests;

    /// <summary>
    /// Class Responsible for building trade login requests.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Trade.Adaptor.ITradeLoginRequestFactory" />
    public class SendDocumentationRequestFactory : ISendDocumentationRequestFactory
    {
        /// <summary>
        /// The login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory loginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendDocumentationRequestFactory"/> class.
        /// </summary>
        /// <param name="loginDetailsFactory">The login details factory.</param>
        public SendDocumentationRequestFactory(IConnectLoginDetailsFactory loginDetailsFactory)
        {
            this.loginDetailsFactory = loginDetailsFactory;
        }

        /// <summary>
        /// Creates the specified docs model.
        /// </summary>
        /// <param name="docsModel">The docs model.</param>
        /// <returns>a connect send documentation request populated from the model passed in</returns>
        public iVectorConnectRequest Create(IBookingDocumentationModel docsModel)
        {
            iVectorConnectRequest getBookingDetailsRequest = new iVectorConnectInterface.SendDocumentationRequest()
                                                                 {
                                                                     LoginDetails = this.loginDetailsFactory.Create(HttpContext.Current), 
                                                                     BookingDocumentationID = docsModel.DocumentationId, 
                                                                     BookingReference = docsModel.BookingReference, 
                                                                     OverrideEmailAddress = docsModel.OverrideEmail,
                                                                     QuoteExternalReference = docsModel.QuoteExternalReference
                                                                 };
            return getBookingDetailsRequest;
        }
    }
}