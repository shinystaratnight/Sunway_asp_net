namespace Web.Template.Domain.Entities.Booking
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Language entity.
    /// </summary>
    [Table("Language")]
    public partial class Language : ILookup
    {
        /// <summary>
        /// Gets or sets the culture code.
        /// </summary>
        /// <value>
        /// The culture code.
        /// </value>
        [StringLength(10)]
        public string CultureCode { get; set; }

        /// <summary>
        /// Gets or sets the customer language.
        /// </summary>
        /// <value>
        /// The customer language.
        /// </value>
        public bool? CustomerLanguage { get; set; }

        /// <summary>
        /// Gets or sets the default language.
        /// </summary>
        /// <value>
        /// The default language.
        /// </value>
        public bool? DefaultLanguage { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("LanguageID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        [Required]
        [StringLength(10)]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Column("Language")]
        [Required]
        [StringLength(40)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the system language.
        /// </summary>
        /// <value>
        /// The system language.
        /// </value>
        public bool? SystemLanguage { get; set; }
    }
}