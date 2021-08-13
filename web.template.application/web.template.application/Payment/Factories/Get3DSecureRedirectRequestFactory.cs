namespace Web.Template.Application.Payment.Factories
{
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Interfaces.Payment;
    using Web.Template.Application.IVectorConnect.Requests;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class Get3DSecureRedirectRequestFactory.
    /// </summary>
    public class Get3DSecureRedirectRequestFactory : IGet3DSecureRedirectRequestFactory
    {
        /// <summary>
        /// The connect login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory connectLoginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Get3DSecureRedirectRequestFactory"/> class.
        /// </summary>
        /// <param name="connectLoginDetailsFactory">The connect login details factory.</param>
        public Get3DSecureRedirectRequestFactory(IConnectLoginDetailsFactory connectLoginDetailsFactory)
        {
            this.connectLoginDetailsFactory = connectLoginDetailsFactory;
        }

        /// <summary>
        /// Creates the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The iVectorConnectRequest.</returns>
        public iVectorConnectRequest Create(IThreeDSecureRedirectModel model)
        {
            var paymentDetails = this.SetupConnectPaymentDetails(model.PaymentDetails);

            var request = new ivci.Get3DSecureRedirectRequest()
            {
                BookingReference = model.BookingReference,
                LoginDetails = this.connectLoginDetailsFactory.Create(HttpContext.Current),
                Payment = paymentDetails,
                RedirectURL = model.RedirectUrl,
            };

            return request;
        }

        /// <summary>
        /// Setups the connect payment details.
        /// </summary>
        /// <param name="paymentDetails">The payment details.</param>
        /// <returns>The Payment Details</returns>
        private ivci.Support.PaymentDetails SetupConnectPaymentDetails(PaymentDetails paymentDetails)
        {
            var connectPaymentDetails = new ivci.Support.PaymentDetails()
            {
                Amount = paymentDetails.Amount,
                CCCardHoldersName = paymentDetails.CardHoldersName,
                CCCardNumber = paymentDetails.CardNumber,
                CCCardTypeID = paymentDetails.CardTypeID,
                CCExpireMonth = paymentDetails.ExpiryMonth,
                CCExpireYear = paymentDetails.ExpiryYear,
                CCIssueNumber = paymentDetails.IssueNumber,
                CCSecurityCode = paymentDetails.SecurityNumber,
                CCStartMonth = paymentDetails.StartMonth,
                CCStartYear = paymentDetails.StartYear,
                PaymentType = paymentDetails.PaymentType.ToString(),
                Surcharge = paymentDetails.Surcharge,
                TotalAmount = paymentDetails.TotalAmount,
                TransactionID = paymentDetails.TransactionID
            };
            return connectPaymentDetails;
        }
    }
}