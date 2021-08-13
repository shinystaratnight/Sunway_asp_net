namespace Web.Template.Application.Booking.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// a model passed to requests for cancelling components
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Booking.Models.IComponentCancellationModel" />
    public class ComponentCancellationModel : IComponentCancellationModel
    {
        /// <summary>
        /// Gets or sets the booking reference.
        /// </summary>
        /// <value>
        /// The booking reference.
        /// </value>
        public string BookingReference { get; set; }

        /// <summary>
        /// Gets or sets the cancellation components.
        /// </summary>
        /// <value>
        /// The cancellation components.
        /// </value>
        public List<CancellationComponent> CancellationComponents { get; set; }
    }
}