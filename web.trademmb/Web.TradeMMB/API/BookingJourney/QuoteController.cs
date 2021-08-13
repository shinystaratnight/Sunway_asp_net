namespace Web.TradeMMB.API.BookingJourney
{
    using System.Web.Http;

    using Web.Template.Application.Interfaces.Quote.Models;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Quote.Models;

    /// <summary>
    /// Class QuoteController.
    /// </summary>
    public class QuoteController : ApiController
    {
        /// <summary>
        /// The quote service
        /// </summary>
        private readonly IQuoteService quoteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteController"/> class.
        /// </summary>
        /// <param name="quoteService">The quote service.</param>
        public QuoteController(IQuoteService quoteService)
        {
            this.quoteService = quoteService;
        }

        /// <summary>
        /// Searches the specified quote search.
        /// </summary>
        /// <param name="quoteSearch">The quote search.</param>
        /// <returns>The quote search return.</returns>
        [Route("api/quote/search")]
        [HttpPost]
        public IQuoteSearchReturn Search([FromBody] QuoteSearch quoteSearch)
        {
            return this.quoteService.Search(quoteSearch);
        }
    }
}