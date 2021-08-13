namespace Web.Template.Application.Interfaces.Book
{
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Defines a class responsible for booking a basket.
    /// </summary>
    public interface IBasketBookService
    {
        /// <summary>
        /// Books the specified basket.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <returns>A book return</returns>
        IBookReturn Book(IBasket basket);
    }
}