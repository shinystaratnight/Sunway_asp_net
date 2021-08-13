namespace Web.Template.Application.Basket.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Model used for adding components
    /// </summary>
    public class BasketComponentModel
    {
        /// <summary>
        /// Gets or sets the adjustment search token.
        /// </summary>
        /// <value>The adjustment search token.</value>
        public string AdjustmentSearchToken { get; set; }

        /// <summary>
        /// Gets or sets the basket component.
        /// </summary>
        /// <value>
        /// The basket component.
        /// </value>
        public IBasketComponent BasketComponent { get; set; }

        /// <summary>
        /// Gets or sets the basket token.
        /// </summary>
        /// <value>The basket token.</value>
        public string BasketToken { get; set; }

        /// <summary>
        ///     Gets or sets the component token.
        /// </summary>
        /// <value>
        ///     The component token.
        /// </value>
        public int ComponentToken { get; set; }

        /// <summary>
        /// Gets or sets the meta data.
        /// </summary>
        /// <value>
        /// The meta data.
        /// </value>
        public Dictionary<string, string> MetaData { get; set; }

        /// <summary>
        /// Gets or sets the search token.
        /// </summary>
        /// <value>The search token.</value>
        public string SearchToken { get; set; }

        /// <summary>
        /// Gets or sets the sub component tokens.
        /// </summary>
        /// <value>
        /// The sub component tokens.
        /// </value>
        public List<int> SubComponentTokens { get; set; }

        /// <summary>
        ///     Gets or sets the user token.
        /// </summary>
        /// <value>
        ///     The user token.
        /// </value>
        public string UserToken { get; set; }
    }
}