namespace Web.Template.Application.Booking.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Booking.Enums;
    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// Documentation return returned when we send or view booking documentation
    /// </summary>
    public class BookingDocumentationReturn : IBookingDocumentationReturn
    {
        /// <summary>
        /// Gets or sets the documentation action.
        /// </summary>
        /// <value>
        /// The documentation action.
        /// </value>
        public DocumentationAction DocumentationAction { get; set; }

        /// <summary>
        /// Gets or sets the document paths.
        /// </summary>
        /// <value>
        /// The document paths.
        /// </value>
        public List<string> DocumentPaths { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BookingDocumentationReturn"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        public List<string> Warnings { get; set; }
    }
}