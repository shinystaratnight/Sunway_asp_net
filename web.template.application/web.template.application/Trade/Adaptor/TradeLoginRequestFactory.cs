namespace Web.Template.Application.Trade.Adaptor
{
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Support;
    using Web.Template.Application.Trade.Models;

    /// <summary>
    /// Class Responsible for building trade login requests.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Trade.Adaptor.ITradeLoginRequestFactory" />
    public class TradeLoginRequestFactory : ITradeLoginRequestFactory
    {
        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeLoginRequestFactory" /> class.
        /// </summary>
        /// <param name="siteService">The site service.</param>
        public TradeLoginRequestFactory(ISiteService siteService)
        {
            this.siteService = siteService;
        }

        /// <summary>
        /// Builds the connect request from model.
        /// </summary>
        /// <param name="tradeLoginModel">The trade login model.</param>
        /// <returns>
        /// A trade login model
        /// </returns>
        public iVectorConnectRequest Create(ITradeLoginModel tradeLoginModel)
        {
            ISite site = this.siteService.GetSite(HttpContext.Current);
            iVectorConnectRequest tradeLoginRequestBody = new iVectorConnectInterface.TradeLoginRequest()
                                                              {
                                                                  LoginDetails =
                                                                      new iVectorConnectInterface.LoginDetails
                                                                          {
                                                                              Password = site.IvectorConnectPassword, 
                                                                              Login = site.IvectorConnectUsername, 
                                                                              AgentReference = tradeLoginModel.UserName
                                                                          }, 
                                                                  Email = tradeLoginModel.EmailAddress, 
                                                                  Password = tradeLoginModel.Password, 
                                                                  UserName = tradeLoginModel.UserName, 
                                                                  WebsitePassword = tradeLoginModel.WebsitePassword
                                                              };
            return tradeLoginRequestBody;
        }
    }
}