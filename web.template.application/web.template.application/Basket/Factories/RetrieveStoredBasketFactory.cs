namespace Web.Template.Application.Basket.Factories
{
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Basket;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.IVectorConnect.Requests;
    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class RetrieveBasketFactory.
    /// </summary>
    public class RetrieveStoredBasketFactory : IRetrieveStoredBasketFactory
    {
        /// <summary>
        /// The login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory loginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetrieveStoredBasketFactory"/> class.
        /// </summary>
        /// <param name="loginDetailsFactory">The login details factory.</param>
        public RetrieveStoredBasketFactory(IConnectLoginDetailsFactory loginDetailsFactory)
        {
            this.loginDetailsFactory = loginDetailsFactory;
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <param name="basketStoreId">The basket store identifier.</param>
        /// <returns>The iVectorConnectRequest.</returns>
        public iVectorConnectRequest Create(int basketStoreId)
        {
            iVectorConnectRequest storeBasketRequest = new ivci.RetrieveStoredBasketRequest()
            {
                LoginDetails = this.loginDetailsFactory.Create(HttpContext.Current),
                BasketStoreID = basketStoreId
            };
            return storeBasketRequest;
        }
    }
}
