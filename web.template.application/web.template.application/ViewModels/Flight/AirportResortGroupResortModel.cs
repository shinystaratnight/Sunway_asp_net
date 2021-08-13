namespace Web.Template.Application.ViewModels.Flight
{
    /// <summary>
    /// View model for list of resorts in AirportResortGroupModel
    /// </summary>
    public class AirportResortGroupResortModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the geography level1 identifier.
        /// </summary>
        /// <value>
        /// The geography level1 identifier.
        /// </value>
        public int GeographyLevel1Id { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }
    }
}