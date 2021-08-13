namespace Web.Template.Domain.Entities.Booking
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Entity class for nationalities
    /// </summary>
    [Table("Nationality")]
    public partial class Nationality : ILookup
    {
        /// <summary>
        /// Gets or sets the nationality identifier.
        /// </summary>
        /// <value>
        /// The nationality identifier.
        /// </value>
        [Column("NationalityID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the iso code.
        /// </summary>
        /// <value>
        /// The iso code.
        /// </value>
        [StringLength(4)]
        public string ISOCode { get; set; }

        /// <summary>
        /// Gets or sets the nationality1.
        /// </summary>
        /// <value>
        /// The nationality1.
        /// </value>
        [Column("Nationality")]
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the selling geography level1 identifier.
        /// </summary>
        /// <value>
        /// The selling geography level1 identifier.
        /// </value>
        public int? SellingGeographyLevel1ID { get; set; }
    }
}