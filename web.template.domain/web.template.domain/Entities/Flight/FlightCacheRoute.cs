namespace Web.Template.Domain.Entities.Flight
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Class FlightCacheRoute.
    /// </summary>
    public class FlightCacheRoute : ILookup
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [XmlElement("ID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the departure airport identifier.
        /// </summary>
        /// <value>The departure airport identifier.</value>
        [XmlElement("DepartureAirportID")]
        public int DepartureAirportId { get; set; }

        /// <summary>
        /// Gets or sets the arrival airport identifier.
        /// </summary>
        /// <value>The arrival airport identifier.</value>
        [XmlElement("ArrivalAirportID")]
        public int ArrivalAirportId { get; set; }

        /// <summary>
        /// Gets or sets the departure dates.
        /// </summary>
        /// <value>The departure dates.</value>
        [XmlArrayItem("DepartureDate")]
        public List<DateTime> DepartureDates { get; set; }

        /// <summary>
        /// Gets or sets the departure dates and durations.
        /// </summary>
        /// <value>The departure dates and durations.</value>
        [XmlArrayItem("DepartureDateAndDuration")]
        public List<DepartureDateAndDuration> DepartureDatesAndDurations { get; set; }

        /// <summary>
        /// Gets or sets the destinations.
        /// </summary>
        /// <value>The destinations.</value>
        public List<Destination> Destinations { get; set; }

        /// <summary>
        /// Class Destination.
        /// </summary>
        public class Destination
        {
            /// <summary>
            /// Gets or sets the geography level2 identifier.
            /// </summary>
            /// <value>The geography level2 identifier.</value>
            [XmlElement("GeographyLevel2ID")]
            public int GeographyLevel2Id { get; set; }

            /// <summary>
            /// Gets or sets the geography level3 identifier.
            /// </summary>
            /// <value>The geography level3 identifier.</value>
            [XmlElement("GeographyLevel3ID")]
            public int GeographyLevel3Id { get; set; }
        }

        public class DepartureDateAndDuration
        {
            [XmlElement("DepartureDate")]
            public DateTime DepartureDate { get; set; }

            [XmlElement("Duration")]
            public int Duration { get; set; }
        }
    }
}