namespace Web.Template.Domain.Entities.Flight
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Flight Carrier Class
    /// </summary>
    [Table("FlightCarrier")]
    public class FlightCarrier : ILookup
    {
        /// <summary>
        /// Gets or sets the type of the carrier.
        /// </summary>
        /// <value>
        /// The type of the carrier.
        /// </value>
        [StringLength(20)]
        public string CarrierType { get; set; }

        /// <summary>
        /// Gets or sets the date of birth requirement.
        /// </summary>
        /// <value>
        /// The date of birth requirement.
        /// </value>
        [StringLength(50)]
        public string DateOfBirthRequirement { get; set; }

        /// <summary>
        /// Gets or sets the flight code prefix.
        /// </summary>
        /// <value>
        /// The flight code prefix.
        /// </value>
        [Required]
        [StringLength(5)]
        public string FlightCodePrefix { get; set; }

        /// <summary>
        /// Gets or sets the flight number.
        /// </summary>
        /// <value>
        /// The flight number.
        /// </value>
        [StringLength(3)]
        public string FlightNumber { get; set; }

        /// <summary>
        /// Gets or sets the flight carrier identifier.
        /// </summary>
        /// <value>
        /// The flight carrier identifier.
        /// </value>
        [Column("FlightCarrierID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        /// <value>
        /// The logo.
        /// </value>
        [StringLength(50)]
        public string Logo { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Column("FlightCarrier")]
        [Required]
        [StringLength(40)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the web description.
        /// </summary>
        /// <value>
        /// The web description.
        /// </value>
        [StringLength(40)]
        public string WebDescription { get; set; }
    }
}