namespace Web.Template.Application.Booking
{
    using System;
    using System.Collections.Generic;

    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Booking.Models;
    using Web.Template.Application.Interfaces.Booking.Models;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Prebook.Models;
    using Web.Template.Domain.Entities.Payment;

    /// <summary>
    /// A model used to represent a booking
    /// </summary>
    /// <seealso cref="IBooking" />
    public class Booking : IBooking
    {
        /// <summary>
        /// Gets or sets the booking date.
        /// </summary>
        /// <value>
        /// The booking date.
        /// </value>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Gets or sets the booking components.
        /// </summary>
        /// <value>
        /// The booking components.
        /// </value>
        public List<IBasketComponent> Components { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>The currency.</value>
        public Currency Currency { get; set; }

        /// <summary>
        /// Gets or sets the customer payments.
        /// </summary>
        /// <value>The customer payments.</value>
        public List<CustomerPayment> CustomerPayments { get; set; }

        /// <summary>
        /// Gets or sets the errata.
        /// </summary>
        /// <value>The errata.</value>
        public List<IErratum> Errata { get; set; }

        /// <summary>
        /// Gets or sets the guests.
        /// </summary>
        /// <value>
        /// The guests.
        /// </value>
        public List<GuestDetail> Guests { get; set; }

        /// <summary>
        /// Gets or sets the booking identifier.
        /// </summary>
        /// <value>
        /// The booking identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the lead guest.
        /// </summary>
        /// <value>
        /// The lead guest.
        /// </value>
        public LeadGuestDetails LeadGuest { get; set; } = new LeadGuestDetails();

        /// <summary>
        /// Gets or sets the booking reference.
        /// </summary>
        /// <value>
        /// The booking reference.
        /// </value>
        public string Reference { get; set; }

        /// <summary>
        /// Gets or sets the total paid.
        /// </summary>
        /// <value>The total paid.</value>
        public decimal TotalPaid { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        /// <value>The total price.</value>
        public decimal TotalPrice { get; set; }
    }
}