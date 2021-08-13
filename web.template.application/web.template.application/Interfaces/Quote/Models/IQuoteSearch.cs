namespace Web.Template.Application.Interfaces.Quote.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface IQuoteSearch
    /// </summary>
    public interface IQuoteSearch
    {
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>The customer identifier.</value>
        int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the trade contact identifier.
        /// </summary>
        /// <value>The trade contact identifier.</value>
        int TradeContactId { get; set; }

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
        /// Gets or sets the earliest booking date.
        /// </summary>
        /// <value>The earliest booking date.</value>
        DateTime EarliestBookingDate { get; set; }

        /// <summary>
        /// Gets or sets the earliest booking time.
        /// </summary>
        /// <value>The earliest booking time.</value>
        string EarliestBookingTime { get; set; }

        /// <summary>
        /// Gets or sets the latest booking date.
        /// </summary>
        /// <value>The latest booking date.</value>
        DateTime LatestBookingDate { get; set; }

        /// <summary>
        /// Gets or sets the latest booking time.
        /// </summary>
        /// <value>The latest booking time.</value>
        string LatestBookingTime { get; set; }

        /// <summary>
        /// Gets or sets the earliest departure date.
        /// </summary>
        /// <value>The earliest departure date.</value>
        DateTime EarliestDepartureDate { get; set; }

        /// <summary>
        /// Gets or sets the latest departure date.
        /// </summary>
        /// <value>The latest departure date.</value>
        DateTime LatestDepartureDate { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        int Duration { get; set; }

        /// <summary>
        /// Gets or sets the brand ids.
        /// </summary>
        /// <value>The brand ids.</value>
        List<int> BrandIds { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        string Source { get; set; }
    }
}
