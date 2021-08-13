namespace Web.Template.Domain.Entities.Booking
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Interfaces.Entity;

    /// <summary>
    ///     URL301Redirect entity representing pages that need redirecting.
    /// </summary>
    [Table("URL301Redirect")]
    public class URL301Redirect : ILookup
    {
        /// <summary>
        ///     Gets or sets the brand identifier.
        /// </summary>
        /// <value>
        ///     The brand identifier.
        /// </value>
        [Column("BrandID")]
        public int BrandID { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [Column("RedirectURL")]
        public string RedirectURL { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [Column("URL")]
        public string URL { get; set; }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        [Column("URL301RedirectID")]
        public int Id { get; set; }
    }
}