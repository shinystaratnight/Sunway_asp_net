namespace Web.Template.Application.IVectorConnect.Lookups
{
    /// <summary>
    ///     A class used to map to the connect geography grouping geography lookup
    /// </summary>
    public class GeographyGroupingGeography
    {
        /// <summary>
        ///     Gets or sets the geography grouping identifier.
        /// </summary>
        /// <value>
        ///     The geography grouping identifier.
        /// </value>
        public int GeographyGroupingID { get; set; }

        /// <summary>
        ///     Gets or sets the geography identifier.
        /// </summary>
        /// <value>
        ///     The geography identifier.
        /// </value>
        public int GeographyID { get; set; }
    }
}