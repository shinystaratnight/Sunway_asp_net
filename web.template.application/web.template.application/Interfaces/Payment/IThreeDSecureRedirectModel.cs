namespace Web.Template.Application.Interfaces.Payment
{
    using Web.Template.Application.Basket.Models;

    /// <summary>
    /// Interface IThreeDSecureRedirectModel
    /// </summary>
    public interface IThreeDSecureRedirectModel
    {
        /// <summary>
        /// Gets or sets the booking reference.
        /// </summary>
        /// <value>The booking reference.</value>
        string BookingReference { get; set; }

        /// <summary>
        /// Gets or sets the payment details.
        /// </summary>
        /// <value>The payment details.</value>
        PaymentDetails PaymentDetails { get; set; }

        /// <summary>
        /// Gets or sets the redirect URL.
        /// </summary>
        /// <value>The redirect URL.</value>
        string RedirectUrl { get; set; }
    }
}