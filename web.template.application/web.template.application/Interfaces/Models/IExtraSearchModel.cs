namespace Web.Template.Application.Interfaces.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface IExtraSearchModel
    /// </summary>
    public interface IExtraSearchModel
    {
        /// <summary>
        /// Gets or sets the adults.
        /// </summary>
        /// <value>The adults.</value>
        int Adults { get; set; }

        /// <summary>
        /// Gets or sets the adult ages.
        /// </summary>
        /// <value>The adult ages.</value>
        List<int> AdultAges { get; set; }

        /// <summary>
        /// Gets or sets the arrival airport identifier.
        /// </summary>
        /// <value>The arrival airport identifier.</value>
        int ArrivalAirportId { get; set; }

        /// <summary>
        /// Gets or sets the booking price.
        /// </summary>
        /// <value>The booking price.</value>
        decimal BookingPrice { get; set; }

        /// <summary>
        /// Gets or sets the type of the booking.
        /// </summary>
        /// <value>The type of the booking.</value>
        string BookingType { get; set; }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>The children.</value>
        int Children { get; set; }

        /// <summary>
        /// Gets or sets the child ages.
        /// </summary>
        /// <value>The child ages.</value>
        List<int> ChildAges { get; set; }

        /// <summary>
        /// Gets or sets the departure airport identifier.
        /// </summary>
        /// <value>The departure airport identifier.</value>
        int DepartureAirportId { get; set; }

        /// <summary>
        /// Gets or sets the departure date.
        /// </summary>
        /// <value>The departure date.</value>
        DateTime DepartureDate { get; set; }

        /// <summary>
        /// Gets or sets the departure time.
        /// </summary>
        /// <value>The departure time.</value>
        string DepartureTime { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        int Duration { get; set; }

        /// <summary>
        /// Gets or sets the geography level1 identifier.
        /// </summary>
        /// <value>The geography level1 identifier.</value>
        int GeographyLevel1Id { get; set; }

        /// <summary>
        /// Gets or sets the geography level2 identifier.
        /// </summary>
        /// <value>The geography level2 identifier.</value>
        int GeographyLevel2Id { get; set; }

        /// <summary>
        /// Gets or sets the geography level3 identifier.
        /// </summary>
        /// <value>The geography level3 identifier.</value>
        int GeographyLevel3Id { get; set; }

        /// <summary>
        /// Gets or sets the extra group identifier.
        /// </summary>
        /// <value>The extra group identifier.</value>
        int ExtraGroupId { get; set; }

        /// <summary>
        /// Gets or sets the extra identifier.
        /// </summary>
        /// <value>The extra identifier.</value>
        int ExtraId { get; set; }

        /// <summary>
        /// Gets or sets the extra types.
        /// </summary>
        /// <value>The extra types.</value>
        List<int> ExtraTypes { get; set; }

        /// <summary>
        /// Gets or sets the infants.
        /// </summary>
        /// <value>The infants.</value>
        int Infants { get; set; }

        /// <summary>
        /// Gets or sets the property reference identifier.
        /// </summary>
        /// <value>The property reference identifier.</value>
        int PropertyReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the return date.
        /// </summary>
        /// <value>The return date.</value>
        DateTime ReturnDate { get; set; }

        /// <summary>
        /// Gets or sets the return time.
        /// </summary>
        /// <value>The return time.</value>
        string ReturnTime { get; set; }
    }
}
