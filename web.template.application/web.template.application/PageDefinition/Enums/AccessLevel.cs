namespace Web.Template.Application.PageDefinition.Enums
{
    /// <summary>
    /// Defines access levels that the user must have to view the page/widget
    /// </summary>
    public enum AccessLevel
    {
        /// <summary>
        /// Public means accessible to all
        /// </summary>
        Public = 0, 

        /// <summary>
        /// The user must be logged in, does not matter if they're a trade or customer.
        /// </summary>
        AnyLoggedIn = 1, 

        /// <summary>
        /// the user must be a logged in customer.
        /// </summary>
        CustomerLoggedIn = 2, 

        /// <summary>
        /// The user must be a logged in trade.
        /// </summary>
        TradeLoggedIn = 3, 

        /// <summary>
        /// The admin logged in
        /// </summary>
        AdminLoggedIn = 4
    }
}