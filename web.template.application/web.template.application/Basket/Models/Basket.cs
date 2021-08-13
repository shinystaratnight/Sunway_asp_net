namespace Web.Template.Application.Basket.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Web.Template.Application.Basket.Models.Components.SubComponent;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Prebook.Models;

    /// <summary>
    /// Basket model
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Models.IBasket" />
    public class Basket : IBasket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Basket"/> class.
        /// </summary>
        public Basket()
        {
            this.Adjustments = new List<IAdjustment>();
            this.Errata = new List<IErratum>();
            this.BasketToken = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the adjustments.
        /// </summary>
        /// <value>
        /// The adjustments.
        /// </value>
        public List<IAdjustment> Adjustments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [all components booked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [all components booked]; otherwise, <c>false</c>.
        /// </value>
        public bool AllComponentsBooked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [all components pre booked].
        /// </summary>
        /// <value>
        /// <c>true</c> if [all components pre booked]; otherwise, <c>false</c>.
        /// </value>
        public bool AllComponentsPreBooked { get; set; }

        /// <summary>
        /// Gets or sets the amount due today.
        /// </summary>
        /// <value>
        /// The amount due today.
        /// </value>
        public decimal AmountDueToday { get; set; }

        /// <summary>
        /// Gets or sets the basket token.
        /// </summary>
        /// <value>
        /// The basket token.
        /// </value>
        public Guid BasketToken { get; set; }

        /// <summary>
        /// Gets or sets the booking reference.
        /// </summary>
        /// <value>
        /// The booking reference.
        /// </value>
        public string BookingReference { get; set; }

        /// <summary>
        /// Gets or sets the commission percentage.
        /// </summary>
        /// <value>
        /// The commission percentage.
        /// </value>
        public decimal CommissionPercentage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [component failed to book].
        /// </summary>
        /// <value>
        /// <c>true</c> if [component failed to book]; otherwise, <c>false</c>.
        /// </value>
        public bool ComponentFailedToBook { get; set; }

        /// <summary>
        /// Gets or sets the components.
        /// </summary>
        /// <value>
        /// The components.
        /// </value>
        public List<IBasketComponent> Components { get; set; }

        /// <summary>
        /// Gets or sets the errata.
        /// </summary>
        /// <value>
        /// The errata.
        /// </value>
        public List<IErratum> Errata { get; set; }

        /// <summary>
        /// Gets or sets the external reference.
        /// </summary>
        /// <value>
        /// The external reference.
        /// </value>
        public string ExternalReference { get; set; }

        /// <summary>
        /// Gets or sets the flight supplier payment amount.
        /// </summary>
        /// <value>
        /// The flight supplier payment amount.
        /// </value>
        public decimal FlightSupplierPaymentAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we required DoB
        /// We only have to take a DoB if there is a flight that requires one, we find this out at prebook.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [guests require do b]; otherwise, <c>false</c>.
        /// </value>
        public bool GuestsRequireDoB { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the booking is a trade booking
        /// </summary>
        /// <value>
        ///   <c>true</c> if [the booking is a trade booking]; otherwise, <c>false</c>.
        /// </value>
        public bool IsTrade { get; set; }

        /// <summary>
        /// Gets or sets the lead guest.
        /// </summary>
        /// <value>
        /// The lead guest.
        /// </value>
        public LeadGuestDetails LeadGuest { get; set; } = new LeadGuestDetails();

        /// <summary>
        /// Gets or sets the outstanding amount.
        /// </summary>
        /// <value>
        /// The outstanding amount.
        /// </value>
        public decimal OutstandingAmount { get; set; }

        /// <summary>
        /// Gets or sets the payment details.
        /// </summary>
        /// <value>
        /// The payment details.
        /// </value>
        public PaymentDetails PaymentDetails { get; set; }

        /// <summary>
        /// Gets or sets the payments.
        /// </summary>
        /// <value>
        /// The payments.
        /// </value>
        public List<IPayment> Payments { get; set; }

        /// <summary>
        /// Gets or sets the prebook total price.
        /// </summary>
        /// <value>
        /// The prebook total price.
        /// </value>
        public decimal PrebookTotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the promo code.
        /// </summary>
        /// <value>
        /// The promo code.
        /// </value>
        public string PromoCode { get; set; }

        /// <summary>
        /// Gets or sets the promotional code discount.
        /// </summary>
        /// <value>
        /// The promotional code discount.
        /// </value>
        public decimal PromoCodeDiscount { get; set; }

        /// <summary>
        /// Gets or sets the quote reference.
        /// </summary>
        /// <value>The quote reference.</value>
        public string QuoteReference { get; set; }

        /// <summary>
        /// Gets or sets the guests.
        /// </summary>
        /// <value>
        /// The guests.
        /// </value>
        public List<BasketRoom> Rooms { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        /// <value>
        /// The total price.
        /// </value>
        public ISearchModel SearchDetails { get; set; }

        /// <summary>
        /// Gets the total amount due.
        /// </summary>
        /// <value>The total amount due.</value>
        public decimal TotalAmountDue
        {
            get
            {
                decimal totalAmountDue = 0;
                if (this.Payments != null)
                {
                    totalAmountDue = this.Payments.Sum(p => p.Amount);
                    totalAmountDue += this.Adjustments.Sum(a => a.AdjustmentAmount);
                    totalAmountDue -= this.PromoCodeDiscount;
                }
                return totalAmountDue;
            }
        }

        /// <summary>
        /// Gets the total price.
        /// </summary>
        /// <value>
        /// The total price.
        /// </value>
        public decimal TotalPrice
        {
            get
            {
                decimal totalPrice = 0;

                totalPrice += this.Components.Sum(c => c.TotalPrice);

                totalPrice += this.Adjustments.Sum(a => a.AdjustmentAmount);

                totalPrice -= this.PromoCodeDiscount;

                return totalPrice;
            }
        }

        /// <summary>
        /// Gets or sets the trade reference.
        /// </summary>
        /// <value>
        /// The trade reference.
        /// </value>
        public string TradeReference { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        public List<string> Warnings { get; set; }

        /// <summary>
        /// Setup Guests ids on components
        /// </summary>
        public void SetupGuestIDs()
        {
            if (this.Rooms != null && this.Rooms.Count > 0)
            {
                foreach (IBasketComponent basketComponent in this.Components)
                {
                    if (basketComponent.ComponentType == ComponentType.Hotel)
                    {
                        var roomNumber = 0;
                        foreach (ISubComponent subComponent in basketComponent.SubComponents)
                        {
                            var room = (Room)subComponent;
                            var adultGuestIDs = this.Rooms[roomNumber].Guests.Where(guest => guest.Type == "Adult").Select(guest => guest.GuestID).ToList();
                            var childGuestIDs = this.Rooms[roomNumber].Guests.Where(guest => guest.Type == "Child").Select(guest => guest.GuestID).ToList();
                            var infantGuestIDs = this.Rooms[roomNumber].Guests.Where(guest => guest.Type == "Infant").Select(guest => guest.GuestID).ToList();

                            room.GuestIDs = new List<int>();
                            room.GuestIDs.AddRange(adultGuestIDs.GetRange(0, room.Adults));
                            room.GuestIDs.AddRange(childGuestIDs.GetRange(0, room.Children));
                            room.GuestIDs.AddRange(infantGuestIDs.GetRange(0, room.Infants));

                            roomNumber++;
                        }
                    }
                    else
                    {
                        var adultGuestIDs = this.Rooms.SelectMany(room => room.Guests.Where(guest => guest.Type == "Adult").Select(guest => guest.GuestID).ToList()).ToList();
                        var childGuestIDs = this.Rooms.SelectMany(room => room.Guests.Where(guest => guest.Type == "Child").Select(guest => guest.GuestID).ToList()).ToList();
                        var infantGuestIDs = this.Rooms.SelectMany(room => room.Guests.Where(guest => guest.Type == "Infant").Select(guest => guest.GuestID).ToList()).ToList();

                        basketComponent.GuestIDs = new List<int>();
                        basketComponent.GuestIDs.AddRange(adultGuestIDs.GetRange(0, basketComponent.Adults));
                        basketComponent.GuestIDs.AddRange(childGuestIDs.GetRange(0, basketComponent.Children));
                        basketComponent.GuestIDs.AddRange(infantGuestIDs.GetRange(0, basketComponent.Infants));
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the VAT on commission percentage.
        /// </summary>
        /// <value>
        /// The VAT on commission percentage.
        /// </value>
        public decimal VATOnCommissionPercentage { get; set; }
    }
}