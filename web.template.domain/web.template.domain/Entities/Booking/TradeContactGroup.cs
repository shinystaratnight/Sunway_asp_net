namespace Web.Template.Domain.Entities.Booking
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// The Trade Contact class
    /// </summary>
    /// <seealso cref="Web.Template.Domain.Interfaces.Entity.ILookup" />
    [Table("TradeContactGroup")]
    public partial class TradeContactGroup : ILookup
    {
        /// <summary>
        /// Gets or sets the trade contact group identifier.
        /// </summary>
        /// <value>
        /// The trade contact group identifier.
        /// </value>
        [Column("TradeContactGroupID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the trade contact group identifier.
        /// </summary>
        /// <value>
        /// The trade contact group identifier.
        /// </value>
        [Column("TradeContactGroup")]
        public string Name { get; set; }
    }
}