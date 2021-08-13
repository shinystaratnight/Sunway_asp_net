namespace Web.Template.Application.Quote.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Quote;
    using Web.Template.Application.Interfaces.Quote.Models;

    /// <summary>
    /// Class QuoteCreateReturn.
    /// </summary>
    public class QuoteCreateReturn : IQuoteCreateReturn
    {
        /// <summary>
        /// Gets or sets the basket.
        /// </summary>
        /// <value>The basket.</value>
        public IBasket Basket { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IQuoteCreateReturn" /> is success.
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
