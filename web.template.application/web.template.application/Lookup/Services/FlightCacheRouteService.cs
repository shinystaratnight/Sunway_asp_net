namespace Web.Template.Application.Lookup.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;

    /// <summary>
    /// Class FlightCacheRouteService.
    /// </summary>
    public class FlightCacheRouteService : IFlightCacheRouteService
    {
        /// <summary>
        /// The flight cache route repository
        /// </summary>
        private readonly IFlightCacheRouteRepository flightCacheRouteRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightCacheRouteService" /> class.
        /// </summary>
        /// <param name="flightCacheRouteRepository">The flight cache route repository.</param>
        public FlightCacheRouteService(IFlightCacheRouteRepository flightCacheRouteRepository)
        {
            this.flightCacheRouteRepository = flightCacheRouteRepository;
        }

        /// <summary>
        /// Gets the departure dates by airport.
        /// </summary>
        /// <param name="departureAirportId">The departure airport identifier.</param>
        /// <param name="arrivalAirportId">The arrival airport identifier.</param>
        /// <param name="searchDate">The date being searched.</param>
        /// <returns>The list of dates.</returns>
        public List<DateTime> GetDepartureDatesByAirport(int departureAirportId, int arrivalAirportId)
        {
            var dates =
                this.flightCacheRouteRepository.GetAll()
                    .Where(route => route.DepartureAirportId == departureAirportId && route.ArrivalAirportId == arrivalAirportId)
                    .SelectMany(route => route.DepartureDates)
                    .Distinct()
                    .ToList();

            return dates;
        }

        /// <summary>
        /// Gets the departure dates by region.
        /// </summary>
        /// <param name="departureAirportId">The departure airport identifier.</param>
        /// <param name="geographyLevel2Id">The geography level2 identifier.</param>
        /// <param name="searchDate">The date being searched.</param>
        /// <returns>The list of dates.</returns>
        public List<DateTime> GetDepartureDatesByRegion(int departureAirportId, int geographyLevel2Id)
        {
            var dates =
                this.flightCacheRouteRepository.GetAll()
                    .Where(route => route.DepartureAirportId == departureAirportId
                        && route.Destinations.Exists(destination => destination.GeographyLevel2Id == geographyLevel2Id))
                    .SelectMany(route => route.DepartureDates)
                    .Distinct()
                    .ToList();

            return dates;
        }

        /// <summary>
        /// Gets the departure dates by resort.
        /// </summary>
        /// <param name="departureAirportId">The departure airport identifier.</param>
        /// <param name="geographyLevel3Id">The geography level3 identifier.</param>
        /// <param name="searchDate">The date being searched.</param>
        /// <returns>The list of dates.</returns>
        public List<DateTime> GetDepartureDatesByResort(int departureAirportId, int geographyLevel3Id)
        {
            var dates =
                this.flightCacheRouteRepository.GetAll()
                    .Where(route => route.DepartureAirportId == departureAirportId
                        && route.Destinations.Exists(destination => destination.GeographyLevel3Id == geographyLevel3Id))
                    .SelectMany(route => route.DepartureDates)
                    .Distinct()
                    .ToList();

            return dates;
        }
    }
}
