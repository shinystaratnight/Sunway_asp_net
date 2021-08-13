namespace Web.Template.Domain.Entities.Geography
{
    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    ///     A class representing a group of geography.
    /// </summary>
    public class GeographyGrouping : ILookup
    {
        /// <summary>
        /// Gets or sets the brand.
        /// </summary>
        /// <value>
        /// The brand.
        /// </value>
        public int BrandID { get; set; }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public string Level { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [show in search].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [show in search]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowInSearch { get; set; }
    }
}