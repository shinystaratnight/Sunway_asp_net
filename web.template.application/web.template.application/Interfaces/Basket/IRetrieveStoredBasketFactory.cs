namespace Web.Template.Application.Interfaces.Basket
{
    using iVectorConnectInterface.Interfaces;

    /// <summary>
    /// Interface IRetrieveBasketFactory
    /// </summary>
    public interface IRetrieveStoredBasketFactory
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <param name="basketStoreId">The basket store identifier.</param>
        /// <returns>The iVectorConnectRequest.</returns>
        iVectorConnectRequest Create(int basketStoreId);
    }
}
