namespace Web.Template.Application.Interfaces.Book
{
    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Interface defining the processor of the prebook response.
    /// </summary>
    public interface IBookResponseProcessor
    {
        /// <summary>
        /// Processes the specified pre book response.
        /// </summary>
        /// <param name="bookResponse">The pre book response.</param>
        /// <param name="basket">The basket.</param>
        void Process(BookResponse bookResponse, IBasket basket);
    }
}