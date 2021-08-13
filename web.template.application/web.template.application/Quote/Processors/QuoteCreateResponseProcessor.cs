namespace Web.Template.Application.Quote
{
    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Quote;
    using Web.Template.Application.Interfaces.Quote.Processors;

    /// <summary>
    /// Class QuoteCreateResponseProcessor.
    /// </summary>
    public class QuoteCreateResponseProcessor : IQuoteCreateResponseProcessor
    {
        /// <summary>
        /// Processes the specified quote response.
        /// </summary>
        /// <param name="quoteResponse">The quote response.</param>
        /// <param name="basket">The basket.</param>
        public void Process(QuoteResponse quoteResponse, IBasket basket)
        {
           this.UpdateBasketWithResponseValues(quoteResponse, basket);
        }

        /// <summary>
        /// Updates the basket with response values.
        /// </summary>
        /// <param name="quoteResponse">The quote response.</param>
        /// <param name="basket">The basket.</param>
        private void UpdateBasketWithResponseValues(QuoteResponse quoteResponse, IBasket basket)
        {
            basket.QuoteReference = quoteResponse.QuoteReference;
        }
    }
}
