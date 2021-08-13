namespace Web.Template.Application.IVectorConnect.Lookups
{
    /// <summary>
    ///     Class used to map to Connects Location Lookup
    /// </summary>
    public class Location
    {
        /// <summary>
        ///     Gets or sets the geography level1 identifier.
        /// </summary>
        /// <value>
        ///     The geography level1 identifier.
        /// </value>
        public int GeographyLevel1ID { get; set; }

        /// <summary>
        ///     Gets or sets the name of the geography level1.
        /// </summary>
        /// <value>
        ///     The name of the geography level1.
        /// </value>
        public string GeographyLevel1Name { get; set; }

        /// <summary>
        ///     Gets or sets the geography level2 identifier.
        /// </summary>
        /// <value>
        ///     The geography level2 identifier.
        /// </value>
        public int GeographyLevel2ID { get; set; }

        /// <summary>
        ///     Gets or sets the name of the geography level2.
        /// </summary>
        /// <value>
        ///     The name of the geography level2.
        /// </value>
        public string GeographyLevel2Name { get; set; }

        /// <summary>
        ///     Gets or sets the geography level3 identifier.
        /// </summary>
        /// <value>
        ///     The geography level3 identifier.
        /// </value>
        public int GeographyLevel3ID { get; set; }

        /// <summary>
        ///     Gets or sets the name of the geography level3.
        /// </summary>
        /// <value>
        ///     The name of the geography level3.
        /// </value>
        public string GeographyLevel3Name { get; set; }

        /// <summary>
        ///     Gets or sets the level1 code.
        /// </summary>
        /// <value>
        ///     The level1 code.
        /// </value>
        public string Level1Code { get; set; }

        /// <summary>
        ///     Gets or sets the level2 code.
        /// </summary>
        /// <value>
        ///     The level2 code.
        /// </value>
        public string Level2Code { get; set; }

        /// <summary>
        ///     Gets or sets the level3 code.
        /// </summary>
        /// <value>
        ///     The level3 code.
        /// </value>
        public string Level3Code { get; set; }
    }
}