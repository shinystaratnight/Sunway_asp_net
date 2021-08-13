namespace Web.Template.Application.Trade.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.User;

    /// <summary>
    /// Class returned from the trade login
    /// </summary>
    /// <seealso cref="Web.Template.Application.Trade.Models.ITradeLoginReturn" />
    public class TradeLoginReturn : ITradeLoginReturn
    {
        /// <summary>
        /// Gets or sets a value indicating whether [login successful].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [login successful]; otherwise, <c>false</c>.
        /// </value>
        public bool LoginSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the trade session.
        /// </summary>
        /// <value>
        /// The trade session.
        /// </value>
        public ITradeSession TradeSession { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        public List<string> Warnings { get; set; }
    }
}