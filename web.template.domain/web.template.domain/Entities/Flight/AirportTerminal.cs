namespace Web.Template.Domain.Entities.Flight
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Entity Class for Airport Terminal Table
    /// </summary>
    [Table("AirportTerminal")]
    public partial class AirportTerminal : ILookup
    {
        /// <summary>
        /// Gets or sets the airport.
        /// </summary>
        /// <value>
        /// The airport.
        /// </value>
        [ForeignKey("AirportID")]
        public Airport Airport { get; set; }

        /// <summary>
        /// Gets or sets the airport identifier.
        /// </summary>
        /// <value>
        /// The airport identifier.
        /// </value>
        public int AirportID { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key]
        [Column("AirportTerminalID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Column("AirportTerminal")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}