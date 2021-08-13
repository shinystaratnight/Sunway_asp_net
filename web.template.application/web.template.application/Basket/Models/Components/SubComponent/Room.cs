namespace Web.Template.Application.Basket.Models.Components.SubComponent
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Class representing a single room.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Models.ISubComponent" />
    public class Room : ISubComponent
    {
        /// <summary>
        /// Gets or sets the adults.
        /// </summary>
        /// <value>
        /// The adults.
        /// </value>
        public int Adults { get; set; }

        /// <summary>
        ///     Gets or sets  The booking token
        /// </summary>
        /// <value>
        ///     The booking token.
        /// </value>
        public string BookingToken { get; set; }

        /// <summary>
        /// Gets or sets the child ages.
        /// </summary>
        /// <value>
        /// The child ages.
        /// </value>
        public List<int> ChildAges { get; set; }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public int Children { get; set; }

        /// <summary>
        /// Gets or sets the component token.
        /// </summary>
        /// <value>
        /// The component token.
        /// </value>
        public int ComponentToken { get; set; }

        /// <summary>
        /// Gets or sets the guest i ds.
        /// </summary>
        /// <value>
        /// The guest i ds.
        /// </value>
        public List<int> GuestIDs { get; set; }

        /// <summary>
        /// Gets or sets the infants.
        /// </summary>
        /// <value>
        /// The infants.
        /// </value>
        public int Infants { get; set; }

        /// <summary>
        /// Gets or sets the meal basis identifier.
        /// </summary>
        /// <value>
        /// The meal basis identifier.
        /// </value>
        public int MealBasisId { get; set; }

        /// <summary>
        /// Gets or sets the room special request to be set to the third party
        /// </summary>
        /// <value>
        /// The room special request.
        /// </value>
        public string Request { get; set; }

        /// <summary>
        ///     Gets or sets  The room type
        /// </summary>
        /// <value>
        ///     The room type.
        /// </value>
        public string RoomType { get; set; }

        /// <summary>
        ///     Gets or sets the room view.
        /// </summary>
        /// <value>
        ///     The room view.
        /// </value>
        public string RoomView { get; set; }

        /// <summary>
        ///     Gets or sets the sequence.
        /// </summary>
        /// <value>
        ///     The sequence.
        /// </value>
        public int Sequence { get; set; }

        /// <summary>
        ///     Gets or sets  The source
        /// </summary>
        /// <value>
        ///     The source.
        /// </value>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the supplier identifier.
        /// </summary>
        /// <value>
        /// The supplier identifier.
        /// </value>
        public int SupplierId { get; set; }

        /// <summary>
        ///     Gets or sets The total price, this should be in the users currency
        /// </summary>
        /// <value>
        ///     The total price.
        /// </value>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Setups the subcomponent search details.
        /// </summary>
        /// <param name="searchRoom">The search room.</param>
        public void SetupSubcomponentSearchDetails(Search.SearchModels.Room searchRoom)
        {
            this.Adults = searchRoom.Adults;
            this.Children = searchRoom.Children;
            this.Infants = searchRoom.Infants;
            this.ChildAges = searchRoom.ChildAges;
        }
    }
}