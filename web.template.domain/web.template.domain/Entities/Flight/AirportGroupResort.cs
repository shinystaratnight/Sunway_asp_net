namespace Web.Template.Domain.Entities.Flight
{
    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Entity representing groups of Airports
    /// </summary>
    public partial class AirportGroupResort : ILookup
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public int AirportGroupId { get; set; }

        /// <summary>
        /// Gets or sets the geography level1 identifier.
        /// </summary>
        /// <value>
        /// The geography level1 identifier.
        /// </value>
        public int GeographyLevel1Id { get; set; }

        /// <summary>
        /// Gets or sets the geography level3 identifier.
        /// </summary>
        /// <value>
        /// The geography level3 identifier.
        /// </value>
        public int GeographyLevel3Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
    }
}