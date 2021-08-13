namespace Web.Template.Application.Interfaces.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Prebook.Models;

    /// <summary>
    /// Interface representing the basket
    /// </summary>
    public interface IBasket
    {
        /// <summary>
        /// Gets or sets the adjustments.
        /// </summary>
        /// <value>
        /// The adjustments.
        /// </value>
        List<IAdjustment> Adjustments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [all components booked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [all components booked]; otherwise, <c>false</c>.
        /// </value>
        bool AllComponentsBooked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [all components pre booked].
        /// </summary>
        /// <value>
        /// <c>true</c> if [all components pre booked]; otherwise, <c>false</c>.
        /// </value>
        bool AllComponentsPreBooked { get; set; }

        /// <summary>
        /// Gets or sets the amount due today.
        /// </summary>
        /// <value>
        /// The amount due today.
        /// </value>
        decimal AmountDueToday { get; set; }

        /// <summary>
        /// Gets or sets the booking reference.
        /// </summary>
        /// <value>
        /// The booking reference.
        /// </value>
        string BookingReference { get; set; }

        /// <summary>
        /// Gets or sets the commission percentage.
        /// </summary>
        /// <value>
        /// The commission percentage.
        /// </value>
        decimal CommissionPercentage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [component failed to book].
        /// </summary>
        /// <value>
        /// <c>true</c> if [component failed to book]; otherwise, <c>false</c>.
        /// </value>
        bool ComponentFailedToBook { get; set; }

        /// <summary>
        /// Gets or sets the components.
        /// </summary>
        /// <value>
        /// The components.
        /// </value>
        List<IBasketComponent> Components { get; set; }

        /// <summary>
        /// Gets or sets the errata.
        /// </summary>
        /// <value>
        /// The errata.
        /// </value>
        List<IErratum> Errata { get; set; }

        /// <summary>
        /// Gets or sets the external reference.
        /// </summary>
        /// <value>
        /// The external reference.
        /// </value>
        string ExternalReference { get; set; }

        /// <summary>
        /// Gets or sets the flight supplier payment amount.
        /// </summary>
        /// <value>
        /// The flight supplier payment amount.
        /// </value>
        decimal FlightSupplierPaymentAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we require date of birth
        /// We only have to take a DoB if there is a flight that requires one, we find this out at prebook.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [guests require do b]; otherwise, <c>false</c>.
        /// </value>
        bool GuestsRequireDoB { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the booking is a trade booking
        /// </summary>
        /// <value>
        ///   <c>true</c> if [the booking is a trade booking]; otherwise, <c>false</c>.
        /// </value>
        bool IsTrade { get; set; }

        /// <summary>
        /// Gets or sets the lead guest.
        /// </summary>
        /// <value>
        /// The lead guest.
        /// </value>
        LeadGuestDetails LeadGuest { get; set; }

        /// <summary>
        /// Gets or sets the outstanding amount.
        /// </summary>
        /// <value>
        /// The outstanding amount.
        /// </value>
        decimal OutstandingAmount { get; set; }

        /// <summary>
        /// Gets or sets the payment details.
        /// </summary>
        /// <value>
        /// The payment details.
        /// </value>
        PaymentDetails PaymentDetails { get; set; }

        /// <summary>
        /// Gets or sets the payments.
        /// </summary>
        /// <value>
        /// The payments.
        /// </value>
        List<IPayment> Payments { get; set; }

        /// <summary>
        /// Gets or sets the prebook total price.
        /// </summary>
        /// <value>
        /// The prebook total price.
        /// </value>
        decimal PrebookTotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the promo code.
        /// </summary>
        /// <value>
        /// The promo code.
        /// </value>
        string PromoCode { get; set; }

        /// <summary>
        /// Gets or sets the promotional code discount.
        /// </summary>
        /// <value>
        /// The promotional code discount.
        /// </value>
        decimal PromoCodeDiscount { get; set; }

        /// <summary>
        /// Gets or sets the quote reference.
        /// </summary>
        /// <value>The quote reference.</value>
        string QuoteReference { get; set; }
  
        /// <summary>
        /// Gets or sets the rooms.
        /// </summary>
        /// <value>
        /// The rooms.
        /// </value>
        List<BasketRoom> Rooms { get; set; }

        /// <summary>
        /// Gets or sets the search details.
        /// </summary>
        /// <value>
        /// The search details.
        /// </value>
        ISearchModel SearchDetails { get; set; }

        /// <summary>
        /// Gets the total amount due.
        /// </summary>
        /// <value>The total amount due.</value>
        decimal TotalAmountDue { get; }

        /// <summary>
        /// Gets the total price.
        /// </summary>
        /// <value>
        /// The total price.
        /// </value>
        decimal TotalPrice { get; }

        /// <summary>
        /// Gets or sets the trade reference.
        /// </summary>
        /// <value>
        /// The trade reference.
        /// </value>
        string TradeReference { get; set; }

        /// <summary>
        /// Gets or sets the VAT on commission percentage.
        /// </summary>
        /// <value>
        /// The VAT on commission percentage.
        /// </value>
        decimal VATOnCommissionPercentage { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        List<string> Warnings { get; set; }

        /// <summary>
        /// Setup Guests ids on components
        /// </summary>
        void SetupGuestIDs();
    }
}