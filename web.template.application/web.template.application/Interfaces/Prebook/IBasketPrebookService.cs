namespace Web.Template.Application.Interfaces.Prebook
{
    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Defines a class responsible for the prebook of a basket.
    /// </summary>
    public interface IBasketPrebookService
    {
        /// <summary>
        /// Creates the specified basket.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="componentToken">The component token.</param>
        /// <returns>a prebook return</returns>
        IPrebookReturn Prebook(IBasket basket, int componentToken = 0);

        /// <summary>
        /// Builds the prebook request.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="basketComponent">The basket component.</param>
        /// <returns></returns>
        PreBookRequest BuildPrebookRequest(IBasket basket, IBasketComponent basketComponent);

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="connectRequestBody">The connect request body.</param>
        /// <returns></returns>
        PreBookResponse GetResponse(PreBookRequest connectRequestBody);
    }
}