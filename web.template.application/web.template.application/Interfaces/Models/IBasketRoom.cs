namespace Web.Template.Application.Interfaces.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// An interface defining a room configuration.
    /// </summary>
    public interface IBasketRoom
    {
        /// <summary>
        /// Gets or sets the guests.
        /// </summary>
        /// <value>
        /// The guests.
        /// </value>
        List<IGuest> Guests { get; set; }

        /// <summary>
        /// Gets or sets the room number.
        /// </summary>
        /// <value>
        /// The room number.
        /// </value>
        int RoomNumber { get; set; }
    }
}