namespace Web.Template.Domain.Entities.Booking
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Newtonsoft.Json;

    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Brand entity representing information regarding the brand itself such as the default selling geography.
    /// </summary>
    [Table("Brand")]
    public partial class Brand : ILookup
    {
        /// <summary>
        /// Gets or sets the brand code.
        /// </summary>
        /// <value>
        /// The brand code.
        /// </value>
        [Required]
        [StringLength(20)]
        public string BrandCode { get; set; }

        /// <summary>
        /// Gets or sets the brand geography.
        /// </summary>
        /// <value>
        /// The brand geography.
        /// </value>
        [JsonIgnore]
        public List<Resort> BrandGeography { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [default brand].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [default brand]; otherwise, <c>false</c>.
        /// </value>
        public bool DefaultBrand { get; set; }

        /// <summary>
        /// Gets or sets the default country of residence g l1 identifier.
        /// </summary>
        /// <value>
        /// The default country of residence g l1 identifier.
        /// </value>
        [Column("DefaultCountryOfResidenceGL1ID")]
        public int? DefaultCountryOfResidenceGL1Id { get; set; }

        /// <summary>
        /// Gets or sets the default customer currency identifier.
        /// </summary>
        /// <value>
        /// The default customer currency identifier.
        /// </value>
        [Column("DefaultCustomerCurrencyID")]
        public int? DefaultCustomerCurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the default nationality identifier.
        /// </summary>
        /// <value>
        /// The default nationality identifier.
        /// </value>
        public int? DefaultNationalityId { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("BrandID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the include nationality and residency.
        /// </summary>
        /// <value>
        /// The include nationality and residency.
        /// </value>
        public bool? IncludeNationalityAndResidency { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Column("BrandName")]
        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the selling geography level1 identifier.
        /// </summary>
        /// <value>
        /// The selling geography level1 identifier.
        /// </value>
        [Column("SellingGeographyLevel1ID")]
        public int? SellingGeographyLevel1Id { get; set; }
    }
}