namespace Web.Template.Application.Interfaces.Booking.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Booking.Enums;
    using Web.Template.Application.Booking.Models;

    /// <summary>
    /// The model returned from requests to cancel components, contains information such as whether it was successful and how much it will cost.
    /// </summary>
    public interface IComponentCancellationReturn
    {
        /// <summary>
        /// Gets or sets the cancellation components.
        /// </summary>
        /// <value>
        /// The cancellation components.
        /// </value>
        List<CancellationComponent> CancellationComponents { get; set; }

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
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        List<string> Warnings { get; set; }
    }
}