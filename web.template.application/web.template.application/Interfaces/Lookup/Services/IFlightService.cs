namespace Web.Template.Application.Interfaces.Lookup.Services
{
    using System.Collections.Generic;

    using Web.Template.Domain.Entities.Flight;

    /// <summary>
    /// Flight service responsible for access to flight information
    /// </summary>
    public interface IFlightService
    {
        /// <summary>
        /// Gets the flight carrier by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The FlightCarrier.</returns>
        FlightCarrier GetFlightCarrierById(int id);

        /// <summary>
        /// Gets the flight carriers.
        /// </summary>
        /// <returns>List of Flight Carriers</returns>
        List<FlightCarrier> GetFlightCarriers();

        /// <summary>
        /// Gets the flight class by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The FlightClass.</returns>
        FlightClass GetFlightClassById(int id);

        /// <summary>
        /// Gets the flight classes.
        /// </summary>
        /// <returns>List of flight classes.</returns>
        List<FlightClass> GetFlightClasses();
    }
}