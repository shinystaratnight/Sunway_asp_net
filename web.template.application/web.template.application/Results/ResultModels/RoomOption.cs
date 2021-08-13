namespace Web.Template.Application.Results.ResultModels
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    ///     Class representing a room result from a hotel search.
    /// </summary>
    public class RoomOption : ISubResult
    {
        /// <summary>
        ///     Gets or sets  The booking token
        /// </summary>
        /// <value>
        ///     The booking token.
        /// </value>
        public string BookingToken { get; set; }

        /// <summary>
        /// Gets or sets the component token.
        /// </summary>
        /// <value>
        /// The component token.
        /// </value>
        public int ComponentToken { get; set; }

        /// <summary>
        /// Gets or sets the invalid flight result ids.
        /// </summary>
        /// <value>The invalid flight result ids.</value>
        public List<int> InvalidFlightResultIds { get; set; }

        /// <summary>
        /// Gets or sets the meal basis identifier.
        /// </summary>
        /// <value>The meal basis identifier.</value>
        public int MealBasisId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [on request].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [on request]; otherwise, <c>false</c>.
        /// </value>
        public bool OnRequest { get; set; }

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

        // public List<SearchResponse.Facility> Facilities { get; set; }
        ///// </value>
        ///// The facilities.
        ///// <value>
        ///// </summary>
        ///// Gets or sets the facilities.

        ///// <summary>
    }
}