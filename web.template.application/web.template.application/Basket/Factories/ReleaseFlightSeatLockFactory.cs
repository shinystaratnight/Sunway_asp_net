namespace Web.Template.Application.Basket.Factories
{
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Basket;
    using Web.Template.Application.IVectorConnect.Requests;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class ReleaseFlightSeatLockFactory.
    /// </summary>
    public class ReleaseFlightSeatLockFactory : IReleaseFlightSeatLockFactory
    {
        /// <summary>
        /// The login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory loginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReleaseFlightSeatLockFactory"/> class.
        /// </summary>
        /// <param name="loginDetailsFactory">The login details factory.</param>
        public ReleaseFlightSeatLockFactory(IConnectLoginDetailsFactory loginDetailsFactory)
        {
            this.loginDetailsFactory = loginDetailsFactory;
        }

        /// <summary>
        /// Creates the specified release flight seat lock model.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>The ivector connect request</returns>
        public iVectorConnectRequest Create(string basketToken)
        {
            iVectorConnectRequest releaseFlightSeatLockRequest = new ivci.Flight.ReleaseFlightSeatLockRequest()
            {
                LoginDetails = this.loginDetailsFactory.Create(HttpContext.Current),
                SessionID = basketToken
            };
            return releaseFlightSeatLockRequest;
        }
    }
}
