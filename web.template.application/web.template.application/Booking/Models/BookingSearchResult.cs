namespace Web.Template.Application.Booking.Models
{
    using System;
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// Booking Search Result
    /// </summary>
    public class BookingSearchResult : IBookingSearchResult
    {
        /// <summary>
        /// Gets or sets the account status.
        /// </summary>
        /// <value>
        /// The account status.
        /// </value>
        public string AccountStatus { get; set; }

        /// <summary>
        /// Gets or sets the arrival date.
        /// </summary>
        /// <value>
        /// The arrival date.
        /// </value>
        public DateTime ArrivalDate { get; set; }

        /// <summary>
        /// Gets or sets the booking date.
        /// </summary>
        /// <value>
        /// The booking date.
        /// </value>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Gets or sets the booking reference.
        /// </summary>
        /// <value>
        /// The booking reference.
        /// </value>
        public string BookingReference { get; set; }

        /// <summary>
        /// Gets or sets the component list.
        /// </summary>
        /// <value>
        /// The component list.
        /// </value>
        public List<iVectorConnectInterface.SearchBookingsResponse.Booking.Component> ComponentList { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        /// <value>
        /// The currency symbol.
        /// </value>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol position.
        /// </summary>
        /// <value>
        /// The currency symbol position.
        /// </value>
        public string CurrencySymbolPosition { get; set; }

        /// <summary>
        /// Gets or sets the customer currency identifier.
        /// </summary>
        /// <value>
        /// The customer currency identifier.
        /// </value>
        public int CustomerCurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the geography level1 identifier.
        /// </summary>
        /// <value>
        /// The geography level1 identifier.
        /// </value>
        public int GeographyLevel1Id { get; set; }

        /// <summary>
        /// Gets or sets the geography level2 identifier.
        /// </summary>
        /// <value>
        /// The geography level2 identifier.
        /// </value>
        public int GeographyLevel2Id { get; set; }

        /// <summary>
        /// Gets or sets the geography level3 identifier.
        /// </summary>
        /// <value>
        /// The geography level3 identifier.
        /// </value>
        public int GeographyLevel3Id { get; set; }

        /// <summary>
        /// Gets or sets the first name of the lead customer.
        /// </summary>
        /// <value>
        /// The first name of the lead customer.
        /// </value>
        public string LeadCustomerFirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the lead customer.
        /// </summary>
        /// <value>
        /// The last name of the lead customer.
        /// </value>
        public string LeadCustomerLastName { get; set; }

        /// <summary>
        /// Gets or sets the resort.
        /// </summary>
        /// <value>
        /// The resort.
        /// </value>
        public string Resort { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the total commission.
        /// </summary>
        /// <value>
        /// The total commission.
        /// </value>
        public decimal TotalCommission { get; set; }

        /// <summary>
        /// Gets or sets the total outstanding.
        /// </summary>
        /// <value>
        /// The total outstanding.
        /// </value>
        public decimal TotalOutstanding { get; set; }

        /// <summary>
        /// Gets or sets the total paid.
        /// </summary>
        /// <value>
        /// The total paid.
        /// </value>
        public decimal TotalPaid { get; set; }

        /// <summary>
        /// Gets or sets the total passengers.
        /// </summary>
        /// <value>
        /// The total passengers.
        /// </value>
        public int TotalPassengers { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        /// <value>
        /// The total price.
        /// </value>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the total vat on commission.
        /// </summary>
        /// <value>
        /// The total vat on commission.
        /// </value>
        public decimal TotalVatOnCommission { get; set; }

        /// <summary>
        /// Gets or sets the trade reference.
        /// </summary>
        /// <value>
        /// The trade reference.
        /// </value>
        public string TradeReference { get; set; }
    }
}