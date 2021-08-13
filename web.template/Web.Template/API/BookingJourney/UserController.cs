namespace Web.Template.API.BookingJourney
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Cors;

    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Data.Site;
    using Web.Template.Domain.Entities.Site;
    using Web.Template.Models.Application;

    /// <summary>
    ///     Controller used to expose to the user interface operations concerning users such as logging in
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        /// <summary>
        /// The trade service
        /// </summary>
        private readonly ITradeService tradeService;

        /// <summary>
        /// The user service
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// The website repository
        /// </summary>
        private readonly IWebsiteRepository websiteRepository;

        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController" /> class.
        /// </summary>
        /// <param name="tradeService">The trade service.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="websiteRepository">The website repository.</param>
        /// <param name="siteService">The site service.</param>
        public UserController(ITradeService tradeService, IUserService userService, IWebsiteRepository websiteRepository, ISiteService siteService)
        {
            this.tradeService = tradeService;
            this.userService = userService;
            this.websiteRepository = websiteRepository;
            this.siteService = siteService;
        }

        /// <summary>
        /// Customers the login.
        /// </summary>
        /// <returns>
        /// Whether we were able to login successfully
        /// </returns>
        /// <exception cref="System.NotImplementedException">not implemented exception</exception>
        [Route("api/user/login/customer")]
        [HttpGet]
        public async Task<bool> CustomerLogin()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <returns>the user session view model</returns>
        [Route("api/user")]
        public SessionViewModel GetUser()
        {
            var viewModel = new SessionViewModel() { UserSession = this.userService.GetUser(HttpContext.Current), Success = true };
            return viewModel;
        }

        /// <summary>
        /// Logouts this instance.
        /// </summary>
        /// <returns>the user session view model</returns>
        [Route("api/user/logout")]
        [HttpGet]
        public SessionViewModel LogOut()
        {
            this.userService.LogOut();
            var viewModel = new SessionViewModel() { UserSession = this.userService.GetUser(HttpContext.Current), Success = true };
            return viewModel;
        }

        /// <summary>
        /// Logs the out.
        /// </summary>
        /// <returns>the user session view model</returns>
        [Route("api/user/logoutadmin")]
        [HttpGet]
        public SessionViewModel LogOutAdmin()
        {
            this.userService.LogOutAdmin();
            var viewModel = new SessionViewModel() { UserSession = this.userService.GetUser(HttpContext.Current), Success = true };
            return viewModel;
        }

        /// <summary>
        /// Changes the website.
        /// </summary>
        /// <param name="websiteName">Name of the website.</param>
        /// <returns>The user session view model.</returns>
        [Route("api/user/website/{websiteName}")]
        [HttpGet]
        public SessionViewModel ChangeWebsite(string websiteName)
        {
            var website = this.websiteRepository.GetWebsiteByName(websiteName);
            this.userService.SetSelectedCmsWebsite(website);

            var viewModel = new SessionViewModel() { UserSession = this.userService.GetUser(HttpContext.Current), Success = true };
            return viewModel;
        }

        /// <summary>
        /// Trades the login.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="websitePassword">The website password.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="saveDetails">The save details.</param>
        /// <returns>the user session view model</returns>
        [Route("api/user/login/trade")]
        [HttpGet]
        public async Task<SessionViewModel> TradeLogin([FromUri] string username = "", [FromUri] string password = "",
            [FromUri] string websitePassword = "", [FromUri] string emailAddress = "", [FromUri] bool saveDetails = true)
        {
            var loginReturn = this.tradeService.Login(username, password, websitePassword, emailAddress, saveDetails);

            if (loginReturn.LoginSuccessful)
            {
                this.userService.LoginTrade(loginReturn.TradeSession);
            }

            var viewModel = new SessionViewModel() { UserSession = this.userService.GetUser(HttpContext.Current), Warnings = loginReturn.Warnings, Success = loginReturn.LoginSuccessful };

            return viewModel;
        }
    }
}