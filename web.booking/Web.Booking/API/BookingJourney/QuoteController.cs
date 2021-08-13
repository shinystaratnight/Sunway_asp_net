
using Web.Template.Application.Models;

namespace Web.Booking.API.BookingJourney
{
    using System.Threading.Tasks;
    using System.Web.Http;

    using Web.Booking.Models.Application;
    using Web.Template.Application.Interfaces.Quote.Models;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Quote.Models;

    /// <summary>
    /// Class QuoteController.
    /// </summary>
    public class QuoteController : ApiController
    {
        /// <summary>
        /// The basket service
        /// </summary>
        private readonly IBasketService basketService;

        /// <summary>
        /// The quote service
        /// </summary>
        private readonly IQuoteService quoteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteController" /> class.
        /// </summary>
        /// <param name="quoteService">The quote service.</param>
        /// <param name="basketService">The basket service.</param>
        public QuoteController(IQuoteService quoteService, IBasketService basketService)
        {
            this.quoteService = quoteService;
            this.basketService = basketService;
        }

        /// <summary>
        /// Creates the quote.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="basket">The basket.</param>
        /// <returns>The quote create return.</returns>
        [Route("api/quote/create/{basketToken}")]
        [HttpPost]
        public IQuoteCreateReturn CreateQuote(string basketToken, [FromBody] BasketBookModel basket)
        {
            if (!string.IsNullOrEmpty(basketToken) && basket != null)
            {
                this.basketService.ChangeGuests(basketToken, basket.GuestDetails);
                this.basketService.ChangeLeadGuest(basketToken, basket.LeadGuest);
                this.basketService.ChangePayment(basketToken, basket.PaymentDetails);
                this.basketService.ChangeTradeReference(basketToken, basket.TradeReference);
                this.basketService.ChangePropertyRequests(basketToken, basket.HotelRequest);
            }
            return this.quoteService.Create(basketToken);
        }

        /// <summary>
        /// Retrieves the quote.
        /// </summary>
        /// <param name="quoteReference">The quote reference.</param>
        /// <returns>The quote retrieve return.</returns>
        [Route("api/quote/retrieve/{quoteReference}")]
        [HttpGet]
        public async Task<QuoteRetrieveReturn> RetrieveQuote(string quoteReference)
        {
            return await this.quoteService.Retrieve(quoteReference.Replace("-", "/"));
        }

        /// <summary>
        /// Creates the quote.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="basket">The basket.</param>
        /// <returns>The quote create return.</returns>
        [Route("api/quote/email")]
        [HttpPost]
        public bool emailQuote([FromBody] QuoteModel quote)
        {
            return this.quoteService.Email(quote);
        }


        /// <summary>
        /// Creates the quote.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="basket">The basket.</param>
        /// <returns>The quote create return.</returns>
        [Route("api/quote/pdf")]
        [HttpPost]
        public Template.Application.Services.DocumentServiceReturn pdfQuote([FromBody] QuoteModel quote)
        {
            return this.quoteService.CreatePDF(quote);
        }
    }
}