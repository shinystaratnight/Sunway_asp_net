namespace Web.Booking.Models.Application
{
    using System.Collections.Generic;

    using Web.Template.Application.Basket.BasketModels;
    using Web.Template.Application.Basket.Models;

    /// <summary>
    /// Class BasketBookModel.
    /// </summary>
    public class BasketBookModel
    {
        /// <summary>
        ///     Gets or sets the guest details.
        /// </summary>
        /// <value>
        ///     The guest details.
        /// </value>
        public List<BasketRoom> GuestDetails { get; set; }

        /// <summary>
        /// Gets or sets the hotel requests.
        /// </summary>
        /// <value>
        /// The hotel requests.
        /// </value>
        public string HotelRequest { get; set; }

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
        /// Gets or sets the trade reference.
        /// </summary>
        /// <value>
        /// The trade reference.
        /// </value>
        public string TradeReference { get; set; }
    }
}