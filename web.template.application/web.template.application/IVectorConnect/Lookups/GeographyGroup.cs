namespace Web.Template.Application.IVectorConnect.Lookups
{
    /// <summary>
    ///     Class used to map to Connect Geography group lookup
    /// </summary>
    public class GeographyGrouping
    {
        /// <summary>
        ///     Gets or sets the brand identifier.
        /// </summary>
        /// <value>
        ///     The brand identifier.
        /// </value>
        public int BrandID { get; set; }

        /// <summary>
        ///     Gets or sets the geography group.
        /// </summary>
        /// <value>
        ///     The geography group.
        /// </value>
        public string GeographyGroup { get; set; }

        /// <summary>
        ///     Gets or sets the geography grouping identifier.
        /// </summary>
        /// <value>
        ///     The geography grouping identifier.
        /// </value>
        public int GeographyGroupingID { get; set; }

        /// <summary>
        ///     Gets or sets the level.
        /// </summary>
        /// <value>
        ///     The level.
        /// </value>
        public string Level { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [show in search].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [show in search]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowInSearch { get; set; }
    }
}