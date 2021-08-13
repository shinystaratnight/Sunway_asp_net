namespace Web.Template.Domain.Entities.Geography
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Newtonsoft.Json;

    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Entity Representing a resort
    /// </summary>
    [Table("GeographyLevel3")]
    public class Resort : ILookup
    {
        /// <summary>
        /// Gets or sets the airports.
        /// </summary>
        /// <value>
        /// The airports.
        /// </value>
        public ICollection<Airport> Airports { get; set; }

        /// <summary>
        /// Gets or sets the brands the resort is available for.
        /// </summary>
        /// <value>
        /// The brands.
        /// </value>
        [JsonIgnore]
        public List<Brand> Brands { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key]
        [Column("GeographyLevel3ID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the obsolete.
        /// </summary>
        /// <value>
        /// The obsolete.
        /// </value>
        public bool? Obsolete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [preferred location].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [preferred location]; otherwise, <c>false</c>.
        /// </value>
        public bool PreferredLocation { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>
        /// The region.
        /// </value>
        [JsonIgnore]
        public Region Region { get; set; }

        /// <summary>
        /// Gets or sets the region identifier.
        /// </summary>
        /// <value>
        /// The region identifier.
        /// </value>
        [Column("GeographyLevel2ID")]
        [JsonIgnore]
        public int RegionID { get; set; }
    }
}