namespace Web.Template.Application.Search.SearchModels
{
    using System.Collections.Generic;

    /// <summary>
    ///     Class representing a search room
    /// </summary>
    public class Room
    {
        /// <summary>
        ///     Gets or sets the adults.
        /// </summary>
        /// <value>
        ///     The number of adults in the room.
        /// </value>
        public int Adults { get; set; }

        /// <summary>
        ///     Gets or sets the child ages.
        /// </summary>
        /// <value>
        ///     The child ages.
        /// </value>
        public List<int> ChildAges { get; set; }

        /// <summary>
        ///     Gets or sets the children.
        /// </summary>
        /// <value>
        ///     The children.
        /// </value>
        public int Children { get; set; }

        /// <summary>
        ///     Gets or sets the infants.
        /// </summary>
        /// <value>
        ///     The infants.
        /// </value>
        public int Infants { get; set; }
    }
}