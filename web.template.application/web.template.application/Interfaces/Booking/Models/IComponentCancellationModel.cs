namespace Web.Template.Application.Interfaces.Booking.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Booking.Models;

    /// <summary>
    /// a model passed into a component cancel that contains all the information needed to do the cancellation
    /// </summary>
    public interface IComponentCancellationModel
    {
        /// <summary>
        /// Gets or sets the booking reference.
        /// </summary>
        /// <value>
        /// The booking reference.
        /// </value>
        string BookingReference { get; set; }

        /// <summary>
        /// Gets or sets the cancellation components.
        /// </summary>
        /// <value>
        /// The cancellation components.
        /// </value>
        List<CancellationComponent> CancellationComponents { get; set; }
    }
}