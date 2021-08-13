namespace Web.Template.Application.Services
{
    using System;
    using System.Web;

    using Intuitive;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.User;
    using Web.Template.Application.User.Models;
    using Web.Template.Domain.Entities.Payment;
    using Web.Template.Domain.Entities.Site;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Payment;

    /// <summary>
    ///     Service used to carry out user operations such as logging in
    /// </summary>
    public class UserService : IUserService
    {
        /// <summary>
        /// The user session key
        /// </summary>
        private const string UserSessionKey = "userSession";

        /// <summary>
        /// The currency repository
        /// </summary>
        private readonly ICurrencyRepository currencyRepository;

        /// <summary>
        /// The language repository
        /// </summary>
        private readonly ILanguageRepository languageRepository;

        /// <summary>
        /// The site configuration
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// The trade service
        /// </summary>
        private readonly ITradeService tradeService;

        /// <summary>
        /// The log writer
        /// </summary>
        private readonly ILogWriter logWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService" /> class.
        /// </summary>
        /// <param name="siteService">The site service.</param>
        /// <param name="tradeService">The trade service.</param>
        /// <param name="currencyRepository">The currency repository.</param>
        /// <param name="languageRepository">The language repository.</param>
        public UserService(ISiteService siteService, ITradeService tradeService, ICurrencyRepository currencyRepository, ILanguageRepository languageRepository, ILogWriter logWriter)
        {
            this.siteService = siteService;
            this.tradeService = tradeService;
            this.currencyRepository = currencyRepository;
            this.languageRepository = languageRepository;
            this.logWriter = logWriter;
        }

        /// <summary>
        /// Gets the name of the trade cooking.
        /// </summary>
        /// <value>
        /// The name of the trade cooking.
        /// </value>
        public string UserCookieName => "__UserDetails";

        /// <summary>
        /// Gets or sets the site.
        /// </summary>
        /// <value>
        /// The site.
        /// </value>
        private ISite Site { get; set; }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A user session
        /// </returns>
        public IUserSession GetUser(HttpContext context)
        {
            IUserSession userSession = null;

            if (context != null)
            {
                try
                {
                    userSession = this.GetUserFromCookie(context);
                }
                catch (Exception ex)
                {
                    Intuitive.FileFunctions.AddLogEntry("UserService", "User Service Exception", ex.ToString());
                }
            }

            return userSession;
        }

        /// <summary>
        /// Logins the trade.
        /// </summary>
        /// <param name="tradeSession">The trade session.</param>
        public void LoginTrade(ITradeSession tradeSession)
        {
            var tradeConcrete = (TradeSession)tradeSession;
            IUserSession user = this.GetUser(HttpContext.Current);
            user.TradeSession = tradeConcrete;
            user.IncludePaymentDetails = false;
            this.SetUser(user, HttpContext.Current);
        }

        /// <summary>
        /// Sets the brand on user.
        /// </summary>
        /// <param name="brandId">The brand identifier.</param>
        public void SetBrandOnUser(int brandId)
        {
            IUserSession user = this.GetUser(HttpContext.Current);
            user.BrandId = brandId;
            this.SetUser(user, HttpContext.Current);
        }


        /// <summary>
        /// Logs out the user.
        /// </summary>
        public void LogOut()
        {
            IUserSession user = this.GetUser(HttpContext.Current);
            user.Logout();
            this.SetUser(user, HttpContext.Current);
            Intuitive.CookieFunctions.Cookies.Remove(this.tradeService.TradeCookieName);
        }

        /// <summary>
        /// Logs the out admin.
        /// </summary>
        public void LogOutAdmin()
        {
            IUserSession user = this.GetUser(HttpContext.Current);
            user.LogoutAdmin();
            this.SetUser(user, HttpContext.Current);
        }

        /// <summary>
        /// News the user.
        /// </summary>
        /// <param name="website">The cms website</param>
        public void NewUser(CmsWebsite website = null)
        {
            var userSession = new UserSession(this.siteService, this.languageRepository, this.currencyRepository, this.tradeService);
            userSession.SelectedCmsWebsite = website;
            this.SetUser(userSession, HttpContext.Current);

            if (string.IsNullOrEmpty(userSession.TradeSession?.ABTAATOL))
            {
                var loginReturn = this.tradeService.LoginFromCookie();

                if (loginReturn != null && loginReturn.LoginSuccessful)
                {
                    this.LoginTrade(loginReturn.TradeSession);
                }
            }
        }

        /// <summary>
        /// Sets the selected CMS website, make sure we also update the selected currency.
        /// </summary>
        /// <param name="website">The website.</param>
        public void SetSelectedCmsWebsite(CmsWebsite website)
        {
            IUserSession user = this.GetUser(HttpContext.Current);
            if (user != null)
            {
                user.SelectedCmsWebsite = website;
                this.SetUser(user, HttpContext.Current);
                this.SetSelectedCurrency(website.CurrencyId);
            }

        }

        /// <summary>
        /// Sets the selected currency.
        /// </summary>
        /// <param name="currencyID">The currency identifier.</param>
        public void SetSelectedCurrency(int currencyID)
        {
            IUserSession user = this.GetUser(HttpContext.Current);

            if (user != null)
            {
                Currency currency = this.currencyRepository.GetSingle(currencyID);
                if (currency != null)
                {
                    user.SelectCurrency = currency;
                    this.SetUser(user, HttpContext.Current);
                }
            }
        }

        /// <summary>
        /// Sets the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="context">The context.</param>
        public void SetUser(IUserSession user, HttpContext context)
        {
            if (context != null)
            {
                if (HttpContext.Current.Session[UserSessionKey] == null)
                {
                    HttpContext.Current.Session.Add("userSession", user);
                }
                else
                {
                    context.Session[UserSessionKey] = user;
                }

#if DEBUG
                string cookie = Intuitive.Functions.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(user, new Newtonsoft.Json.Converters.StringEnumConverter()));
                Intuitive.CookieFunctions.Cookies.SetValue(this.UserCookieName, cookie, Intuitive.CookieFunctions.CookieExpiry.OneWeek);
#else
                var userContent = Newtonsoft.Json.JsonConvert.SerializeObject(user, new Newtonsoft.Json.Converters.StringEnumConverter());
                var cookieContent = Intuitive.Functions.Encrypt(userContent);
                var cookie = Intuitive.CookieFunctions.Cookies.CreateCookie(this.UserCookieName, Intuitive.CookieFunctions.CookieExpiry.OneWeek);
                cookie.SameSite = SameSiteMode.None;
                cookie.Secure = true;
                cookie.Value = cookieContent;
#endif
            }
        }

        /// <summary>
        /// Sets the user from cookie.
        /// </summary>
        /// <param name="website">The cms website</param>
        public void SetUserFromCookie(CmsWebsite website = null)
        {
            string userCookie = Intuitive.CookieFunctions.Cookies.GetValue(this.UserCookieName);
            if (!string.IsNullOrEmpty(userCookie))
            {
                string decryptedCookie = Intuitive.Functions.Decrypt(userCookie);
                var userSession = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSession>(decryptedCookie, new Newtonsoft.Json.Converters.StringEnumConverter());

                this.SetUser(userSession, HttpContext.Current);
            }
            else
            {
                this.NewUser(website);
            }
        }

        /// <summary>
        /// Gets the user from cookie.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>the user session object</returns>
        private IUserSession GetUserFromCookie(HttpContext context)
        {
            IUserSession userSession;
            try
            {
                string userCookie = Intuitive.CookieFunctions.Cookies.GetValueFromContext(context, this.UserCookieName);
                userSession = new UserSession();
                userSession.AdminSession = new AdminSession();
                userSession.TradeSession = new TradeSession();
                if (!string.IsNullOrEmpty(userCookie))
                {
                    string decryptedCookie = Intuitive.Functions.Decrypt(userCookie);
                    userSession = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSession>(decryptedCookie, new Newtonsoft.Json.Converters.StringEnumConverter());

                    var affiliateId = context.Request.QueryString["affiliateid"];
                   if (!String.IsNullOrEmpty(affiliateId) && 
                            (!userSession.OverBranded || userSession.TradeSession.TradeId != Int32.Parse(affiliateId)))
                    {
                        this.NewUser();
                        userSession = this.GetUser(HttpContext.Current);
                    }
                }
            }
            catch (Exception ex)
            {
                this.NewUser();
                userSession = this.GetUser(HttpContext.Current);
                this.logWriter.Write("user server", "Error in getting user from cookie", ex.ToString());
            }

            return userSession;
        }
    }
}