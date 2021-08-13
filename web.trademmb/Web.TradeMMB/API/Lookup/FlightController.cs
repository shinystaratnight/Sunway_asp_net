namespace Web.TradeMMB.API.Lookup
{
    using System.Collections.Generic;
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
        /// Initializes a new instance of the <see cref="FlightController"/> class.
        /// </summary>
        /// <param name="flightService">The flight service.</param>
        public FlightController(IFlightService flightService)
        {
            this.flightService = flightService;
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
        /// Gets the flight classes.
        /// </summary>
        /// <returns>List of flight classes</returns>
        [Route("api/flightclass")]
        [HttpGet]
        public List<FlightClass> GetFlightClasses()
        {
            return this.flightService.GetFlightClasses();
        }
    }
}