namespace Web.Template.Application.Interfaces.Prebook
{
    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Interface defining the processor of the prebook response.
    /// </summary>
    public interface IPrebookResponseProcessor
    {
        /// <summary>
        /// Processes the specified pre book response.
        /// </summary>
        /// <param name="preBookResponse">The pre book response.</param>
        /// <param name="basket">The basket.</param>
        /// <param name="basketComponent">The basket component.</param>
        void Process(PreBookResponse preBookResponse, IBasket basket, IBasketComponent basketComponent);
    }
}