namespace Web.Template.Domain.Entities.Payment
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Entity Class mapped to the Currency table
    /// </summary>
    [Table("Currency")]
    public partial class Currency : ILookup
    {
        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        /// <value>
        /// The currency code.
        /// </value>
        [Required]
        [StringLength(10)]
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the customer symbol override.
        /// </summary>
        /// <value>
        /// The customer symbol override.
        /// </value>
        [StringLength(10)]
        public string CustomerSymbolOverride { get; set; }

        /// <summary>
        /// Gets or sets the display decimals.
        /// </summary>
        /// <value>
        /// The display decimals.
        /// </value>
        public int? DisplayDecimals { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("CurrencyID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [local currency].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [local currency]; otherwise, <c>false</c>.
        /// </value>
        public bool LocalCurrency { get; set; }

        /// <summary>
        /// Gets or sets the currency1.
        /// </summary>
        /// <value>
        /// The currency1.
        /// </value>
        [Column("Currency")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the selling currency identifier.
        /// </summary>
        /// <value>
        /// The selling currency identifier.
        /// </value>
        [Column("SellingCurrencyID")]
        public int SellingCurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        /// <value>
        /// The symbol.
        /// </value>
        [StringLength(10)]
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the symbol position.
        /// </summary>
        /// <value>
        /// The symbol position.
        /// </value>
        [StringLength(10)]
        public string SymbolPosition { get; set; }
    }
}