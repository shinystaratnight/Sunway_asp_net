namespace Web.Template.Application.Quote.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Quote;
    using Web.Template.Application.Interfaces.Quote.Models;

    /// <summary>
    /// Class QuoteSearchReturn.
    /// </summary>
    public class QuoteSearchReturn : IQuoteSearchReturn
    {
        /// <summary>
        /// Gets or sets the quotes.
        /// </summary>
        /// <value>The quotes.</value>
        public List<IQuote> Quotes { get; set; }
 
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IQuoteSearchReturn" /> is success.
        /// </summary>
        /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        public List<string> Warnings { get; set; }
    }
}
