namespace Web.Template.Application.Interfaces.Booking.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a class that is returned as a result of a booking search
    /// </summary>
    public interface IBookingSearchResult
    {
        /// <summary>
        /// Gets or sets the account status.
        /// </summary>
        /// <value>
        /// The account status.
        /// </value>
        string AccountStatus { get; set; }

        /// <summary>
        /// Gets or sets the arrival date.
        /// </summary>
        /// <value>
        /// The arrival date.
        /// </value>
        DateTime ArrivalDate { get; set; }

        /// <summary>
        /// Gets or sets the booking date.
        /// </summary>
        /// <value>
        /// The booking date.
        /// </value>
        DateTime BookingDate { get; set; }

        /// <summary>
        /// Gets or sets the booking reference.
        /// </summary>
        /// <value>
        /// The booking reference.
        /// </value>
        string BookingReference { get; set; }

        /// <summary>
        /// Gets or sets the component list.
        /// </summary>
        /// <value>
        /// The component list.
        /// </value>
        List<iVectorConnectInterface.SearchBookingsResponse.Booking.Component> ComponentList { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        /// <value>
        /// The currency symbol.
        /// </value>
        string CurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol position.
        /// </summary>
        /// <value>
        /// The currency symbol position.
        /// </value>
        string CurrencySymbolPosition { get; set; }

        /// <summary>
        /// Gets or sets the customer currency identifier.
        /// </summary>
        /// <value>
        /// The customer currency identifier.
        /// </value>
        int CustomerCurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        int Duration { get; set; }

        /// <summary>
        /// Gets or sets the geography level1 identifier.
        /// </summary>
        /// <value>
        /// The geography level1 identifier.
        /// </value>
        int GeographyLevel1Id { get; set; }

        /// <summary>
        /// Gets or sets the geography level2 identifier.
        /// </summary>
        /// <value>
        /// The geography level2 identifier.
        /// </value>
        int GeographyLevel2Id { get; set; }

        /// <summary>
        /// Gets or sets the geography level3 identifier.
        /// </summary>
        /// <value>
        /// The geography level3 identifier.
        /// </value>
        int GeographyLevel3Id { get; set; }

        /// <summary>
        /// Gets or sets the first name of the lead customer.
        /// </summary>
        /// <value>
        /// The first name of the lead customer.
        /// </value>
        string LeadCustomerFirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the lead customer.
        /// </summary>
        /// <value>
        /// The last name of the lead customer.
        /// </value>
        string LeadCustomerLastName { get; set; }

        /// <summary>
        /// Gets or sets the resort.
        /// </summary>
        /// <value>
        /// The resort.
        /// </value>
        string Resort { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        string Status { get; set; }

        /// <summary>
        /// Gets or sets the total commission.
        /// </summary>
        /// <value>
        /// The total commission.
        /// </value>
        decimal TotalCommission { get; set; }

        /// <summary>
        /// Gets or sets the total outstanding.
        /// </summary>
        /// <value>
        /// The total outstanding.
        /// </value>
        decimal TotalOutstanding { get; set; }

        /// <summary>
        /// Gets or sets the total paid.
        /// </summary>
        /// <value>
        /// The total paid.
        /// </value>
        decimal TotalPaid { get; set; }

        /// <summary>
        /// Gets or sets the total passengers.
        /// </summary>
        /// <value>
        /// The total passengers.
        /// </value>
        int TotalPassengers { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        /// <value>
        /// The total price.
        /// </value>
        decimal TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the total vat on commission.
        /// </summary>
        /// <value>
        /// The total vat on commission.
        /// </value>
        decimal TotalVatOnCommission { get; set; }

        /// <summary>
        /// Gets or sets the trade reference.
        /// </summary>
        /// <value>
        /// The trade reference.
        /// </value>
        string TradeReference { get; set; }
    }
}