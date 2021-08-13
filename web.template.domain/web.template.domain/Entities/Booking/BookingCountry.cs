namespace Web.Template.Domain.Entities.Booking
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Entity representing country that a booking is made against
    /// </summary>
    [Table("BookingCountry")]
    public partial class BookingCountry : ILookup
    {
        /// <summary>
        /// Gets or sets the currency identifier.
        /// </summary>
        /// <value>
        /// The currency identifier.
        /// </value>
        public int? CurrencyID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [default country].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [default country]; otherwise, <c>false</c>.
        /// </value>
        public bool DefaultCountry { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("BookingCountryID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the iso code.
        /// </summary>
        /// <value>
        /// The iso code.
        /// </value>
        [StringLength(3)]
        public string ISOCode { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        /// <value>
        /// The locale.
        /// </value>
        [StringLength(30)]
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Column("BookingCountry")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}