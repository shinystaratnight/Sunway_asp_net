namespace Web.Template.Application.ViewModels.Flight
{
    using System.Collections.Generic;

    using Web.Template.Domain.Entities.Geography;

    /// <summary>
    /// View model for RouteAvailability regions
    /// </summary>
    public class AirportResortGroupModel
    {
        /// <summary>
        /// Gets or sets the region identifier.
        /// </summary>
        /// <value>
        /// The region identifier.
        /// </value>
        public int AirportGroupId { get; set; }

        /// <summary>
        /// Gets or sets the name of the airport group.
        /// </summary>
        /// <value>
        /// The name of the airport group.
        /// </value>
        public string AirportGroupName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [display on search].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [display on search]; otherwise, <c>false</c>.
        /// </value>
        public bool? DisplayOnSearch { get; set; }

        /// <summary>
        /// Gets or sets the associated geography level1 identifier.
        /// </summary>
        /// <value>
        /// The associated geography level1 identifier.
        /// </value>
        public int? AssociatedGeographyLevel1Id { get; set; }

        /// <summary>
        /// Gets or sets the resort ids.
        /// </summary>
        /// <value>
        /// The resort ids.
        /// </value>
        public List<AirportResortGroupResortModel> Resorts { get; set; }
    }
}