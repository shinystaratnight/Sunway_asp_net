namespace Web.Template.Application.Payment.Models
{
    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Interfaces.Payment;

    /// <summary>
    /// Class ThreeDSecureRedirectModel.
    /// </summary>
    public class ThreeDSecureRedirectModel : IThreeDSecureRedirectModel
    {
        /// <summary>
        /// Gets or sets the booking reference.
        /// </summary>
        /// <value>The booking reference.</value>
        public string BookingReference { get; set; }

        /// <summary>
        /// Gets or sets the payment details.
        /// </summary>
        /// <value>The payment details.</value>
        public PaymentDetails PaymentDetails { get; set; }

        /// <summary>
        /// Gets or sets the redirect URL.
        /// </summary>
        /// <value>The redirect URL.</value>
        public string RedirectUrl { get; set; }
    }
}
