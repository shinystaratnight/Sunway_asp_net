namespace Web.Template.Domain.Entities.Geography
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Newtonsoft.Json;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Entity Representing a Region
    /// </summary>
    [Table("GeographyLevel2")]
    public class Region : ILookup
    {
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
        /// Gets or sets the country.
        /// </summary>
        /// <value>
        /// The country.
        /// </value>
        [JsonIgnore]
        public Country Country { get; set; }

        /// <summary>
        /// Gets or sets the country identifier.
        /// </summary>
        /// <value>
        /// The country identifier.
        /// </value>
        [Column("GeographyLevel1ID")]
        [JsonIgnore]
        public int CountryId { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("GeographyLevel2ID")]
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
        /// Gets or sets the resorts.
        /// </summary>
        /// <value>
        /// The resorts.
        /// </value>
        public List<Resort> Resorts { get; set; }
    }
}