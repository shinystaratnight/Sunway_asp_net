namespace Web.Template.Application.Booking.Models
{
    using Web.Template.Application.Booking.Enums;
    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// A booking documentation model, containing information required for the sending and viewing of documentation via connect
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Booking.Models.IBookingDocumentationModel" />
    public class BookingDocumentationModel : IBookingDocumentationModel
    {
        /// <summary>
        /// Gets or sets the booking reference.
        /// </summary>
        /// <value>
        /// The booking reference.
        /// </value>
        public string BookingReference { get; set; }

        /// <summary>
        /// Gets or sets the documentation action.
        /// </summary>
        /// <value>
        /// The documentation action.
        /// </value>
        public DocumentationAction DocumentationAction { get; set; }

        /// <summary>
        /// Gets or sets the documentation identifier.
        /// </summary>
        /// <value>
        /// The documentation identifier.
        /// </value>
        public int DocumentationId { get; set; }

        /// <summary>
        /// Gets or sets the override email.
        /// </summary>
        /// <value>
        /// The override email.
        /// </value>
        public string OverrideEmail { get; set; }

        /// <summary>
        /// Gets or sets the quote external reference.
        /// </summary>
        /// <value>The quote external reference.</value>
        public string QuoteExternalReference { get; set; }
    }
}