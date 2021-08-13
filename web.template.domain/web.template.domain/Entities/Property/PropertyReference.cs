namespace Web.Template.Domain.Entities.Property
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Entity class mapped to the PropertyReference table.
    /// </summary>
    /// <seealso cref="Web.Template.Domain.Interfaces.Entity.ILookup" />
    [Table("PropertyReference")]
    public partial class PropertyReference : ILookup
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PropertyReference"/> is current.
        /// </summary>
        /// <value>
        ///   <c>true</c> if current; otherwise, <c>false</c>.
        /// </value>
        [Column("CurrentPropertyReference")]
        public bool Current { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("PropertyReferenceID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Column("PropertyName")]
        public string Name { get; set; }
    }
}