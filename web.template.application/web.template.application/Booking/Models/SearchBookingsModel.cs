namespace Web.Template.Application.Booking.Models
{
    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// A model containing information that will be used to search for bookings
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Booking.Models.ISearchBookingsModel" />
    public class SearchBookingsModel : ISearchBookingsModel
    {
        /// <summary>
        /// Gets or sets the trade contact identifier.
        /// </summary>
        /// <value>
        /// The trade contact identifier.
        /// </value>
        public int TradeContactId { get; set; }

        /// <summary>
        /// Gets or sets the trade identifier.
        /// </summary>
        /// <value>
        /// The trade identifier.
        /// </value>
        public int TradeId { get; set; }

        /// <summary>
        /// Gets or sets the trade reference.
        /// </summary>
        /// <value>
        /// The trade reference.
        /// </value>
        public string TradeReference { get; set; }
    }
}