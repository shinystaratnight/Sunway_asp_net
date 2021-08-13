namespace Web.Template.Application.Lookup.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DealFinder.Response;
    using DealFinder.Services;
    using Intuitive;
    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;

    public class DealFinderFlightCacheRouteService : IAsyncFlightCacheRouteService
    {
        private readonly IAirportRepository airportRepository;
        private readonly IFlightCacheSearchService dealFinder;
        private readonly IFlightCacheRouteRepository flightCacheRouteRepository;

        private readonly int[] allDurations;

        public DealFinderFlightCacheRouteService(
            IAirportRepository airportRepository,
            IFlightCacheSearchService dealFinder,
            IFlightCacheRouteRepository flightCacheRouteRepository)
        {
            this.airportRepository = airportRepository;
            this.dealFinder = dealFinder;
            this.flightCacheRouteRepository = flightCacheRouteRepository;
            this.allDurations = Enumerable.Range(1, 30).ToArray();
        }

        public async Task<FlightCacheRouteDate[]> GetDepartureDatesByAirportAsync(
            int departureAirportId,
            int arrivalAirportId,
            DateTime? searchDate,
            CancellationToken cancellationToken)
        {
            List<int> arrivalAirportIds = new List<int> { arrivalAirportId };
            return await GetDepartureDatesAsync(departureAirportId, arrivalAirportIds, searchDate, cancellationToken);

        }

        public async Task<FlightCacheRouteDate[]> GetDepartureDatesByRegionAsync(
            int departureAirportId,
            int geographyLevel2Id,
            DateTime? searchDate,
            CancellationToken cancellationToken)
        {
            List<int> arrivalAirportIds = GetArrivalAirportIDsByRegion(geographyLevel2Id);
            return await GetDepartureDatesAsync(departureAirportId, arrivalAirportIds, searchDate, cancellationToken);
        }

        public async Task<FlightCacheRouteDate[]> GetDepartureDatesByResortAsync(
            int departureAirportId,
            int geographyLevel3Id,
            DateTime? searchDate,
            CancellationToken cancellationToken)
        {
            List<int> arrivalAirportIds = GetArrivalAirportIDsByResort(geographyLevel3Id);
            return await GetDepartureDatesAsync(departureAirportId, arrivalAirportIds, searchDate, cancellationToken);
        }

        private async Task<FlightCacheRouteDate[]> GetDepartureDatesAsync(
            int departureAirportId,
            List<int> arrivalAirportIds,
            DateTime? startDate,
            CancellationToken cancellationToken)
        {
            if (!(startDate is null))
            {
                DateTime search = startDate ?? DateTime.MinValue;
                DateTime month = new DateTime(search.Year, search.Month, 1);
                DateTime end = month.AddMonths(19);
                DateTime[] allDates = Enumerable.Range(0, end.Subtract(month).Days)
                    .Select(i => month.AddDays(i))
                    .ToArray();

                const int advanceSearchMonths = 16;
                var searchMonths =
                    BuildMonths(month, advanceSearchMonths)
                    .Select(d => d.ToString("MM/yyyy"))
                    .ToList();

                var dealFinderResponse = await dealFinder.ProcessAsync(
                    new[] { departureAirportId },
                    arrivalAirportIds,
                    searchMonths,
                    cancellationToken);

                var response = this.flightCacheRouteRepository
                    .GetAll()
                    .Where(route => route.DepartureAirportId == departureAirportId
                        && arrivalAirportIds.Contains(route.ArrivalAirportId))
                    .SelectMany(route => route.DepartureDatesAndDurations)
                    .Where(date => date.DepartureDate >= search && date.DepartureDate < end)
                    .GroupBy(date => date.DepartureDate)
                    .Select(date => new FlightCacheRouteDate(date.Key, date.Select(x => x.Duration).ToArray(), true))
                    .ToDictionary(cacheDate => cacheDate.DepartureDate, cacheDate => cacheDate);

                foreach (var dfDate in dealFinderResponse.Where(date => date.Durations.Any()))
                    if (!response.ContainsKey(dfDate.DepartureDate))
                    {
                        response.Add(dfDate.DepartureDate, new FlightCacheRouteDate(dfDate.DepartureDate, dfDate.Durations, false));
                    }
                    else
                    {
                        response[dfDate.DepartureDate].Durations = response[dfDate.DepartureDate].Durations.Union(dfDate.Durations).ToArray();
                    }

                foreach (var date in allDates)
                    if (!response.ContainsKey(date))
                        response.Add(date, new FlightCacheRouteDate(date, new int[0], true));

                return response
                    .OrderBy(kvp => kvp.Key)
                    .Select(kvp => kvp.Value)
                    .ToArray();
            }

            return new FlightCacheRouteDate[0];
        }

        private static IEnumerable<DateTime> BuildMonths(DateTime start, int advanceMonths)
        {
            for (var monthCount = 0; monthCount < advanceMonths; monthCount++)
                yield return start.AddMonths(monthCount);
        }

        private List<int> GetArrivalAirportIDsByRegion(int geographyLevel2Id)
        {
            return airportRepository
                .FindBy(airport => airport.Resorts.Any(resort => resort.RegionID == geographyLevel2Id))
                .Select(airport => airport.Id)
                .ToList();
        }

        private List<int> GetArrivalAirportIDsByResort(int geographyLevel3Id)
        {
            return airportRepository
                .FindBy(airport => airport.Resorts.Any(resort => resort.Id == geographyLevel3Id))
                .Select(airport => airport.Id)
                .ToList();
        }
    }
}