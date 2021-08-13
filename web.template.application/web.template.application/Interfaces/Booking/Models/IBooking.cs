namespace Web.Template.Application.Interfaces.Booking.Models
{
    using System;
    using System.Collections.Generic;

    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Booking.Models;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Prebook.Models;
    using Web.Template.Domain.Entities.Payment;

    /// <summary>
    /// An interface defining a booking
    /// </summary>
    public interface IBooking
    {
        /// <summary>
        /// Gets or sets the booking date.
        /// </summary>
        /// <value>
        /// The booking date.
        /// </value>
        DateTime BookingDate { get; set; }

        /// <summary>
        /// Gets or sets the components.
        /// </summary>
        /// <value>
        /// The components.
        /// </value>
        List<IBasketComponent> Components { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>The currency.</value>
        Currency Currency { get; set; }

        /// <summary>
        /// Gets or sets the customer payments.
        /// </summary>
        /// <value>The customer payments.</value>
        List<CustomerPayment> CustomerPayments { get; set; }

        /// <summary>
        /// Gets or sets the errata.
        /// </summary>
        /// <value>The errata.</value>
        List<IErratum> Errata { get; set; }

        /// <summary>
        /// Gets or sets the guests.
        /// </summary>
        /// <value>
        /// The guests.
        /// </value>
        List<GuestDetail> Guests { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        int Id { get; set; }

        /// <summary>
        /// Gets or sets the lead guest.
        /// </summary>
        /// <value>
        /// The lead guest.
        /// </value>
        LeadGuestDetails LeadGuest { get; set; }

        /// <summary>
        /// Gets or sets the booking reference.
        /// </summary>
        /// <value>
        /// The booking reference.
        /// </value>
        string Reference { get; set; }

        /// <summary>
        /// Gets or sets the total paid.
        /// </summary>
        /// <value>The total paid.</value>
        decimal TotalPaid { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        /// <value>The total price.</value>
        decimal TotalPrice { get; set; }
    }
}