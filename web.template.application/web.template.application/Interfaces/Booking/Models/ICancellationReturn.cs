namespace Web.Template.Application.Interfaces.Booking.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Booking.Enums;

    /// <summary>
    /// Defines a class returned from a cancellation request containing information about the attempted cancellation.
    /// </summary>
    public interface ICancellationReturn
    {
        /// <summary>
        /// Gets or sets the cost.
        /// </summary>
        /// <value>
        /// The cost.
        /// </value>
        decimal Cost { get; set; }

        /// <summary>
        /// Gets or sets the stage.
        /// </summary>
        /// <value>
        /// The stage.
        /// </value>
        CancellationStage Stage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CancellationReturn"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        bool Success { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        string Token { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        List<string> Warnings { get; set; }
    }
}