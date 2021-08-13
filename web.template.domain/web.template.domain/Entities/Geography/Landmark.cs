namespace Web.Template.Domain.Entities.Geography
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Entity class mapped to the Landmark table
    /// </summary>
    [Table("Landmark")]
    public partial class Landmark : ILookup
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("LandmarkID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>
        /// The latitude.
        /// </value>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        /// <value>
        /// The longitude.
        /// </value>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [StringLength(200)]
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