namespace Web.Template.Application.Payment.Factories
{
    using System.Collections.Generic;
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Interfaces.Payment;
    using Web.Template.Application.IVectorConnect.Requests;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class Process3DSecureReturnRequestFactory.
    /// </summary>
    /// <seealso cref="Web.TemplatWeb.Template.Application.Interfaces.PaymentSecureRequestFactory" />
    public class Process3DSecureReturnRequestFactory : IProcess3DSecureReturnRequestFactory
    {
        /// <summary>
        /// The connect login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory connectLoginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Process3DSecureReturnRequestFactory"/> class.
        /// </summary>
        /// <param name="connectLoginDetailsFactory">The connect login details factory.</param>
        public Process3DSecureReturnRequestFactory(IConnectLoginDetailsFactory connectLoginDetailsFactory)
        {
            this.connectLoginDetailsFactory = connectLoginDetailsFactory;
        }

        /// <summary>
        /// Creates the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>a Connect request</returns>
        public iVectorConnectRequest Create(IProcessThreeDSecureModel model)
        {
            var paymentDetails = this.SetupConnectPaymentDetails(model.PaymentDetails);
            var request = new ivci.Process3DSecureReturnRequest()
            {
                LoginDetails = this.connectLoginDetailsFactory.Create(HttpContext.Current),
                Body = string.Empty,
                Cookies = new List<ivci.Process3DSecureReturnRequest.Cookie>(),
                FormValues = HttpUtility.UrlEncode(model.FormValues),
                Headers = new List<ivci.Process3DSecureReturnRequest.Header>(),
                Payment = paymentDetails,
                QueryString = model.QueryString,
                URL = model.Url
            };

            return request;
        }

        /// <summary>
        /// Setups the connect payment details.
        /// </summary>
        /// <param name="paymentDetails">The payment details.</param>
        /// <returns>The PaymentDetails.</returns>
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
                PaymentToken = paymentDetails.PaymentToken,
                PaymentType = paymentDetails.PaymentType.ToString(),
                Surcharge = paymentDetails.Surcharge,
                ThreeDSecureCode = paymentDetails.ThreeDSecureCode,
                TotalAmount = paymentDetails.TotalAmount,
                TransactionID = paymentDetails.TransactionID
            };
            return connectPaymentDetails;
        }
    }
}