namespace Web.Template.Application.Interfaces.Quote.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface IQuote
    /// </summary>
    public interface IQuote
    {
        /// <summary>
        /// Gets or sets the quote reference.
        /// </summary>
        /// <value>The quote reference.</value>
        string QuoteReference { get; set; }

        /// <summary>
        /// Gets or sets the trade reference.
        /// </summary>
        /// <value>The trade reference.</value>
        string TradeReference { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        string Status { get; set; }

        /// <summary>
        /// Gets or sets the account status.
        /// </summary>
        /// <value>The account status.</value>
        string AccountStatus { get; set; }

        /// <summary>
        /// Gets or sets the first name of the lead customer.
        /// </summary>
        /// <value>The first name of the lead customer.</value>
        string LeadCustomerFirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the lead customer.
        /// </summary>
        /// <value>The last name of the lead customer.</value>
        string LeadCustomerLastName { get; set; }

        /// <summary>
        /// Gets or sets the total passengers.
        /// </summary>
        /// <value>The total passengers.</value>
        int TotalPassengers { get; set; }

        /// <summary>
        /// Gets or sets the booking date.
        /// </summary>
        /// <value>The booking date.</value>
        DateTime BookingDate { get; set; }

        /// <summary>
        /// Gets or sets the arrival date.
        /// </summary>
        /// <value>The arrival date.</value>
        DateTime ArrivalDate { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        int Duration { get; set; }

        /// <summary>
        /// Gets or sets the resort.
        /// </summary>
        /// <value>The resort.</value>
        string Resort { get; set; }

        /// <summary>
        /// Gets or sets the last return date.
        /// </summary>
        /// <value>The last return date.</value>
        DateTime LastReturnDate { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        /// <value>The total price.</value>
        decimal TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the total commission.
        /// </summary>
        /// <value>The total commission.</value>
        decimal TotalCommission { get; set; }

        /// <summary>
        /// Gets or sets the components.
        /// </summary>
        /// <value>The components.</value>
        List<IQuoteComponent> Components { get; set; }
    }
}