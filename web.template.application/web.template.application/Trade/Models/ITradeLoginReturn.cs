namespace Web.Template.Application.Trade.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.User;

    /// <summary>
    /// Interface defining what we need out of a trade login.
    /// </summary>
    public interface ITradeLoginReturn
    {
        /// <summary>
        /// Gets or sets a value indicating whether [login successful].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [login successful]; otherwise, <c>false</c>.
        /// </value>
        bool LoginSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the trade session.
        /// </summary>
        /// <value>
        /// The trade session.
        /// </value>
        ITradeSession TradeSession { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        List<string> Warnings { get; set; }
    }
}