namespace Web.Template.Application.Interfaces.Services
{
    using System.Web;

    using Web.Template.Application.Interfaces.User;
    using Web.Template.Domain.Entities.Site;

    /// <summary>
    /// Defines a service that manages operations concerning users.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A user session</returns>
        IUserSession GetUser(HttpContext context);

        /// <summary>
        /// Logins the trade.
        /// </summary>
        /// <param name="tradeSession">The trade session.</param>
        void LoginTrade(ITradeSession tradeSession);

        /// <summary>
        /// Logs the out.
        /// </summary>
        void LogOut();

        /// <summary>
        /// Logs the out admin.
        /// </summary>
        void LogOutAdmin();

        /// <summary>
        /// News the user.
        /// </summary>
        /// <param name="website">The website.</param>
        void NewUser(CmsWebsite website = null);

        /// <summary>
        /// Sets the brand on user.
        /// </summary>
        /// <param name="brandId">The brand identifier.</param>
        void SetBrandOnUser(int brandId);

        /// <summary>
        /// Sets the selected CMS website.
        /// </summary>
        /// <param name="website">The website.</param>
        void SetSelectedCmsWebsite(CmsWebsite website);

        /// <summary>
        /// Sets the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="context">The context.</param>
        void SetUser(IUserSession user, HttpContext context);

        /// <summary>
        /// Sets the user from cookie.
        /// </summary>
        /// <param name="website">The website.</param>
        void SetUserFromCookie(CmsWebsite website = null);
    }
}