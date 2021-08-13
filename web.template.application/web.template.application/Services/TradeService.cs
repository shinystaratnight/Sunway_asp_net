namespace Web.Template.Application.Services
{
    using System.Linq;

    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.Trade;
    using Web.Template.Application.Trade.Models;
    using Web.Template.Domain.Entities.Booking;

    /// <summary>
    /// Trade service responsible for actions regarding a trade, such as logging in and out.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Services.ITradeService" />
    public class TradeService : ITradeService
    {
        /// <summary>
        /// The trade contact group
        /// </summary>
        private readonly ITradeContactGroupRepository tradeContactGroupRepository;

        /// <summary>
        /// The trade contact repository
        /// </summary>
        private readonly ITradeContactRepository tradeContactRepository;

        /// <summary>
        /// The trade group repository
        /// </summary>
        private readonly ITradeGroupRepository tradeGroupRepository;

        /// <summary>
        /// The trade login adaptor
        /// </summary>
        private readonly ITradeLoginAdaptor tradeLoginAdaptor;

        /// <summary>
        /// The trade contact repository
        /// </summary>
        private readonly ITradeRepository tradeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeService" /> class.
        /// </summary>
        /// <param name="tradeLoginAdaptor">The trade login adaptor.</param>
        /// <param name="tradeContactRepository">The trade contact repository.</param>
        /// <param name="tradeRepository">The trade repository.</param>
        /// <param name="tradeContactGroup">The trade contact group.</param>
        /// <param name="tradeGroupRepository">The trade group repository.</param>
        public TradeService(ITradeLoginAdaptor tradeLoginAdaptor, ITradeContactRepository tradeContactRepository, ITradeRepository tradeRepository, ITradeContactGroupRepository tradeContactGroup, ITradeGroupRepository tradeGroupRepository)
        {
            this.tradeLoginAdaptor = tradeLoginAdaptor;
            this.tradeContactRepository = tradeContactRepository;
            this.tradeRepository = tradeRepository;
            this.tradeContactGroupRepository = tradeContactGroup;
            this.tradeGroupRepository = tradeGroupRepository;
        }

        /// <summary>
        /// Gets the trade by identifier.
        /// </summary>
        /// <param name="tradeId">The trade identifier.</param>
        /// <returns>the trade that matches the provided id.</returns>
        public Trade GetTradeById(int tradeId)
        {
            return this.tradeRepository.GetSingle(tradeId);
        }

        /// <summary>
        /// Gets the name of the trade cooking.
        /// </summary>
        /// <value>
        /// The name of the trade cooking.
        /// </value>
        public string TradeCookieName => "__TradeLoginDetails";

        /// <summary>
        /// Logins the specified email address.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="websitepassword">The website password.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="saveDetails">if set to <c>true</c> [save details].</param>
        /// <returns>
        /// Whether we were able to login successfully.
        /// </returns>
        public ITradeLoginReturn Login(string username = "", string password = "", string websitepassword = "", string emailAddress = "", bool saveDetails = true)
        {
            var tradeLoginModel = new TradeLoginModel() { EmailAddress = emailAddress, Password = password, WebsitePassword = websitepassword, UserName = username, SaveDetails = saveDetails };
            var tradeLoginReturn = this.Login(tradeLoginModel);

            return tradeLoginReturn;
        }

        /// <summary>
        /// Logins the specified login model.
        /// </summary>
        /// <param name="loginmodel">The login model.</param>
        /// <returns>
        /// A trade login return
        /// </returns>
        public ITradeLoginReturn Login(ITradeLoginModel loginmodel)
        {
            ITradeLoginReturn tradeLoginReturn = this.tradeLoginAdaptor.Login(loginmodel);

            if (tradeLoginReturn.LoginSuccessful)
            {
                tradeLoginReturn.TradeSession.LoggedIn = true;

                if (tradeLoginReturn.TradeSession.TradeContactId > 0)
                {
                    TradeContact tradeContact = this.tradeContactRepository.GetSingle(tradeLoginReturn.TradeSession.TradeContactId);

                    TradeContactGroup tradeContactGroup = this.tradeContactGroupRepository.GetSingle(tradeContact.TradeContactGroupId);

                    tradeLoginReturn.TradeSession.TradeContactGroup = tradeContactGroup;
                    tradeLoginReturn.TradeSession.TradeContact = tradeContact;
                }

                if (tradeLoginReturn.TradeSession.TradeId > 0)
                {
                    Trade trade = this.tradeRepository.GetSingle(tradeLoginReturn.TradeSession.TradeId);
                    tradeLoginReturn.TradeSession.ABTAATOL = trade.ABTAATOLNumber;
                    tradeLoginReturn.TradeSession.Trade = trade;

                    if (trade.TradeGroupId > 0)
                    {
                        TradeGroup tradeGroup = this.tradeGroupRepository.GetSingle(trade.TradeGroupId);
                        tradeLoginReturn.TradeSession.TradeGroup = tradeGroup;
                    }
                }

                if (loginmodel.SaveDetails)
                {
                    string cookie = Intuitive.Functions.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(loginmodel, new Newtonsoft.Json.Converters.StringEnumConverter()));
                    Intuitive.CookieFunctions.Cookies.SetValue(this.TradeCookieName, cookie, Intuitive.CookieFunctions.CookieExpiry.OneWeek);
                }
            }

            return tradeLoginReturn;
        }

        /// <summary>
        /// Logins from cookie.
        /// </summary>
        /// <returns>
        /// A trade login return
        /// </returns>
        public ITradeLoginReturn LoginFromCookie()
        {
            ITradeLoginReturn tradeLoginReturn = null;

            string tradeCookie = Intuitive.CookieFunctions.Cookies.GetValue(this.TradeCookieName);

            if (!string.IsNullOrEmpty(tradeCookie))
            {
                string decryptedCookie = Intuitive.Functions.Decrypt(tradeCookie);
                var loginModel = Newtonsoft.Json.JsonConvert.DeserializeObject<TradeLoginModel>(decryptedCookie, new Newtonsoft.Json.Converters.StringEnumConverter());

                tradeLoginReturn = this.Login(loginModel);
            }

            return tradeLoginReturn;
        }
    }
}