namespace Web.Template.Application.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Repositories;

    /// <summary>
    ///     Basket repository used to retrieve stored baskets
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Repositories.IBasketRepository" />
    public class BasketRepository : IBasketRepository
    {
        /// <summary>
        /// Adds the new basket.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="basket">The basket.</param>
        public void AddNewBasket(string token, IBasket basket)
        {
            string cacheKey = $"Basket_{token}";
            HttpRuntime.Cache.Insert(cacheKey, basket, null, DateTime.Now.AddMinutes(60), TimeSpan.Zero);
        }

        /// <summary>
        /// Retrieves the basket by token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        /// A Basket
        /// </returns>
        public IBasket RetrieveBasketByToken(string token)
        {
            string cacheKey = $"Basket_{token}";
            var basket = HttpRuntime.Cache[cacheKey] as IBasket
                         ?? new Basket() { Components = new List<IBasketComponent> { }, AllComponentsBooked = false, AllComponentsPreBooked = false, BookingReference = string.Empty, ExternalReference = string.Empty };
            return basket;
        }

        /// <summary>
        /// Updates the basket.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="basket">The basket.</param>
        public void UpdateBasket(string token, IBasket basket)
        {
            string cacheKey = $"Basket_{token}";
            HttpRuntime.Cache.Insert(cacheKey, basket, null, DateTime.Now.AddMinutes(60), TimeSpan.Zero);
        }
    }
}