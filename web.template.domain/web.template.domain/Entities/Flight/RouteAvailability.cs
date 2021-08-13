namespace Web.Template.Domain.Entities.Flight
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Route availability lookup
    /// </summary>
    /// <seealso cref="Web.Template.Domain.Interfaces.Entity.ILookup" />
    [Table("vRouteAvailability")]
    public partial class RouteAvailability : ILookup
    {
        /// <summary>
        /// Gets or sets the airport group.
        /// </summary>
        /// <value>
        /// The airport group.
        /// </value>
        [Column("AirportGroupID")]
        public AirportGroup AirportGroup { get; set; }

        /// <summary>
        /// Gets or sets the airport group identifier.
        /// </summary>
        /// <value>
        /// The airport group identifier.
        /// </value>
        [Column(Order = 0)]
        public int? AirportGroupID { get; set; }

        /// <summary>
        /// Gets or sets the arrival airport.
        /// </summary>
        /// <value>
        /// The arrival airport.
        /// </value>
        [Column("ArrivalAirportID")]
        public Airport ArrivalAirport { get; set; }

        /// <summary>
        /// Gets or sets the arrival airport identifier.
        /// </summary>
        /// <value>
        /// The arrival airport identifier.
        /// </value>
        [Key, Column("ArrivalAirportID", Order = 2)]
        public int ArrivalAirportID { get; set; }

        /// <summary>
        /// Gets or sets the departure airport.
        /// </summary>
        /// <value>
        /// The departure airport.
        /// </value>
        [Column("DepartureAirportID")]
        public Airport DepartureAirport { get; set; }

        /// <summary>
        /// Gets or sets the departure airport identifier.
        /// </summary>
        /// <value>
        /// The departure airport identifier.
        /// </value>
        [Key, Column("DepartureAirportID", Order = 1)]
        public int DepartureAirportID { get; set; }

        /// <summary>
        /// Gets or sets the iata code.
        /// </summary>
        /// <value>
        /// The iata code.
        /// </value>
        public string IATACode { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [NotMapped]
        public int Id { get; set; }
    }
}