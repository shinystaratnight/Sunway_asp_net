namespace Web.Template.Application.Interfaces.Quote.Services
{
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Quote.Models;

    /// <summary>
    /// Defines a class responsible for creating a quote from basket.
    /// </summary>
    public interface IBasketQuoteCreateService
    {
        /// <summary>
        /// Creates a quote from the specified basket.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <returns>The quote create return.</returns>
        IQuoteCreateReturn Create(IBasket basket);
    }
}
