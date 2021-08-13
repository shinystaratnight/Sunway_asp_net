namespace Web.Template.Models.Application
{
    using System.Collections.Generic;

    using Web.Template.Application.Basket.Models;

    /// <summary>
    ///     Model that is passed to the basket controller when changes to a basket are desired.
    /// </summary>
    public class ChangeBasketModel
    {
        /// <summary>
        ///     Gets or sets the basket token.
        /// </summary>
        /// <value>
        ///     The basket token.
        /// </value>
        public string BasketToken { get; set; }

        /// <summary>
        ///     Gets or sets the guest details.
        /// </summary>
        /// <value>
        ///     The guest details.
        /// </value>
        public List<BasketRoom> GuestDetails { get; set; }

        /// <summary>
        ///     Gets or sets the lead guest.
        /// </summary>
        /// <value>
        ///     The lead guest.
        /// </value>
        public LeadGuestDetails LeadGuest { get; set; }

        /// <summary>
        ///     Gets or sets the payment details.
        /// </summary>
        /// <value>
        ///     The payment details.
        /// </value>
        public PaymentDetails PaymentDetails { get; set; }

        /// <summary>
        /// Gets or sets the promo code.
        /// </summary>
        /// <value>
        /// The promo code.
        /// </value>
        public string PromoCode { get; set; }

        /// <summary>
        ///     Gets or sets the user token.
        /// </summary>
        /// <value>
        ///     The user token.
        /// </value>
        public string UserToken { get; set; }
    }
}