namespace Web.Template.Application.Basket.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Payment details class
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Models.IPaymentDetails" />
    public class PaymentDetails : IPaymentDetails
    {
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>The amount.</value>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the name of the card holders.
        /// </summary>
        /// <value>
        /// The name of the card holders.
        /// </value>
        public string CardHoldersName { get; set; }

        /// <summary>
        /// Gets or sets the card number.
        /// </summary>
        /// <value>
        /// The card number.
        /// </value>
        public string CardNumber { get; set; }

        /// <summary>
        /// Gets or sets the card type identifier.
        /// </summary>
        /// <value>
        /// The card type identifier.
        /// </value>
        public int CardTypeID { get; set; }

        /// <summary>
        /// Gets or sets the expiry month.
        /// </summary>
        /// <value>
        /// The expiry month.
        /// </value>
        public string ExpiryMonth { get; set; }

        /// <summary>
        /// Gets or sets the expiry year.
        /// </summary>
        /// <value>
        /// The expiry year.
        /// </value>
        public string ExpiryYear { get; set; }

        /// <summary>
        /// Gets or sets the issue number.
        /// </summary>
        /// <value>
        /// The issue number.
        /// </value>
        public int IssueNumber { get; set; }

        /// <summary>
        /// Gets or sets the payment token.
        /// </summary>
        /// <value>The payment token.</value>
        public string PaymentToken { get; set; }

        /// <summary>
        /// Gets or sets the type of the payment.
        /// </summary>
        /// <value>The type of the payment.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// Gets or sets the security number.
        /// </summary>
        /// <value>
        /// The security number.
        /// </value>
        public string SecurityNumber { get; set; }

        /// <summary>
        /// Gets or sets the start month.
        /// </summary>
        /// <value>
        /// The start month.
        /// </value>
        public string StartMonth { get; set; }

        /// <summary>
        /// Gets or sets the start year.
        /// </summary>
        /// <value>
        /// The start year.
        /// </value>
        public string StartYear { get; set; }

        /// <summary>
        /// Gets or sets the surcharge.
        /// </summary>
        /// <value>The surcharge.</value>
        public decimal Surcharge { get; set; }

        /// <summary>
        /// Gets or sets the three d secure code.
        /// </summary>
        /// <value>The three d secure code.</value>
        public string ThreeDSecureCode { get; set; }

        /// <summary>
        /// Gets or sets the three d secure HTML.
        /// </summary>
        /// <value>The three d secure HTML.</value>
        public string ThreeDSecureHtml { get; set; }


        /// <summary>
        /// Gets or sets the offsite payment HTML.
        /// </summary>
        /// <value>
        /// The offsite payment HTML.
        /// </value>
        public string OffsitePaymentHtml { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [offsite payment taken].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [offsite payment taken]; otherwise, <c>false</c>.
        /// </value>
        public bool OffsitePaymentTaken { get; set; }

        /// <summary>
        /// Gets or sets the total amount.
        /// </summary>
        /// <value>The total amount.</value>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets of sets the transaction ID
        /// </summary>
        public string TransactionID { get; set; }
    }
}