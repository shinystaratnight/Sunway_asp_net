namespace Web.TradeMMB.Models.Application
{
    using System.Collections.Generic;

    /// <summary>
    /// Class represeting the Redux State for initial setup
    /// </summary>
    public class ReduxState
    {
        /// <summary>
        /// Gets or sets the entities.
        /// </summary>
        /// <value>The entities.</value>
        public Dictionary<string, object> entities { get; set; }
    }
}