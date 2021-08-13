namespace Web.Template.Domain.Entities.Extras
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Class ExtraType.
    /// </summary>
    [Table("ExtraType")]
    public class ExtraType : ILookup
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [Column("ExtraTypeID")]
        [Required]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Column("ExtraType")]
        [StringLength(50)]
        [Required]
        public string Name { get; set; }
    }
}
