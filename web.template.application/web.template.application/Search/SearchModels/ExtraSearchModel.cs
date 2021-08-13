namespace Web.Template.Application.Search.SearchModels
{
    using System;
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Class ExtraSearchModel.
    /// </summary>
    public class ExtraSearchModel : IExtraSearchModel
    {
        /// <summary>
        /// Gets or sets the adults.
        /// </summary>
        /// <value>The adults.</value>
        public int Adults { get; set; }

        /// <summary>
        /// Gets or sets the adult ages.
        /// </summary>
        /// <value>The adult ages.</value>
        public List<int> AdultAges { get; set; }

        /// <summary>
        /// Gets or sets the arrival airport identifier.
        /// </summary>
        /// <value>The arrival airport identifier.</value>
        public int ArrivalAirportId { get; set; }

        /// <summary>
        /// Gets or sets the booking price.
        /// </summary>
        /// <value>The booking price.</value>
        public decimal BookingPrice { get; set; }

        /// <summary>
        /// Gets or sets the type of the booking.
        /// </summary>
        /// <value>The type of the booking.</value>
        public string BookingType { get; set; }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>The children.</value>
        public int Children { get; set; }

        /// <summary>
        /// Gets or sets the child ages.
        /// </summary>
        /// <value>The child ages.</value>
        public List<int> ChildAges { get; set; }

        /// <summary>
        /// Gets or sets the departure airport identifier.
        /// </summary>
        /// <value>The departure airport identifier.</value>
        public int DepartureAirportId { get; set; }

        /// <summary>
        /// Gets or sets the departure date.
        /// </summary>
        /// <value>The departure date.</value>
        public DateTime DepartureDate { get; set; }

        /// <summary>
        /// Gets or sets the departure time.
        /// </summary>
        /// <value>The departure time.</value>
        public string DepartureTime { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the geography level1 identifier.
        /// </summary>
        /// <value>The geography level1 identifier.</value>
        public int GeographyLevel1Id { get; set; }

        /// <summary>
        /// Gets or sets the geography level2 identifier.
        /// </summary>
        /// <value>The geography level2 identifier.</value>
        public int GeographyLevel2Id { get; set; }

        /// <summary>
        /// Gets or sets the geography level3 identifier.
        /// </summary>
        /// <value>The geography level3 identifier.</value>
        public int GeographyLevel3Id { get; set; }

        /// <summary>
        /// Gets or sets the extra group identifier.
        /// </summary>
        /// <value>The extra group identifier.</value>
        public int ExtraGroupId { get; set; }

        /// <summary>
        /// Gets or sets the extra identifier.
        /// </summary>
        /// <value>The extra identifier.</value>
        public int ExtraId { get; set; }

        /// <summary>
        /// Gets or sets the extra types.
        /// </summary>
        /// <value>The extra types.</value>
        public List<int> ExtraTypes { get; set; }

        /// <summary>
        /// Gets or sets the infants.
        /// </summary>
        /// <value>The infants.</value>
        public int Infants { get; set; }

        /// <summary>
        /// Gets or sets the property reference identifier.
        /// </summary>
        /// <value>The property reference identifier.</value>
        public int PropertyReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the return date.
        /// </summary>
        /// <value>The return date.</value>
        public DateTime ReturnDate { get; set; }

        /// <summary>
        /// Gets or sets the return time.
        /// </summary>
        /// <value>The return time.</value>
        public string ReturnTime { get; set; }
    }
}
