namespace Web.Template.Application.ViewModels.Flight
{
    using System.Collections.Generic;

    /// <summary>
    /// View model for RouteAvailability regions
    /// </summary>
    public class RouteRegionModel
    {
        /// <summary>
        /// Gets or sets the region identifier.
        /// </summary>
        /// <value>
        /// The region identifier.
        /// </value>
        public int RegionId { get; set; }

        /// <summary>
        /// Gets or sets the resort ids.
        /// </summary>
        /// <value>
        /// The resort ids.
        /// </value>
        public List<int> ResortIds { get; set; }
    }
}