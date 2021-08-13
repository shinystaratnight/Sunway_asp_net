namespace Web.Template.Domain.Entities.Booking
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Entity class linked to the booking documentation table, containing information about core documents we will send to the user.
    /// </summary>
    [Table("BookingDocumentation")]
    public partial class BookingDocumentation : ILookup
    {
        /// <summary>
        /// Gets or sets the name of the document.
        /// </summary>
        /// <value>
        /// The name of the document.
        /// </value>
        [Required]
        [StringLength(50)]
        public string DocumentName { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("BookingDocumentationID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the recipient.
        /// </summary>
        /// <value>
        /// The recipient.
        /// </value>
        [Required]
        [StringLength(30)]
        public string Recipient { get; set; }

        /// <summary>
        /// Gets or sets the trade document.
        /// </summary>
        /// <value>
        /// The trade document.
        /// </value>
        public bool? TradeDocument { get; set; }
    }
}