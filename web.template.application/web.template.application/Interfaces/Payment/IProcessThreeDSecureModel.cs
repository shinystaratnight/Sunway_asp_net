namespace Web.Template.Application.Interfaces.Payment
{
    using Web.Template.Application.Basket.Models;

    /// <summary>
    /// Interface IProcessThreeDSecureModel
    /// </summary>
    public interface IProcessThreeDSecureModel
    {
        /// <summary>
        /// Gets or sets the form values.
        /// </summary>
        /// <value>The form values.</value>
        string FormValues { get; set; }

        /// <summary>
        /// Gets or sets the payment details.
        /// </summary>
        /// <value>The payment details.</value>
        PaymentDetails PaymentDetails { get; set; }

        /// <summary>
        /// Gets or sets the query string.
        /// </summary>
        /// <value>The query string.</value>
        string QueryString { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        string Url { get; set; }
    }
}