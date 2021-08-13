namespace Web.Template.Application.Booking.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Booking.Enums;
    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// Class returned from cancellation requests
    /// </summary>
    public class CancellationReturn : ICancellationReturn
    {
        /// <summary>
        /// Gets or sets the cost.
        /// </summary>
        /// <value>
        /// The cost.
        /// </value>
        public decimal Cost { get; set; }

        /// <summary>
        /// Gets or sets the stage.
        /// </summary>
        /// <value>
        /// The stage.
        /// </value>
        public CancellationStage Stage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CancellationReturn"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        public List<string> Warnings { get; set; }
    }
}