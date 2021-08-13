namespace Web.Template.Domain.Entities.Geography
{
    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    ///     A class representing a group of geography.
    /// </summary>
    public class GeographyGroupingGeography : ILookup
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int GeographyGroupingId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public int GeographyId { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
    }
}