namespace Web.Template.Application.Trade.Adaptor
{
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Trade;
    using Web.Template.Application.Net.IVectorConnect;
    using Web.Template.Application.Trade.Models;

    using ivci = iVectorConnectInterface;

    /// <summary>
    ///  Class responsible for talking to connect regarding the login and returns a trade session object.
    /// </summary>
    /// <seealso cref="ITradeLoginAdaptor" />
    public class ConnectTradeLoginAdaptor : ITradeLoginAdaptor
    {
        /// <summary>
        /// The connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory connectRequestFactory;

        /// <summary>
        /// The trade login request factory
        /// </summary>
        private readonly ITradeLoginRequestFactory tradeLoginRequestFactory;

        /// <summary>
        /// The trade session factory
        /// </summary>
        private ITradeSessionFactory tradeSessionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectTradeLoginAdaptor" /> class.
        /// </summary>
        /// <param name="connectRequestFactory">The i vector connect request factory.</param>
        /// <param name="tradeSessionFactory">The trade session factory.</param>
        /// <param name="tradeLoginRequestFactory">The trade login request factory.</param>
        public ConnectTradeLoginAdaptor(IIVectorConnectRequestFactory connectRequestFactory, ITradeSessionFactory tradeSessionFactory, ITradeLoginRequestFactory tradeLoginRequestFactory)
        {
            this.connectRequestFactory = connectRequestFactory;
            this.tradeSessionFactory = tradeSessionFactory;
            this.tradeLoginRequestFactory = tradeLoginRequestFactory;
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="tradeLoginRequestBody">The trade login request body.</param>
        /// <param name="tradeLoginReturn">The trade login return.</param>
        public void GetResponse(iVectorConnectRequest tradeLoginRequestBody, ITradeLoginReturn tradeLoginReturn)
        {
            IIVectorConnectRequest ivcRequest = this.connectRequestFactory.Create(tradeLoginRequestBody, HttpContext.Current);
            ivci.TradeLoginResponse loginResponse = ivcRequest.Go<ivci.TradeLoginResponse>();

            tradeLoginReturn.LoginSuccessful = loginResponse.ReturnStatus.Success;
            tradeLoginReturn.Warnings.AddRange(loginResponse.ReturnStatus.Exceptions);

            if (tradeLoginReturn.Warnings.Count == 0)
            {
                tradeLoginReturn.TradeSession = this.tradeSessionFactory.Create(loginResponse.ABTAATOL, loginResponse.Commissionable, loginResponse.CreditCardAgent, loginResponse.TradeContactID, loginResponse.TradeID);
            }
        }

        /// <summary>
        /// Builds the trade login request.
        /// </summary>
        /// <param name="tradeLoginModel">The trade login model.</param>
        /// <returns>A trade session object</returns>
        public ITradeLoginReturn Login(ITradeLoginModel tradeLoginModel)
        {
            ITradeLoginReturn tradeLoginReturn = new TradeLoginReturn() { LoginSuccessful = false };
            iVectorConnectRequest tradeLoginRequestBody = this.tradeLoginRequestFactory.Create(tradeLoginModel);
            tradeLoginReturn.Warnings = tradeLoginRequestBody.Validate();

            if (tradeLoginReturn.Warnings.Count == 0)
            {
                this.GetResponse(tradeLoginRequestBody, tradeLoginReturn);
            }

            return tradeLoginReturn;
        }
    }
}