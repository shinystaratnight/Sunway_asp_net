namespace Web.Template.Domain.Entities.Flight
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Entity Class for Geography Grouping Table
    /// </summary>
    [Table("GeographyGrouping")]
    public partial class GeographyGrouping : ILookup
    {
        /// <summary>
        /// Gets or sets the brand identifier.
        /// </summary>
        /// <value>
        /// The brand identifier.
        /// </value>
        public int? BrandID { get; set; }

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
        [Column("GeographyGroupingID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        [Required]
        [StringLength(10)]
        public string Level { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Column("GeographyGrouping")]
        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        /// <value>
        /// The sequence.
        /// </value>
        public bool? Sequence { get; set; }

        /// <summary>
        /// Gets or sets the show in search.
        /// </summary>
        /// <value>
        /// The show in search.
        /// </value>
        public bool? ShowInSearch { get; set; }
    }
}