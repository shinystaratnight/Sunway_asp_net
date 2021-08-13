namespace Web.Template.Domain.Entities.Geography
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Entity Representing a port
    /// </summary>
    [Table("Port")]
    public class Port : ILookup
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [StringLength(5)]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("PortID")]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Column("PortName")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the resort.
        /// </summary>
        /// <value>
        /// The resort.
        /// </value>
        public Resort Resort { get; set; }

        /// <summary>
        /// Gets or sets the resort identifier.
        /// </summary>
        /// <value>
        /// The resort identifier.
        /// </value>
        [Column("GeographyLevel3ID")]
        public int ResortId { get; set; }
    }
}