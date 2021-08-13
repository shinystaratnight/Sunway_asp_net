namespace Web.Template.Application.Interfaces.Quote.Builders
{
    using System.Collections.Generic;

    using iVectorConnectInterface;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Quote.Models;

    /// <summary>
    /// Interface IQuoteRetrieveReturnBuilder
    /// </summary>
    public interface IQuoteRetrieveReturnBuilder
    {
        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <returns>The Quote Retrieve Return.</returns>
        IQuoteRetrieveReturn GetResult();

        /// <summary>
        /// Sets the basket token.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        void SetBasketToken(string basketToken);

        /// <summary>
        /// Sets the search model.
        /// </summary>
        /// <param name="quoteRetrieveResponse">The quote retrieve response.</param>
        void SetSearchModel(QuoteRetrieveResponse quoteRetrieveResponse);

        /// <summary>
        /// Sets the search result.
        /// </summary>
        /// <param name="searchResult">The search result.</param>
        void SetSearchResult(List<IResultsModel> searchResult);

        /// <summary>
        /// Setups the components.
        /// </summary>
        /// <param name="quoteRetrieveResponse">The quote retrieve response.</param>
        void SetupComponents(QuoteRetrieveResponse quoteRetrieveResponse);
    }
}