namespace Web.Template.Models.Application
{
    using System.Collections.Generic;

    /// <summary>
    /// Model that is handed to the Basket controller to access a specific component in a basket
    /// </summary>
    public class ComponentModel
    {
        /// <summary>
        ///     Gets or sets the basket token.
        /// </summary>
        /// <value>
        ///     The basket token.
        /// </value>
        public string BasketToken { get; set; }

        /// <summary>
        ///     Gets or sets the component token.
        /// </summary>
        /// <value>
        ///     The component token.
        /// </value>
        public int ComponentToken { get; set; }

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