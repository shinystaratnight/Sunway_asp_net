namespace Web.Template.Application.Interfaces.Quote.Processors
{
    using iVectorConnectInterface;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Interface IQuoteRetrieveResponseProcessor
    /// </summary>
    public interface IQuoteRetrieveResponseProcessor
    {
        /// <summary>
        /// Processes the specified quote retrieve response.
        /// </summary>
        /// <param name="quoteRetrieveResponse">The quote retrieve response.</param>
        /// <param name="basket">The basket.</param>
        void Process(QuoteRetrieveResponse quoteRetrieveResponse, IBasket basket);
    }
}