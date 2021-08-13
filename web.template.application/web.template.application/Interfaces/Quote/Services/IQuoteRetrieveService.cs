namespace Web.Template.Application.Interfaces.Quote.Services
{
    using System.Threading.Tasks;

    using Web.Template.Application.Interfaces.Quote.Models;
    using Web.Template.Application.Quote.Models;

    /// <summary>
    /// Interface IQuoteRetrieveService
    /// </summary>
    public interface IQuoteRetrieveService
    {
        /// <summary>
        /// Retrieves the specified quote reference.
        /// </summary>
        /// <param name="quoteReference">The quote reference.</param>
        /// <returns>The Quote Retrieve Return.</returns>
        Task<QuoteRetrieveReturn> Retrieve(string quoteReference);
    }
}
