namespace Web.Template.Domain.Entities.Flight
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Entity representing groups of Airports
    /// </summary>
    [Table("AirportGroup")]
    public partial class AirportGroup : ILookup
    {
        /// <summary>
        /// Gets or sets the city code.
        /// </summary>
        /// <value>
        /// The city code.
        /// </value>
        [StringLength(10)]
        public string CityCode { get; set; }

        /// <summary>
        /// Gets or sets the display on search.
        /// </summary>
        /// <value>
        /// The display on search.
        /// </value>
        public bool? DisplayOnSearch { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("AirportGroupID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Column("AirportGroup")]
        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the resort.
        /// </summary>
        /// <value>
        /// The resort.
        /// </value>
        public Region Region { get; set; }

        /// <summary>
        /// Gets or sets the resort identifier.
        /// </summary>
        /// <value>
        /// The resort identifier.
        /// </value>
        [Column("GeographyLevel2ID")]
        public int RegionId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [Required]
        [StringLength(15)]
        public string Type { get; set; }


        /// <summary>
        /// Gets or sets the airports.
        /// </summary>
        /// <value>
        /// The airports.
        /// </value>
        public List<Airport> Airports { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [preferred group].
		/// </summary>
		/// <value>
		///   <c>true</c> if [preferred group]; otherwise, <c>false</c>.
		/// </value>
		public bool PreferredGroup { get; set; }

		/// <summary>
		/// Returns shallow copy of airport group
		/// </summary>
		/// <returns></returns>
		public AirportGroup ShallowCopy()
        {
            return (AirportGroup)this.MemberwiseClone();
        }
	}
}