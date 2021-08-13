namespace Web.Template.Application.Interfaces.User
{
    using Web.Template.Domain.Entities.Booking;

    /// <summary>
    /// interface defining what a trade session object must have.
    /// </summary>
    public interface ITradeSession
    {
        /// <summary>
        /// Gets or sets the abtaatol.
        /// </summary>
        /// <value>
        /// The abtaatol.
        /// </value>
        string ABTAATOL { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ITradeSession"/> is commissionable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if commissionable; otherwise, <c>false</c>.
        /// </value>
        bool Commissionable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [credit card agent].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [credit card agent]; otherwise, <c>false</c>.
        /// </value>
        bool CreditCardAgent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [logged in].
        /// </summary>
        /// <value><c>true</c> if [logged in]; otherwise, <c>false</c>.</value>
        bool LoggedIn { get; set; }

        /// <summary>
        /// Gets or sets the trade.
        /// </summary>
        /// <value>
        /// The trade.
        /// </value>
        Trade Trade { get; set; }

        /// <summary>
        /// Gets or sets the trade contact.
        /// </summary>
        /// <value>
        /// The trade contact.
        /// </value>
        TradeContact TradeContact { get; set; }

        /// <summary>
        /// Gets or sets the trade contact group.
        /// </summary>
        /// <value>
        /// The trade contact group.
        /// </value>
        TradeContactGroup TradeContactGroup { get; set; }

        /// <summary>
        /// Gets or sets the trade contact identifier.
        /// </summary>
        /// <value>
        /// The trade contact identifier.
        /// </value>
        int TradeContactId { get; set; }

        /// <summary>
        /// Gets or sets the trade group.
        /// </summary>
        /// <value>
        /// The trade group.
        /// </value>
        TradeGroup TradeGroup { get; set; }

        /// <summary>
        /// Gets or sets the trade identifier.
        /// </summary>
        /// <value>
        /// The trade identifier.
        /// </value>
        int TradeId { get; set; }
    }
}