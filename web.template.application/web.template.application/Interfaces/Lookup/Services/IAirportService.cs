namespace Web.Template.Application.Interfaces.Lookup.Services
{
    using System.Collections.Generic;

    using Web.Template.Application.ViewModels.Flight;
    using Web.Template.Domain.Entities.Flight;

    /// <summary>
    /// Airport service responsible for access to Airport information
    /// </summary>
    public interface IAirportService
    {
        /// <summary>
        /// Gets the airport group by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>An Airport Group with the matching ID</returns>
        AirportGroup GetAirportGroupById(int id);

        /// <summary>
        /// Gets the airport groups.
        /// </summary>
        /// <returns>All Airport Groups</returns>
        List<AirportGroup> GetAirportGroups();

        /// <summary>
        /// Gets the airport group resorts.
        /// </summary>
        /// <returns></returns>
        List<AirportGroupResort> GetAirportGroupResorts();

            /// <summary>
        /// Gets the airport resort groups.
        /// </summary>
        /// <returns></returns>
        List<AirportResortGroupModel> GetAirportResortGroups();

        /// <summary>
        /// Gets the airports by group.
        /// </summary>
        /// <returns></returns>
        AirportGroupModel GetAirportsByGroup(bool groupByAirportGroup, bool sortByPreferred);

            /// <summary>
        /// Gets the airport by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>An Airport with the matching ID</returns>
        Airport GetAirportById(int id);

        /// <summary>
        /// Gets the airports.
        /// </summary>
        /// <returns>All Airports</returns>
        List<Airport> GetBrandAirports();

        /// <summary>
        /// Gets the route availabilities.
        /// </summary>
        /// <returns>All route Availability</returns>
        List<RouteAvailabilityModel> GetRouteAvailabilities();

        /// <summary>
        /// Gets the route availability by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A route availability</returns>
        RouteAvailability GetRouteAvailabilityById(int id);
    }
}