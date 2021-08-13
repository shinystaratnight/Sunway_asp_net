namespace Web.Template.Application.Interfaces.Quote.Adaptors
{
    using iVectorConnectInterface;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Interface IQuoteRetrieveSearchAdaptor
    /// </summary>
    public interface IQuoteRetrieveSearchAdaptor
    {
        /// <summary>
        /// Creates the specified search model.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <param name="quoteRetrieveResponse">The quote retrieve response.</param>
        void Create(ISearchModel searchModel, QuoteRetrieveResponse quoteRetrieveResponse);
    }
}