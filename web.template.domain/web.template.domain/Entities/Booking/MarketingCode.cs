namespace Web.Template.Domain.Entities.Booking
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Entity representing a Marketing code
    /// </summary>
    [Table("MarketingCode")]
    public partial class MarketingCode : ILookup
    {
        /// <summary>
        /// Gets or sets the brand.
        /// </summary>
        /// <value>
        /// The brand.
        /// </value>
        public Brand Brand { get; set; }

        /// <summary>
        /// Gets or sets the brand identifier.
        /// </summary>
        /// <value>
        /// The brand identifier.
        /// </value>
        public int? BrandID { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("MarketingCodeID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the include in hotel request.
        /// </summary>
        /// <value>
        /// The include in hotel request.
        /// </value>
        public bool? IncludeInHotelRequest { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Column("MarketingCode")]
        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the sales channel.
        /// </summary>
        /// <value>
        /// The sales channel.
        /// </value>
        public SalesChannel SalesChannel { get; set; }

        /// <summary>
        /// Gets or sets the sales channel identifier.
        /// </summary>
        /// <value>
        /// The sales channel identifier.
        /// </value>
        public int? SalesChannelID { get; set; }
    }
}