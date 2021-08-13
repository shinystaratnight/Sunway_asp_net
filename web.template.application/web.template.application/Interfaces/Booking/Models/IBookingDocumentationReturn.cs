namespace Web.Template.Application.Interfaces.Booking.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Booking.Enums;
    using Web.Template.Application.Booking.Models;

    /// <summary>
    /// A model returned from the documentation service, will contain if sending the document went okay, and a list of any documents generated if they have been requested.
    /// </summary>
    public interface IBookingDocumentationReturn
    {
        /// <summary>
        /// Gets or sets the documentation action.
        /// </summary>
        /// <value>
        /// The documentation action.
        /// </value>
        DocumentationAction DocumentationAction { get; set; }

        /// <summary>
        /// Gets or sets the document paths.
        /// </summary>
        /// <value>
        /// The document paths.
        /// </value>
        List<string> DocumentPaths { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BookingDocumentationReturn"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        bool Success { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        List<string> Warnings { get; set; }
    }
}