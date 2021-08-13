namespace Web.Template.Application.Quote.Models
{
    using System;
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Quote.Models;

    /// <summary>
    /// Class QuoteSearch.
    /// </summary>
    public class QuoteSearch : IQuoteSearch
    {
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>The customer identifier.</value>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the trade contact identifier.
        /// </summary>
        /// <value>The trade contact identifier.</value>
        public int TradeContactId { get; set; }

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
        /// Gets or sets the earliest booking date.
        /// </summary>
        /// <value>The earliest booking date.</value>
        public DateTime EarliestBookingDate { get; set; }

        /// <summary>
        /// Gets or sets the earliest booking time.
        /// </summary>
        /// <value>The earliest booking time.</value>
        public string EarliestBookingTime { get; set; }

        /// <summary>
        /// Gets or sets the latest booking date.
        /// </summary>
        /// <value>The latest booking date.</value>
        public DateTime LatestBookingDate { get; set; }

        /// <summary>
        /// Gets or sets the latest booking time.
        /// </summary>
        /// <value>The latest booking time.</value>
        public string LatestBookingTime { get; set; }

        /// <summary>
        /// Gets or sets the earliest departure date.
        /// </summary>
        /// <value>The earliest departure date.</value>
        public DateTime EarliestDepartureDate { get; set; }

        /// <summary>
        /// Gets or sets the latest departure date.
        /// </summary>
        /// <value>The latest departure date.</value>
        public DateTime LatestDepartureDate { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the brand ids.
        /// </summary>
        /// <value>The brand ids.</value>
        public List<int> BrandIds { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public string Source { get; set; }
    }
}
