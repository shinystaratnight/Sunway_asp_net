namespace Web.Template.Application.Basket.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Class representing a basket room.
    /// </summary>
    public class BasketRoom
    {
        /// <summary>
        /// Gets or sets the guests.
        /// </summary>
        /// <value>
        /// The guests.
        /// </value>
        public List<GuestDetail> Guests { get; set; }

        /// <summary>
        /// Gets or sets the room number.
        /// </summary>
        /// <value>
        /// The room number.
        /// </value>
        public int RoomNumber { get; set; }
    }
}