namespace Web.Template.Application.Interfaces.Quote.Processors
{
    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Interface IQuoteCreateResponseProcessor
    /// </summary>
    public interface IQuoteCreateResponseProcessor
    {
        /// <summary>
        /// Processes the specified quote response.
        /// </summary>
        /// <param name="quoteResponse">The quote response.</param>
        /// <param name="basket">The basket.</param>
        void Process(QuoteResponse quoteResponse, IBasket basket);
    }
}
