namespace Web.Template.Application.Interfaces.Quote.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Interface IQuoteRetrieveReturn
    /// </summary>
    public interface IQuoteRetrieveReturn
    {
        /// <summary>
        /// Gets or sets the basket token.
        /// </summary>
        /// <value>The basket token.</value>
        string BasketToken { get; set; }

        /// <summary>
        /// Gets or sets the price change.
        /// </summary>
        /// <value>The price change.</value>
        decimal PriceChange { get; set; }

        /// <summary>
        /// Gets or sets the property identifier.
        /// </summary>
        /// <value>The property identifier.</value>
        int PropertyId { get; set; }

        /// <summary>
        /// Gets or sets the quote component types.
        /// </summary>
        /// <value>The quote component types.</value>
        List<ComponentType> QuoteComponentTypes { get; set; }

        /// <summary>
        /// Gets or sets the re-priced component types.
        /// </summary>
        /// <value>The re-priced component types.</value>
        List<ComponentType> RepricedComponentTypes { get; set; }

        /// <summary>
        /// Gets or sets the result counts.
        /// </summary>
        /// <value>The result counts.</value>
        Dictionary<string, int> ResultCounts { get; set; }

        /// <summary>
        /// Gets or sets the result tokens.
        /// </summary>
        /// <value>The result tokens.</value>
        Dictionary<string, string> ResultTokens { get; set; }

        /// <summary>
        /// Gets or sets the search model.
        /// </summary>
        /// <value>The search model.</value>
        ISearchModel SearchModel { get; set; }
 
        /// <summary>
        /// Gets or sets the selected flight token.
        /// </summary>
        /// <value>The selected flight token.</value>
        int SelectedFlightToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether success.
        /// </summary>
        bool Success { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        List<string> Warnings { get; set; }
    }
}
