namespace Web.Template.Application.Interfaces.Services
{
    using Web.Template.Application.Trade.Models;
    using Web.Template.Domain.Entities.Booking;

    /// <summary>
    /// Service responsible for actions regarding a trade such as logging in/out.
    /// </summary>
    public interface ITradeService
    {
        /// <summary>
        /// Gets the name of the trade cooking.
        /// </summary>
        /// <value>
        /// The name of the trade cooking.
        /// </value>
        string TradeCookieName { get; }

        /// <summary>
        /// Gets the trade by identifier.
        /// </summary>
        /// <param name="tradeId">The trade identifier.</param>
        /// <returns>The Trade.</returns>
        Trade GetTradeById(int tradeId);

        /// <summary>
        /// Logins the specified email address.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="password">The password.</param>
        /// <param name="websitepassword">The website password.</param>
        /// <param name="username">The username.</param>
        /// <param name="saveDetails">if set to <c>true</c> [save details].</param>
        /// <returns>Whether the login was successful.</returns>
        ITradeLoginReturn Login(string emailAddress = "", string password = "", string websitepassword = "", string username = "", bool saveDetails = true);

        /// <summary>
        /// Logins the specified email address.
        /// </summary>
        /// <param name="loginModel">The login model.</param>
        /// <returns>
        /// Whether the login was successful.
        /// </returns>
        ITradeLoginReturn Login(ITradeLoginModel loginModel);

        /// <summary>
        /// Logins from cookie.
        /// </summary>
        /// <returns>A trade login return</returns>
        ITradeLoginReturn LoginFromCookie();
    }
}