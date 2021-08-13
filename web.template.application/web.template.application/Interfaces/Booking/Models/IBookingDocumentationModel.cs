namespace Web.Template.Application.Interfaces.Booking.Models
{
    using Web.Template.Application.Booking.Enums;

    /// <summary>
    /// A model passed into request to view and send booking documentation contains the information required to generate a document
    /// </summary>
    public interface IBookingDocumentationModel
    {
        /// <summary>
        /// Gets or sets the booking reference.
        /// </summary>
        /// <value>
        /// The booking reference.
        /// </value>
        string BookingReference { get; set; }

        /// <summary>
        /// Gets or sets the documentation action.
        /// </summary>
        /// <value>
        /// The documentation action.
        /// </value>
        DocumentationAction DocumentationAction { get; set; }

        /// <summary>
        /// Gets or sets the documentation identifier.
        /// </summary>
        /// <value>
        /// The documentation identifier.
        /// </value>
        int DocumentationId { get; set; }

        /// <summary>
        /// Gets or sets the override email.
        /// </summary>
        /// <value>
        /// The override email.
        /// </value>
        string OverrideEmail { get; set; }

        /// <summary>
        /// Gets or sets the quote external reference.
        /// </summary>
        /// <value>The quote external reference.</value>
        string QuoteExternalReference { get; set; }
    }
}