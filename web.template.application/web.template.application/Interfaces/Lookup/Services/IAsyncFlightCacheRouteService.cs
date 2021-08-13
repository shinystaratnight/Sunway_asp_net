namespace Web.Template.Application.Interfaces.Lookup.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Web.Template.Domain.Entities.Flight;

    /// <summary>
    /// Interface IFlightCacheRouteService
    /// </summary>
    public interface IAsyncFlightCacheRouteService
    {
        /// <summary>
        /// Gets the departure dates by airport.
        /// </summary>
        /// <param name="departureAirportId">The departure airport identifier.</param>
        /// <param name="arrivalAirportId">The arrival airport identifier.</param>
        /// <param name="searchDate">The date being searched.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of dates.</returns>
        Task<FlightCacheRouteDate[]> GetDepartureDatesByAirportAsync(
            int departureAirportId,
            int arrivalAirportId, 
            DateTime? searchDate,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets the departure dates by region.
        /// </summary>
        /// <param name="departureAirportId">The departure airport identifier.</param>
        /// <param name="geographyLevel2Id">The geography level2 identifier.</param>
        /// <param name="startDate">The date being searched.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of dates.</returns>
        Task<FlightCacheRouteDate[]> GetDepartureDatesByRegionAsync(
            int departureAirportId, 
            int geographyLevel2Id, 
            DateTime? startDate, 
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets the departure dates by resort.
        /// </summary>
        /// <param name="departureAirportId">The departure airport identifier.</param>
        /// <param name="geographyLevel3Id">The geography level3 identifier.</param>
        /// <param name="searchDate">The date being searched.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of dates.</returns>
        Task<FlightCacheRouteDate[]> GetDepartureDatesByResortAsync(
            int departureAirportId,
            int geographyLevel3Id, 
            DateTime? searchDate, 
            CancellationToken cancellationToken);
    }
}