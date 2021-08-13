namespace Web.Template.Domain.Entities.Flight
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Entity class for Flight Class table
    /// </summary>
    [Table("FlightClass")]
    public partial class FlightClass : ILookup
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("FlightClassID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Column("FlightClass")]
        [Required]
        [StringLength(25)]
        public string Name { get; set; }
    }
}