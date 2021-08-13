namespace Web.Template.Application.ViewModels.Flight
{
    using System.Collections.Generic;

    using Web.Template.Domain.Entities.Flight;

    /// <summary>
    /// View model for RouteAvailability regions
    /// </summary>
    public class AirportGroupModel
    {

        /// <summary>
        /// Gets or sets the airport groups.
        /// </summary>
        /// <value>
        /// The airport groups.
        /// </value>
        public List<AirportGroup> AirportGroups { get; set; }

        /// <summary>
        /// Gets or sets the airports.
        /// </summary>
        /// <value>
        /// The airports.
        /// </value>
        public List<Airport> Airports { get; set; }
    }
}