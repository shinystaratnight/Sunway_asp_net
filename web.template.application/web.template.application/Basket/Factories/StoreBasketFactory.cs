namespace Web.Template.Application.Basket.Factories
{
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Newtonsoft.Json;

    using Web.Template.Application.Interfaces.Basket;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.IVectorConnect.Requests;
    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class StoreBasketFactory.
    /// </summary>
    public class StoreBasketFactory : IStoreBasketFactory
    {
        /// <summary>
        /// The login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory loginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreBasketFactory" /> class.
        /// </summary>
        /// <param name="loginDetailsFactory">The login details factory.</param>
        public StoreBasketFactory(IConnectLoginDetailsFactory loginDetailsFactory)
        {
            this.loginDetailsFactory = loginDetailsFactory;
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="basketStoreId">The basket store identifier.</param>
        /// <returns>The iVectorConnectRequest.</returns>
        public iVectorConnectRequest Create(IBasket basket, int basketStoreId)
        {
            iVectorConnectRequest storeBasketRequest = new ivci.StoreBasketRequest()
            {
                LoginDetails = this.loginDetailsFactory.Create(HttpContext.Current),
                BasketXML = JsonConvert.SerializeObject(basket, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }),
                BasketStoreID = basketStoreId
            };
            return storeBasketRequest;
        }
    }
}
