namespace Web.Template.Domain.Entities.Flight
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Airport Entity
    /// </summary>
    /// <seealso cref="Web.Template.Domain.Interfaces.Entity.ILookup" />
    [Table("Airport")]
    public class Airport : ILookup
    {
        /// <summary>
        /// Gets or sets a value indicating whether [brand valid airport].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [brand valid airport]; otherwise, <c>false</c>.
        /// </value>
        public bool BrandValidAirport { get; set; } = true;

        /// <summary>
        /// Gets or sets the check in minutes.
        /// </summary>
        /// <value>
        /// The check in minutes.
        /// </value>
        public int? CheckInMinutes { get; set; }

        /// <summary>
        /// Gets or sets the geography level1 identifier.
        /// </summary>
        /// <value>
        /// The geography level1 identifier.
        /// </value>
        public int GeographyLevel1ID { get; set; }

        /// <summary>
        /// Gets or sets the iata code.
        /// </summary>
        /// <value>
        /// The iata code.
        /// </value>
        [Required]
        [StringLength(10)]
        public string IATACode { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("AirportID")]
        [Required]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>
        /// The latitude.
        /// </value>
        [StringLength(50)]
        public string Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        /// <value>
        /// The longitude.
        /// </value>
        [StringLength(50)]
        public string Longitude { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Column("Airport")]
        [Required]
        [StringLength(80)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the offset days.
        /// </summary>
        /// <value>
        /// The offset days.
        /// </value>
        public int? OffsetDays { get; set; }

        /// <summary>
        /// Gets or sets the preferred airport.
        /// </summary>
        /// <value>
        /// The preferred airport.
        /// </value>
        public bool? PreferredAirport { get; set; }

        /// <summary>
        /// Gets or sets the resorts.
        /// </summary>
        /// <value>
        /// The resorts.
        /// </value>
        public ICollection<Resort> Resorts { get; set; }

        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        /// <value>
        /// The short name.
        /// </value>
        [StringLength(15)]
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the terminals.
        /// </summary>
        /// <value>
        /// The terminals.
        /// </value>
        public ICollection<AirportTerminal> Terminals { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [Required]
        [StringLength(30)]
        public string Type { get; set; }
    }
}