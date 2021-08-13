namespace Web.Template.Application.Interfaces.Services
{
    using System.Threading.Tasks;
    using Web.Template.Application.Interfaces.Quote.Models;
    using Web.Template.Application.Models;
    using Web.Template.Application.Quote.Models;
    using Web.Template.Application.Services;

    /// <summary>
    /// Interface IQuoteService
    /// </summary>
    public interface IQuoteService
    {
        /// <summary>
        /// Creates the specified basket token.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>The quote create return.</returns>
        IQuoteCreateReturn Create(string basketToken);

        /// <summary>
        /// Creates a pdf document from a given quote
        /// </summary>
        /// <param name="quote">The quote</param>
        /// <returns>The path to the created pdf.</returns>
        DocumentServiceReturn CreatePDF(QuoteModel quote);

        /// <summary>
        /// Sends an email created from the given quote
        /// </summary>
        /// <param name="quote">the quote</param>
        /// <returns>The success of the email send</returns>
        bool Email(QuoteModel quote);

        /// <summary>
        /// Retrieves the specified quote reference.
        /// </summary>
        /// <param name="quoteReference">The quote reference.</param>
        /// <returns>The Quote Retrieve Return.</returns>
        Task<QuoteRetrieveReturn> Retrieve(string quoteReference);

        /// <summary>
        /// Searches the specified quote search.
        /// </summary>
        /// <param name="quoteSearch">The quote search.</param>
        /// <returns>The Quote Search Return.</returns>
        IQuoteSearchReturn Search(IQuoteSearch quoteSearch);
    }
}
