namespace Web.Template.Domain.Entities.Booking
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Repository linked to a trade.
    /// </summary>
    /// <seealso cref="Web.Template.Domain.Interfaces.Entity.ILookup" />
    [Table("Trade")]
    public partial class Trade : ILookup
    {
        /// <summary>
        /// Gets or sets the ABTA ATOL number.
        /// </summary>
        /// <value>The ABTA ATOL number.</value>
        public string ABTAATOLNumber { get; set; }

        /// <summary>
        /// Gets or sets the address1.
        /// </summary>
        /// <value>
        /// The address1.
        /// </value>
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the address2.
        /// </summary>
        /// <value>
        /// The address2.
        /// </value>
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the telephone.
        /// </summary>
        /// <value>
        /// The telephone.
        /// </value>
        [Column("BookingCountryID")]
        public int BookingCountryId { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("TradeID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the trade.
        /// </summary>
        /// <value>
        /// The name of the trade.
        /// </value>
        [Column("TradeName")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [non transacting].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [non transacting]; otherwise, <c>false</c>.
        /// </value>
        public bool NonTransacting { get; set; }

        /// <summary>
        /// Gets or sets the post code.
        /// </summary>
        /// <value>
        /// The post code.
        /// </value>
        public string PostCode { get; set; }

        /// <summary>
        /// Gets or sets the telephone.
        /// </summary>
        /// <value>
        /// The telephone.
        /// </value>
        public string Telephone { get; set; }

        /// <summary>
        /// Gets or sets the town city.
        /// </summary>
        /// <value>
        /// The town city.
        /// </value>
        public string TownCity { get; set; }

        /// <summary>
        /// Gets or sets the trade group identifier.
        /// </summary>
        /// <value>
        /// The trade group identifier.
        /// </value>
        [Column("TradeGroupID")]
        public int TradeGroupId { get; set; }

        /// <summary>
        /// Gets or sets the website.
        /// </summary>
        /// <value>The website.</value>
        public string Website { get; set; }

        /// <summary>
        /// Gets or sets the county.
        /// </summary>
        /// <value>The county.</value>
        public string County { get; set; }
    }
}