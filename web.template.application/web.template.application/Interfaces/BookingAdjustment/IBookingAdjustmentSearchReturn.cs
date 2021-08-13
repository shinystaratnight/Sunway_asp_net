namespace Web.Template.Application.Interfaces.BookingAdjustment
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Prebook;

    /// <summary>
    /// Interface IBookingAdjustmentSearchReturn
    /// </summary>
    public interface IBookingAdjustmentSearchReturn
    {
        /// <summary>
        /// Gets or sets the booking adjustments.
        /// </summary>
        /// <value>The booking adjustments.</value>
        List<IAdjustment> BookingAdjustments { get; set; }

        /// <summary>
        /// Gets or sets the result token.
        /// </summary>
        /// <value>The result token.</value>
        string ResultToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IPrebookReturn"/> is success.
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
