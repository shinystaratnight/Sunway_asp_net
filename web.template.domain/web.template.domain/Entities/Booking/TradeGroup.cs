namespace Web.Template.Domain.Entities.Booking
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Repository linked to a trade.
    /// </summary>
    /// <seealso cref="Web.Template.Domain.Interfaces.Entity.ILookup" />
    [Table("TradeGroup")]
    public partial class TradeGroup : ILookup
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the trade parent group.
        /// </summary>
        /// <value>
        /// The trade parent group.
        /// </value>
        public string TradeParentGroup { get; set; }

        /// <summary>
        /// Gets or sets the trade parent group identifier.
        /// </summary>
        /// <value>
        /// The trade parent group identifier.
        /// </value>
        public string TradeParentGroupId { get; set; }
    }
}