namespace Web.Template.Application.IVectorConnect.Requests
{
    using System;
    using System.Web;

    using iVectorConnectInterface;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Application.Support;

    /// <summary>
    /// Class responsible for building connect details.
    /// </summary>
    /// <seealso cref="Web.Template.Application.IVectorConnect.Requests.IConnectLoginDetailsFactory" />
    public class ConnectLoginDetailsFactory : IConnectLoginDetailsFactory
    {
        /// <summary>
        /// The site
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// The user service
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectLoginDetailsFactory" /> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="siteService">The site service.</param>
        public ConnectLoginDetailsFactory(IUserService userService, ISiteService siteService)
        {
            this.userService = userService;
            this.siteService = siteService;
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="useContentDetails"></param>
        /// <returns>
        /// A new connect login details
        /// </returns>
        public LoginDetails Create(HttpContext context, bool useContentDetails = false)
        {
            var site = this.siteService.GetSite(context);
            if (context.Request.QueryString["bid"] != null)
            {
                var brandId = 0;
                int.TryParse(context.Request.QueryString["bid"], out brandId);
                this.userService.SetBrandOnUser(brandId);
            }
            var login = useContentDetails ? site.IvectorConnectContentUsername : site.IvectorConnectUsername;
            var password = useContentDetails ? site.IvectorConnectContentPassword : site.IvectorConnectPassword;

            var user = this.userService.GetUser(context);

            var loginDetails = new LoginDetails();
            try
            {
                loginDetails.Login = login;
                loginDetails.Password = password;
                loginDetails.AgentReference = user.TradeSession.ABTAATOL ?? string.Empty;
                loginDetails.SellingCurrencyID = user.SelectCurrency?.SellingCurrencyId ?? 1;
                loginDetails.LanguageID = user.SelectedLanguage?.Id ?? 1;
                if (user.BrandId > 0)
                {
                    loginDetails.BrandID = user.BrandId;
                }
            }
            catch (Exception ex)
            {
                Intuitive.FileFunctions.AddLogEntry("LoginDetailsFactory", "loginDetailsFactory Error", ex.ToString());
            }

            return loginDetails;
        }
    }
}