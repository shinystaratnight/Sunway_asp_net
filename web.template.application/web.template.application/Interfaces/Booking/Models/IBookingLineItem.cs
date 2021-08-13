namespace Web.Template.Application.Interfaces.Booking.Models
{
    using System;

    /// <summary>
    /// Interface for the booking line item in direct debits
    /// </summary>
    public interface IBookingLineItem
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        int Id { get; set; }

        /// <summary>
        /// Gets or sets the direct debit list identifier.
        /// </summary>
        /// <value>
        /// The direct debit list identifier.
        /// </value>
        int DirectDebitListId { get; set; }

        /// <summary>
        /// Gets or sets the booking identifier.
        /// </summary>
        /// <value>
        /// The booking identifier.
        /// </value>
        int BookingId { get; set; }

        /// <summary>
        /// Gets or sets the booking reference.
        /// </summary>
        /// <value>
        /// The booking reference.
        /// </value>
        string BookingReference { get; set; }

        /// <summary>
        /// Gets or sets the brand identifier.
        /// </summary>
        /// <value>
        /// The brand identifier.
        /// </value>
        int BrandId { get; set; }

        /// <summary>
        /// Gets or sets the name of the trade head office.
        /// </summary>
        /// <value>
        /// The name of the trade head office.
        /// </value>
        string TradeHeadOfficeName { get; set; }

        /// <summary>
        /// Gets or sets the trade head office abtaatol.
        /// </summary>
        /// <value>
        /// The trade head office abtaatol.
        /// </value>
        string TradeHeadOfficeAbtaatol { get; set; }

        /// <summary>
        /// Gets or sets the name of the trade.
        /// </summary>
        /// <value>
        /// The name of the trade.
        /// </value>
        string TradeName { get; set; }

        /// <summary>
        /// Gets or sets the trade abtaatol.
        /// </summary>
        /// <value>
        /// The trade abtaatol.
        /// </value>
        string TradeAbtaatol { get; set; }

        /// <summary>
        /// Gets or sets the trade identifier.
        /// </summary>
        /// <value>
        /// The trade identifier.
        /// </value>
        int TradeId { get; set; }

        /// <summary>
        /// Gets or sets the head office trade identifier.
        /// </summary>
        /// <value>
        /// The head office trade identifier.
        /// </value>
        int HeadOfficeTradeId { get; set; }

        /// <summary>
        /// Gets or sets the first name of the lead guest.
        /// </summary>
        /// <value>
        /// The first name of the lead guest.
        /// </value>
        string LeadGuestFirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the lead guest.
        /// </summary>
        /// <value>
        /// The last name of the lead guest.
        /// </value>
        string LeadGuestLastName { get; set; }

        /// <summary>
        /// Gets or sets the departure date.
        /// </summary>
        /// <value>
        /// The departure date.
        /// </value>
        DateTime DepartureDate { get; set; }

        /// <summary>
        /// Gets or sets the amount due.
        /// </summary>
        /// <value>
        /// The amount due.
        /// </value>
        decimal AmountDue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IBookingLineItem"/> is pay.
        /// </summary>
        /// <value>
        ///   <c>true</c> if pay; otherwise, <c>false</c>.
        /// </value>
        bool Pay { get; set; }
    }
}