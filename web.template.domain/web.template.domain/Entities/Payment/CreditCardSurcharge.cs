namespace Web.Template.Domain.Entities.Payment
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Entity class mapped to the Credit Card surcharge table
    /// </summary>
    [Table("CreditCardSurcharge")]
    public partial class CreditCardSurcharge : ILookup
    {
        /// <summary>
        /// Gets or sets the credit card type identifier.
        /// </summary>
        /// <value>
        /// The credit card type identifier.
        /// </value>
        [Column("CreditCardTypeID")]
        public int CreditCardTypeId { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("CreditCardSurchargeID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the selling geography level1 identifier.
        /// </summary>
        /// <value>
        /// The selling geography level1 identifier.
        /// </value>
        [Column("SellingGeographyLevel1ID")]
        public int SellingGeographyLevel1Id { get; set; }

        /// <summary>
        /// Gets or sets the surcharge percentage.
        /// </summary>
        /// <value>
        /// The surcharge percentage.
        /// </value>
        public decimal SurchargePercentage { get; set; }

        /// <summary>
        /// Gets or sets the type of the surcharge.
        /// </summary>
        /// <value>
        /// The type of the surcharge.
        /// </value>
        [StringLength(15)]
        public string SurchargeType { get; set; }

        /// <summary>
        /// Gets or sets the use credit card.
        /// </summary>
        /// <value>
        /// The use credit card.
        /// </value>
        public bool? UseCreditCard { get; set; }
    }
}