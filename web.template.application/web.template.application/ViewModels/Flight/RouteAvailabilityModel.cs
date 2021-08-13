namespace Web.Template.Application.ViewModels.Flight
{
    using System.Collections.Generic;

    /// <summary>
    /// View model for route availabilities.
    /// </summary>
    public class RouteAvailabilityModel
    {
        /// <summary>
        /// Gets or sets the arrival airport i ds.
        /// </summary>
        /// <value>
        /// The arrival airport i ds.
        /// </value>
        public List<int> ArrivalAirportIDs { get; set; }

        /// <summary>
        /// Gets or sets the arrival regions.
        /// </summary>
        /// <value>
        /// The arrival regions.
        /// </value>
        public List<RouteRegionModel> ArrivalRegions { get; set; }

        /// <summary>
        /// Gets or sets the departure airport identifier.
        /// </summary>
        /// <value>
        /// The departure airport identifier.
        /// </value>
        public int DepartureAirportGroupId { get; set; }

        /// <summary>
        /// Gets or sets the departure airport identifier.
        /// </summary>
        /// <value>
        /// The departure airport identifier.
        /// </value>
        public int DepartureAirportId { get; set; }
    }
}