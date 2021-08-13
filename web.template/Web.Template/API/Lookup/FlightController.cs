namespace Web.Template.API.Lookup
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Domain.Entities.Flight;

    /// <summary>
    /// Web API controller for retrieving information regarding flights
    /// </summary>
    public class FlightController : ApiController
    {
        /// <summary>
        /// The flight service
        /// </summary>
        private readonly IFlightService flightService;

        /// <summary>
        /// The flight cache route service
        /// </summary>
        private readonly IFlightCacheRouteService flightCacheRouteService;

        /// <summary>
        /// The flight cache route service
        /// </summary>
        private readonly IAsyncFlightCacheRouteService asyncFlightCacheRouteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightController" /> class.
        /// </summary>
        /// <param name="flightService">The flight service.</param>
        /// <param name="flightCacheRouteService">The flight cache route service.</param>
        public FlightController(
            IFlightService flightService,
            IFlightCacheRouteService flightCacheRouteService,
            IAsyncFlightCacheRouteService asyncFlightCacheRouteService)
        {
            this.flightService = flightService;
            this.flightCacheRouteService = flightCacheRouteService;
            this.asyncFlightCacheRouteService = asyncFlightCacheRouteService;
        }

        /// <summary>
        /// Gets the flight carriers.
        /// </summary>
        /// <returns>List of flight carriers</returns>
        [Route("api/flightcarrier")]
        [HttpGet]
        public List<FlightCarrier> GetFlightCarriers()
        {
            return this.flightService.GetFlightCarriers();
        }

        /// <summary>
        /// Gets the flight carrier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The FlightCarrier tat matches the ID.</returns>
        [Route("api/flightcarrier/{id}")]
        [HttpGet]
        public FlightCarrier GetFlightCarrier(int id)
        {
            return this.flightService.GetFlightCarrierById(id);
        }


        /// <summary>
        /// Gets the flight classes.
        /// </summary>
        /// <returns>List of flight classes</returns>
        [Route("api/flightclass")]
        [HttpGet]
        public List<FlightClass> GetFlightClasses()
        {
            return this.flightService.GetFlightClasses();
        }

        /// <summary>
        /// Gets the departure dates by airport.
        /// </summary>
        /// <param name="departureAirportId">The departure airport identifier.</param>
        /// <param name="arrivalAirportId">The arrival airport identifier.</param>
        /// <returns>The list of dates</returns>
        [Route("api/flightcacheroute/airport/{departureAirportId}/{arrivalAirportId}")]
        [HttpGet]
        public List<DateTime> GetDepartureDatesByAirport(int departureAirportId, int arrivalAirportId)
        {
            return this.flightCacheRouteService.GetDepartureDatesByAirport(departureAirportId, arrivalAirportId);
        }

        /// <summary>
        /// Gets the departure dates by airport.
        /// </summary>
        /// <param name="departureAirportId">The departure airport identifier.</param>
        /// <param name="arrivalAirportId">The arrival airport identifier.</param>
        /// <returns>The list of dates</returns>
        [Route("api/dealfinderflightcacheroute/airport/{departureAirportId}/{arrivalAirportId}")]
        [HttpGet]
        public async Task<FlightCacheRouteDate[]> GetDepartureDatesByAirportAsync(int departureAirportId, int arrivalAirportId)
        {
            return await this.asyncFlightCacheRouteService.GetDepartureDatesByAirportAsync(departureAirportId, arrivalAirportId, DateTime.Now, CancellationToken.None);
        }

        /// <summary>
        /// Gets the departure dates by region.
        /// </summary>
        /// <param name="departureAirportId">The departure airport identifier.</param>
        /// <param name="geographyLevel2Id">The geography level2 identifier.</param>
        /// <returns>The list of dates</returns>
        [Route("api/flightcacheroute/region/{departureAirportId}/{geographyLevel2Id}")]
        [HttpGet]
        public List<DateTime> GetDepartureDatesByRegion(int departureAirportId, int geographyLevel2Id)
        {
            return this.flightCacheRouteService.GetDepartureDatesByRegion(departureAirportId, geographyLevel2Id);
        }

        /// <summary>
        /// Gets the departure dates by region.
        /// </summary>
        /// <param name="departureAirportId">The departure airport identifier.</param>
        /// <param name="geographyLevel2Id">The geography level2 identifier.</param>
        /// <returns>The list of dates</returns>
        [Route("api/dealfinderflightcacheroute/region/{departureAirportId}/{geographyLevel2Id}")]
        [HttpGet]
        public async Task<FlightCacheRouteDate[]> GetDepartureDatesByRegionAsync(int departureAirportId, int geographyLevel2Id)
        {
            return await this.asyncFlightCacheRouteService.GetDepartureDatesByRegionAsync(departureAirportId, geographyLevel2Id, DateTime.Now, CancellationToken.None);
        }

        /// <summary>
        /// Gets the departure dates by resort.
        /// </summary>
        /// <param name="departureAirportId">The departure airport identifier.</param>
        /// <param name="geographyLevel3Id">The geography level3 identifier.</param>
        /// <returns>The list of dates</returns>
        [Route("api/flightcacheroute/resort/{departureAirportId}/{geographyLevel3Id}")]
        [HttpGet]
        public List<DateTime> GetDepartureDatesByResort(int departureAirportId, int geographyLevel3Id)
        {
            return this.flightCacheRouteService.GetDepartureDatesByResort(departureAirportId, geographyLevel3Id);
        }

        /// <summary>
        /// Gets the departure dates by resort.
        /// </summary>
        /// <param name="departureAirportId">The departure airport identifier.</param>
        /// <param name="geographyLevel3Id">The geography level3 identifier.</param>
        /// <returns>The list of dates</returns>
        [Route("api/dealfinderflightcacheroute/resort/{departureAirportId}/{geographyLevel3Id}")]
        [HttpGet]
        public async Task<FlightCacheRouteDate[]> GetDepartureDatesByResortAsync(int departureAirportId, int geographyLevel3Id)
        {
            return await this.asyncFlightCacheRouteService.GetDepartureDatesByResortAsync(departureAirportId, geographyLevel3Id, DateTime.Now, CancellationToken.None);
        }
    }
}