namespace Web.Template.Application.Interfaces.Quote.Services
{
    using Web.Template.Application.Interfaces.Quote.Models;

    /// <summary>
    /// Interface IQuoteSearchService
    /// </summary>
    public interface IQuoteSearchService
    {
        /// <summary>
        /// Searches this instance.
        /// </summary>
        /// <param name="quoteSearch">The quote search.</param>
        /// <returns>The Quote Search Return.</returns>
        IQuoteSearchReturn Search(IQuoteSearch quoteSearch);
    }
}
