namespace Web.Template.Application.Booking.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Booking.Enums;
    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// Class returned from cancellation requests
    /// </summary>
    public class ComponentCancellationReturn : IComponentCancellationReturn
    {
        /// <summary>
        /// Gets or sets the cancellation components.
        /// </summary>
        /// <value>
        /// The cancellation components.
        /// </value>
        public List<CancellationComponent> CancellationComponents { get; set; }

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
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        public List<string> Warnings { get; set; }
    }
}