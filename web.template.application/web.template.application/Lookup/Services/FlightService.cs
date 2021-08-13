namespace Web.Template.Application.Lookup.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;

    /// <summary>
    /// Flight service responsible for access to flight information
    /// </summary>
    public class FlightService : IFlightService
    {
        /// <summary>
        /// The flight carrier repository
        /// </summary>
        private readonly IFlightCarrierRepository flightCarrierRepository;

        /// <summary>
        /// The flight class repository
        /// </summary>
        private readonly IFlightClassRepository flightClassRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightService" /> class.
        /// </summary>
        /// <param name="flightCarrierRepository">The flight carrier repository.</param>
        /// <param name="flightClassRepository">The flight class repository.</param>
        public FlightService(IFlightCarrierRepository flightCarrierRepository, IFlightClassRepository flightClassRepository)
        {
            this.flightCarrierRepository = flightCarrierRepository;
            this.flightClassRepository = flightClassRepository;
        }

        /// <summary>
        /// Gets the flight carrier by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The FlightCarrier.</returns>
        public FlightCarrier GetFlightCarrierById(int id)
        {
            return this.flightCarrierRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the flight carriers.
        /// </summary>
        /// <returns>List of Flight Carriers</returns>
        public List<FlightCarrier> GetFlightCarriers()
        {
            return this.flightCarrierRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets the flight class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The FlightClass.</returns>
        public FlightClass GetFlightClassById(int id)
        {
            return this.flightClassRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the flight classes.
        /// </summary>
        /// <returns>List of flight classes</returns>
        public List<FlightClass> GetFlightClasses()
        {
            return this.flightClassRepository.GetAll().ToList();
        }
    }
}