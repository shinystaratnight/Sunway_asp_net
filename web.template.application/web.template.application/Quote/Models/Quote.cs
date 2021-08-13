namespace Web.Template.Application.Quote.Models
{
    using System;
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Quote;
    using Web.Template.Application.Interfaces.Quote.Models;

    /// <summary>
    /// Class Quote.
    /// </summary>
    public class Quote : IQuote
    {
        /// <summary>
        /// Gets or sets the quote reference.
        /// </summary>
        /// <value>The quote reference.</value>
        public string QuoteReference { get; set; }

        /// <summary>
        /// Gets or sets the trade reference.
        /// </summary>
        /// <value>The trade reference.</value>
        public string TradeReference { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the account status.
        /// </summary>
        /// <value>The account status.</value>
        public string AccountStatus { get; set; }

        /// <summary>
        /// Gets or sets the first name of the lead customer.
        /// </summary>
        /// <value>The first name of the lead customer.</value>
        public string LeadCustomerFirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the lead customer.
        /// </summary>
        /// <value>The last name of the lead customer.</value>
        public string LeadCustomerLastName { get; set; }

        /// <summary>
        /// Gets or sets the total passengers.
        /// </summary>
        /// <value>The total passengers.</value>
        public int TotalPassengers { get; set; }

        /// <summary>
        /// Gets or sets the booking date.
        /// </summary>
        /// <value>The booking date.</value>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Gets or sets the arrival date.
        /// </summary>
        /// <value>The arrival date.</value>
        public DateTime ArrivalDate { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the resort.
        /// </summary>
        /// <value>The resort.</value>
        public string Resort { get; set; }

        /// <summary>
        /// Gets or sets the last return date.
        /// </summary>
        /// <value>The last return date.</value>
        public DateTime LastReturnDate { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        /// <value>The total price.</value>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the total commission.
        /// </summary>
        /// <value>The total commission.</value>
        public decimal TotalCommission { get; set; }

        /// <summary>
        /// Gets or sets the components.
        /// </summary>
        /// <value>The components.</value>
        public List<IQuoteComponent> Components { get; set; }
    }
}
