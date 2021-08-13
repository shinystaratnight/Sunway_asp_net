namespace Web.Template.Application.Interfaces.Repositories
{
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    ///     Interface defining what a basket repository can do.
    /// </summary>
    public interface IBasketRepository
    {
        /// <summary>
        /// Adds the new basket.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="basket">The basket.</param>
        void AddNewBasket(string token, IBasket basket);

        /// <summary>
        ///     Retrieves the basket by token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>A Basket</returns>
        IBasket RetrieveBasketByToken(string token);

        /// <summary>
        /// Updates the basket.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="basket">The basket.</param>
        void UpdateBasket(string token, IBasket basket);
    }
}