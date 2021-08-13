namespace Web.Booking.API.Lookup
{
    using System.Collections.Generic;
    using System.Web.Http;

    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Application.ViewModels.Flight;
    using Web.Template.Domain.Entities.Flight;

    /// <summary>
    /// Web API controller for retrieving information regarding airports
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class AirportController : ApiController
    {
        /// <summary>
        /// The airport service
        /// </summary>
        private readonly IAirportService airportService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AirportController"/> class.
        /// </summary>
        /// <param name="airportService">The airport service.</param>
        public AirportController(IAirportService airportService)
        {
            this.airportService = airportService;
        }

        /// <summary>
        /// Gets the airport.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>An Airport that matches the ID</returns>
        [Route("api/Airport/{id}")]
        [HttpGet]
        public Airport GetAirport(int id)
        {
            return this.airportService.GetAirportById(id);
        }

        /// <summary>
        /// Gets the airport group.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>An airport group that matches the ID.</returns>
        [Route("api/AirportGroup/{id}")]
        [HttpGet]
        public AirportGroup GetAirportGroup(int id)
        {
            return this.airportService.GetAirportGroupById(id);
        }

        /// <summary>
        /// Gets the airports.
        /// </summary>
        /// <returns>All loaded Airports.</returns>
        [Route("api/Airport")]
        [HttpGet]
        public List<Airport> GetAirports()
        {
            return this.airportService.GetBrandAirports();
        }

        /// <summary>
        /// Gets the airports groups.
        /// </summary>
        /// <returns>All Loaded Airport Groups</returns>
        [Route("api/AirportGroup")]
        [HttpGet]
        public List<AirportGroup> GetAirportsGroups()
        {
            return this.airportService.GetAirportGroups();
        }

        /// <summary>
        /// Gets the airports groups.
        /// </summary>
        /// <returns>All Loaded Airport Groups</returns>
        [Route("api/AirportsByGroup/{groupByAirportGroup}/{sortByPreferred}")]
        [HttpGet]
        public AirportGroupModel GetAirportsByGroup(bool groupByAirportGroup, bool sortByPreferred)
        {
            return this.airportService.GetAirportsByGroup(groupByAirportGroup, sortByPreferred);
        }

        /// <summary>
        /// Gets the airports groups.
        /// </summary>
        /// <returns>All Loaded Airport Groups</returns>
        [Route("api/AirportResortGroup")]
        [HttpGet]
        public List<AirportResortGroupModel> GetAirportsResortGroups()
        {
            return this.airportService.GetAirportResortGroups();
        }

        /// <summary>
        /// Gets the airports groups.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>All Loaded Airport Groups</returns>
        [Route("api/RouteAvailability/{id}")]
        [HttpGet]
        public RouteAvailability GetRoute(int id)
        {
            return this.airportService.GetRouteAvailabilityById(id);
        }

        /// <summary>
        /// Gets the airports.
        /// </summary>
        /// <returns>All loaded Airports.</returns>
        [Route("api/RouteAvailability")]
        [HttpGet]
        public List<RouteAvailabilityModel> GetRoutes()
        {
            return this.airportService.GetRouteAvailabilities();
        }
    }
}