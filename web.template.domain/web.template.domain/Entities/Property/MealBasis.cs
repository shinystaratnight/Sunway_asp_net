namespace Web.Template.Domain.Entities.Property
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Meal basis entity representing the type of catering a room comes with e.g. half board.
    /// </summary>
    public partial class MealBasis : ILookup
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("MealBasisID")]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the meal basis code.
        /// </summary>
        /// <value>
        /// The meal basis code.
        /// </value>
        [Required]
        [StringLength(10)]
        public string MealBasisCode { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Column("MealBasis")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}