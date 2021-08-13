namespace Web.Template.Application.Payment.Models
{
    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Interfaces.Payment;

    /// <summary>
    /// Class ProcessThreeDSecureModel.
    /// </summary>
    public class ProcessThreeDSecureModel : IProcessThreeDSecureModel
    {
        /// <summary>
        /// Gets or sets the form values.
        /// </summary>
        /// <value>The form values.</value>
        public string FormValues { get; set; }

        /// <summary>
        /// Gets or sets the payment details.
        /// </summary>
        /// <value>The payment details.</value>
        public PaymentDetails PaymentDetails { get; set; }

        /// <summary>
        /// Gets or sets the query string.
        /// </summary>
        /// <value>The query string.</value>
        public string QueryString { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }
    }
}
