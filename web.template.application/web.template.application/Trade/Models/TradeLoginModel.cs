namespace Web.Template.Application.Trade.Models
{
    using Web.Template.Application.Trade.Enum;

    /// <summary>
    /// Trade login model class.
    /// </summary>
    public class TradeLoginModel : ITradeLoginModel
    {
        /// <summary>
        /// Gets or sets the agent reference.
        /// </summary>
        /// <value>
        /// The agent reference.
        /// </value>
        public string AgentReference { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [save details].
        /// </summary>
        /// <value><c>true</c> if [save details]; otherwise, <c>false</c>.</value>
        public bool SaveDetails { get; set; }

        /// <summary>
        /// Gets or sets the trade login level.
        /// </summary>
        /// <value>
        /// The trade login level.
        /// </value>
        public TradeLoginLevel TradeLoginLevel { get; set; }

        /// <summary>
        /// Gets or sets the trade login method.
        /// </summary>
        /// <value>
        /// The trade login method.
        /// </value>
        public TradeLoginMethod TradeLoginMethod { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the website password.
        /// </summary>
        /// <value>
        /// The website password.
        /// </value>
        public string WebsitePassword { get; set; }
    }
}