namespace Web.Template.Application.User.Models
{
    using System;
    using System.Linq;
    using System.Web;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.User;
    using Web.Template.Application.User.Enums;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Entities.Payment;
    using Web.Template.Domain.Entities.Site;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Payment;

    /// <summary>
    /// Class concerning all information about the active user.
    /// </summary>
    public class UserSession : IUserSession
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserSession" /> class.
        /// </summary>
        /// <param name="siteService">The site service.</param>
        /// <param name="languageRepository">The language repository.</param>
        /// <param name="currencyRepository">The currency repository.</param>
        /// <param name="tradeService">The trade service.</param>
        public UserSession(ISiteService siteService, ILanguageRepository languageRepository, ICurrencyRepository currencyRepository, ITradeService tradeService)
        {
            ISite site = siteService.GetSite(HttpContext.Current);
            this.SelectedLanguage = languageRepository.GetAll().FirstOrDefault(l => l.CultureCode == site.DefaultLanguageCode);
            this.SelectCurrency = currencyRepository.GetAll().FirstOrDefault(c => c.CurrencyCode == site.DefaultCurrencyCode);
            
            this.AorB = AorB.A;
            this.AdminMode = false;
            this.AdminSession = new AdminSession();
            this.Guid = Guid.NewGuid();
            this.TradeSession = new TradeSession();
            this.IncludePaymentDetails = true;

            if (HttpContext.Current != null)
            {
                this.Browser = HttpContext.Current.Request.Browser.Browser;
                this.BrowserVersion = HttpContext.Current.Request.Browser.MajorVersion;

                var affiliateId = HttpContext.Current.Request.QueryString["affiliateid"];
                if (!string.IsNullOrEmpty(affiliateId))
                {
                    var trade = tradeService.GetTradeById(int.Parse(affiliateId));

                    if (trade != null)
                    {
                        this.OverBranded = true;
                        this.TradeSession.ABTAATOL = trade.ABTAATOLNumber;
                        this.TradeSession.Trade = trade;
                        this.TradeSession.TradeId = trade.Id;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSession"/> class.
        /// Default constructor used for serialization
        /// </summary>
        public UserSession()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether [admin mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [admin mode]; otherwise, <c>false</c>.
        /// </value>
        public bool AdminMode { get; set; }

        /// <summary>
        /// Gets or sets the admin session.
        /// </summary>
        /// <value>The admin session.</value>
        public AdminSession AdminSession { get; set; }

        /// <summary>
        /// Gets or sets the a or b.
        /// </summary>
        /// <value>
        /// The a or b.
        /// </value>
        public AorB AorB { get; set; }

        /// <summary>
        /// Gets or sets the brand identifier.
        /// </summary>
        /// <value>
        /// The brand identifier.
        /// </value>
        public int BrandId { get; set; }

        /// <summary>
        /// Gets or sets the browser.
        /// </summary>
        /// <value>The browser.</value>
        public string Browser { get; set; }

        /// <summary>
        /// Gets or sets the browser version.
        /// </summary>
        /// <value>The browser version.</value>
        public int BrowserVersion { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid Guid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [include payment details].
        /// </summary>
        /// <value>
        /// <c>true</c> if [include payment details]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludePaymentDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UserSession"/> is over branded.
        /// </summary>
        /// <value><c>true</c> if over branded; otherwise, <c>false</c>.</value>
        public bool OverBranded { get; set; }

        /// <summary>
        /// Gets or sets the select currency.
        /// </summary>
        /// <value>
        /// The select currency.
        /// </value>
        public Currency SelectCurrency { get; set; }

        /// <summary>
        /// Gets or sets the selected CMS website.
        /// </summary>
        /// <value>
        /// The selected CMS website.
        /// </value>
        public CmsWebsite SelectedCmsWebsite { get; set; }

        /// <summary>
        /// Gets or sets the selected language.
        /// </summary>
        /// <value>
        /// The selected language.
        /// </value>
        public Language SelectedLanguage { get; set; }

        /// <summary>
        /// Gets or sets the site builder authorization.
        /// </summary>
        /// <value>The site builder authorization.</value>
        public string SiteBuilderAuthorization { get; set; }

        /// <summary>
        /// Gets or sets the trade session.
        /// </summary>
        /// <value>
        /// The trade session.
        /// </value>
        public TradeSession TradeSession { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [logged in].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [logged in]; otherwise, <c>false</c>.
        /// </value>
        [Obsolete]
        public bool LoggedIn
        {
            get
            {
                bool loggedInAdmin = this.AdminSession != null && this.AdminSession.LoggedIn;
                bool loggedInTrade = this.TradeSession != null && this.TradeSession.LoggedIn;

                var loggedIn = loggedInAdmin || loggedInTrade;
                return loggedIn;
            }
            set
            {
            }
        }

        /// <summary>
        /// Logouts this instance.
        /// </summary>
        public void Logout()
        {
            this.TradeSession = new TradeSession() { LoggedIn = false };
        }

        /// <summary>
        /// Logouts the admin.
        /// </summary>
        public void LogoutAdmin()
        {
            this.AdminMode = false;
            this.AdminSession = new AdminSession() { LoggedIn = false };
        }
    }
}