namespace Web.Template.Application.Trade.Models
{
    using Web.Template.Application.Trade.Enum;

    /// <summary>
    /// Trade login model defines all the details we need to perform a trade login.
    /// </summary>
    public interface ITradeLoginModel
    {
        /// <summary>
        /// Gets or sets the agent reference.
        /// </summary>
        /// <value>
        /// The agent reference.
        /// </value>
        string AgentReference { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [save details].
        /// </summary>
        /// <value><c>true</c> if [save details]; otherwise, <c>false</c>.</value>
        bool SaveDetails { get; set; }

        /// <summary>
        /// Gets or sets the trade login level.
        /// </summary>
        /// <value>
        /// The trade login level.
        /// </value>
        TradeLoginLevel TradeLoginLevel { get; set; }

        /// <summary>
        /// Gets or sets the trade login method.
        /// </summary>
        /// <value>
        /// The trade login method.
        /// </value>
        TradeLoginMethod TradeLoginMethod { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        string UserName { get; set; }

        /// <summary>
        /// Gets or sets the website password.
        /// </summary>
        /// <value>
        /// The website password.
        /// </value>
        string WebsitePassword { get; set; }
    }
}