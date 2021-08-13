namespace Web.Template.Domain.Entities.Payment
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Entity Table linked to the Credit Card type table
    /// </summary>
    [Table("CreditCardType")]
    public partial class CreditCardType : ILookup
    {
        /// <summary>
        /// Gets or sets the CVV optional.
        /// </summary>
        /// <value>
        /// The CVV optional.
        /// </value>
        public bool? CVVOptional { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("CreditCardTypeID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Column("CreditCardType")]
        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the selling geography level1 identifier.
        /// </summary>
        /// <value>
        /// The selling geography level1 identifier.
        /// </value>
        public int? SellingGeographyLevel1ID { get; set; }

        /// <summary>
        /// Gets or sets the surcharge percentage.
        /// </summary>
        /// <value>
        /// The surcharge percentage.
        /// </value>
        public decimal SurchargePercentage { get; set; }
    }
}