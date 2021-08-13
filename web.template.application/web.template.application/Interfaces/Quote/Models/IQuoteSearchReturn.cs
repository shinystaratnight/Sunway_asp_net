namespace Web.Template.Application.Interfaces.Quote.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface IQuoteSearchReturn
    /// </summary>
    public interface IQuoteSearchReturn
    {
        /// <summary>
        /// Gets or sets the quotes.
        /// </summary>
        /// <value>The quotes.</value>
        List<IQuote> Quotes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IQuoteSearchReturn" /> is success.
        /// </summary>
        /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
        bool Success { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        List<string> Warnings { get; set; }
    }
}