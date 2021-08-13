namespace Web.Template.Application.Interfaces.Basket
{
    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Interface IStoreBasketFactory
    /// </summary>
    public interface IStoreBasketFactory
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="basketStoreId">The basket store identifier.</param>
        /// <returns>The iVectorConnectRequest.</returns>
        iVectorConnectRequest Create(IBasket basket, int basketStoreId);
    }
}
