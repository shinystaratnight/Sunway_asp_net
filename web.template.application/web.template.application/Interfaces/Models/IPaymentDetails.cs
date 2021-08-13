namespace Web.Template.Application.Interfaces.Models
{
    using Web.Template.Application.Enum;

    /// <summary>
    /// Payment details required to make a booking
    /// </summary>
    public interface IPaymentDetails
    {
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>The amount.</value>
        decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the name of the card holders.
        /// </summary>
        /// <value>
        /// The name of the card holders.
        /// </value>
        string CardHoldersName { get; set; }

        /// <summary>
        /// Gets or sets the card number.
        /// </summary>
        /// <value>
        /// The card number.
        /// </value>
        string CardNumber { get; set; }

        /// <summary>
        /// Gets or sets the card type identifier.
        /// </summary>
        /// <value>
        /// The card type identifier.
        /// </value>
        int CardTypeID { get; set; }

        /// <summary>
        /// Gets or sets the expiry month.
        /// </summary>
        /// <value>
        /// The expiry month.
        /// </value>
        string ExpiryMonth { get; set; }

        /// <summary>
        /// Gets or sets the expiry year.
        /// </summary>
        /// <value>
        /// The expiry year.
        /// </value>
        string ExpiryYear { get; set; }

        /// <summary>
        /// Gets or sets the issue number.
        /// </summary>
        /// <value>
        /// The issue number.
        /// </value>
        int IssueNumber { get; set; }

        /// <summary>
        /// Gets or sets the payment token.
        /// </summary>
        /// <value>The payment token.</value>
        string PaymentToken { get; set; }

        /// <summary>
        /// Gets or sets the type of the payment.
        /// </summary>
        /// <value>The type of the payment.</value>
        PaymentType PaymentType { get; set; }

        /// <summary>
        /// Gets or sets the security number.
        /// </summary>
        /// <value>
        /// The security number.
        /// </value>
        string SecurityNumber { get; set; }

        /// <summary>
        /// Gets or sets the start month.
        /// </summary>
        /// <value>
        /// The start month.
        /// </value>
        string StartMonth { get; set; }

        /// <summary>
        /// Gets or sets the start year.
        /// </summary>
        /// <value>
        /// The start year.
        /// </value>
        string StartYear { get; set; }

        /// <summary>
        /// Gets or sets the surcharge.
        /// </summary>
        /// <value>The surcharge.</value>
        decimal Surcharge { get; set; }

        /// <summary>
        /// Gets or sets the three d secure code.
        /// </summary>
        /// <value>The three d secure code.</value>
        string ThreeDSecureCode { get; set; }

        /// <summary>
        /// Gets or sets the three d secure HTML.
        /// </summary>
        /// <value>The three d secure HTML.</value>
        string ThreeDSecureHtml { get; set; }

		/// <summary>
		/// Gets or sets the offsite payment HTML.
		/// </summary>
		/// <value>
		/// The offsite payment HTML.
		/// </value>
		string OffsitePaymentHtml { get; set; }

		/// <summary>
		/// Gets or sets the offsite payment taken.
		/// </summary>
		/// <value>
		/// The offsite payment taken.
		/// </value>
		bool OffsitePaymentTaken { get; set; }

        /// <summary>
        /// Gets or sets the total amount.
        /// </summary>
        /// <value>The total amount.</value>
        decimal TotalAmount { get; set; }
    }
}