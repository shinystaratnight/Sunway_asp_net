namespace Web.Template.Application.User.Models
{
    using Web.Template.Application.Interfaces.User;
    using Web.Template.Domain.Entities.Booking;

    /// <summary>
    /// Trade session class.
    /// </summary>
    /// <seealso cref="ITradeSession" />
    public class TradeSession : ITradeSession
    {
        /// <summary>
        /// Gets or sets the abtaatol.
        /// </summary>
        /// <value>
        /// The abtaatol.
        /// </value>
        public string ABTAATOL { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TradeSession"/> is commissionable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if commissionable; otherwise, <c>false</c>.
        /// </value>
        public bool Commissionable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [credit card agent].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [credit card agent]; otherwise, <c>false</c>.
        /// </value>
        public bool CreditCardAgent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [logged in].
        /// </summary>
        /// <value><c>true</c> if [logged in]; otherwise, <c>false</c>.</value>
        public bool LoggedIn { get; set; }

        /// <summary>
        /// Gets or sets the trade.
        /// </summary>
        /// <value>
        /// The trade.
        /// </value>
        public Trade Trade { get; set; }

        /// <summary>
        /// Gets or sets the trade contact.
        /// </summary>
        /// <value>
        /// The trade contact.
        /// </value>
        public TradeContact TradeContact { get; set; }

        /// <summary>
        /// Gets or sets the trade contact group.
        /// </summary>
        /// <value>
        /// The trade contact group.
        /// </value>
        public TradeContactGroup TradeContactGroup { get; set; }

        /// <summary>
        /// Gets or sets the trade contact identifier.
        /// </summary>
        /// <value>
        /// The trade contact identifier.
        /// </value>
        public int TradeContactId { get; set; }

        /// <summary>
        /// Gets or sets the trade group.
        /// </summary>
        /// <value>
        /// The trade group.
        /// </value>
        public TradeGroup TradeGroup { get; set; }

        /// <summary>
        /// Gets or sets the trade identifier.
        /// </summary>
        /// <value>
        /// The trade identifier.
        /// </value>
        public int TradeId { get; set; }
    }
}