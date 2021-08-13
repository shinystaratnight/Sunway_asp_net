namespace Web.Template.Application.Interfaces.Promocode
{
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// service responsible for managing promotional codes
    /// </summary>
    public interface IPromoCodeService
    {
        /// <summary>
        /// Applies the promotional code.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="promocode">The promotional code.</param>
        /// <returns>a promotional code return</returns>
        IPromoCodeReturn ApplyPromocode(IBasket basket, string promocode);

        /// <summary>
        /// Removes the promotional code.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <returns>The basket</returns>
        IBasket RemovePromocode(IBasket basket);
    }
}