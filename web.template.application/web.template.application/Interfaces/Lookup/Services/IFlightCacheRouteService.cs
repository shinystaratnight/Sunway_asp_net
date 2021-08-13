namespace Web.Template.Application.Interfaces.Lookup.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Web.Template.Domain.Entities.Flight;

    /// <summary>
    /// Interface IFlightCacheRouteService
    /// </summary>
    public interface IFlightCacheRouteService
    {
        /// <summary>
        /// Gets the departure dates by airport.
        /// </summary>
        /// <param name="departureAirportId">The departure airport identifier.</param>
        /// <param name="arrivalAirportId">The arrival airport identifier.</param>
        /// <returns>The list of dates.</returns>
        List<DateTime> GetDepartureDatesByAirport(
            int departureAirportId,
            int arrivalAirportId);

        /// <summary>
        /// Gets the departure dates by region.
        /// </summary>
        /// <param name="departureAirportId">The departure airport identifier.</param>
        /// <param name="geographyLevel2Id">The geography level2 identifier.</param>
        /// <returns>The list of dates.</returns>
        List<DateTime> GetDepartureDatesByRegion(
            int departureAirportId,
            int geographyLevel2Id);

        /// <summary>
        /// Gets the departure dates by resort.
        /// </summary>
        /// <param name="departureAirportId">The departure airport identifier.</param>
        /// <param name="geographyLevel3Id">The geography level3 identifier.</param>
        /// <returns>The list of dates.</returns>
        List<DateTime> GetDepartureDatesByResort(
            int departureAirportId,
            int geographyLevel3Id);
    }
}