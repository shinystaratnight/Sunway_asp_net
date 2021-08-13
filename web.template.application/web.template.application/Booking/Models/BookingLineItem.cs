namespace Web.Template.Application.Booking.Models
{
    using System;

    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// Booking line item which is returned from a direct debit call
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Booking.Models.IBookingLineItem" />
    public class BookingLineItem : IBookingLineItem
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the direct debit list identifier.
        /// </summary>
        /// <value>
        /// The direct debit list identifier.
        /// </value>
        public int DirectDebitListId { get; set; }

        /// <summary>
        /// Gets or sets the booking identifier.
        /// </summary>
        /// <value>
        /// The booking identifier.
        /// </value>
        public int BookingId { get; set; }

        /// <summary>
        /// Gets or sets the booking reference.
        /// </summary>
        /// <value>
        /// The booking reference.
        /// </value>
        public string BookingReference { get; set; }

        /// <summary>
        /// Gets or sets the brand identifier.
        /// </summary>
        /// <value>
        /// The brand identifier.
        /// </value>
        public int BrandId { get; set; }

        /// <summary>
        /// Gets or sets the name of the trade head office.
        /// </summary>
        /// <value>
        /// The name of the trade head office.
        /// </value>
        public string TradeHeadOfficeName { get; set; }

        /// <summary>
        /// Gets or sets the trade head office abtaatol.
        /// </summary>
        /// <value>
        /// The trade head office abtaatol.
        /// </value>
        public string TradeHeadOfficeAbtaatol { get; set; }

        /// <summary>
        /// Gets or sets the name of the trade.
        /// </summary>
        /// <value>
        /// The name of the trade.
        /// </value>
        public string TradeName { get; set; }

        /// <summary>
        /// Gets or sets the trade abtaatol.
        /// </summary>
        /// <value>
        /// The trade abtaatol.
        /// </value>
        public string TradeAbtaatol { get; set; }

        /// <summary>
        /// Gets or sets the trade identifier.
        /// </summary>
        /// <value>
        /// The trade identifier.
        /// </value>
        public int TradeId { get; set; }

        /// <summary>
        /// Gets or sets the head office trade identifier.
        /// </summary>
        /// <value>
        /// The head office trade identifier.
        /// </value>
        public int HeadOfficeTradeId { get; set; }

        /// <summary>
        /// Gets or sets the first name of the lead guest.
        /// </summary>
        /// <value>
        /// The first name of the lead guest.
        /// </value>
        public string LeadGuestFirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the lead guest.
        /// </summary>
        /// <value>
        /// The last name of the lead guest.
        /// </value>
        public string LeadGuestLastName { get; set; }

        /// <summary>
        /// Gets or sets the departure date.
        /// </summary>
        /// <value>
        /// The departure date.
        /// </value>
        public DateTime DepartureDate { get; set; }

        /// <summary>
        /// Gets or sets the amount due.
        /// </summary>
        /// <value>
        /// The amount due.
        /// </value>
        public decimal AmountDue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IBookingLineItem" /> is pay.
        /// </summary>
        /// <value>
        ///   <c>true</c> if pay; otherwise, <c>false</c>.
        /// </value>
        public bool Pay { get; set; }
    }
}