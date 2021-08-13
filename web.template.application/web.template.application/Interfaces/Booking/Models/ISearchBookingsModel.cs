namespace Web.Template.Application.Interfaces.Booking.Models
{
    /// <summary>
    /// model passed in for searching bookings
    /// </summary>
    public interface ISearchBookingsModel
    {
        /// <summary>
        /// Gets or sets the trade contact identifier.
        /// </summary>
        /// <value>
        /// The trade contact identifier.
        /// </value>
        int TradeContactId { get; set; }
        
        /// <summary>
        /// Gets or sets the trade identifier.
        /// </summary>
        /// <value>
        /// The trade identifier.
        /// </value>
        int TradeId { get; set; }

        /// <summary>
        /// Gets or sets the trade reference.
        /// </summary>
        /// <value>
        /// The trade reference.
        /// </value>
        string TradeReference { get; set; }
    }
}