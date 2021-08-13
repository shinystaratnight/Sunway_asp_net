namespace DealFinder.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using DealFinder.Request;
    using DealFinder.Response;
    using Intuitive.Web.Api;
    using Newtonsoft.Json;

    /// <summary>
    /// A service for finding flight cache data by routes for a given month using the DealFinder API
    /// </summary>
    public class FlightCacheSearchService : DealFinderSDKServiceBase<DepartureDateAndDurations>, IFlightCacheSearchService
    {
        /// <summary>
        /// Initialises a new instance of <see cref="FlightCacheSearchService"/>
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use</param>
        /// <param name="baseUrl">The URL of the DealFinder API</param>
        public FlightCacheSearchService(HttpClient httpClient, string baseUrl) : base(httpClient, baseUrl) { }

        protected override string RelativeUrl => "/api/flightcachesearch";

        /// <summary>
        /// Finds the flight cache data for all specified routes in the specified month.
        /// </summary>
        /// <param name="departureAirportIDs">The departure airport ids</param>
        /// <param name="arrivalAirportIDs">The arrival airport ids</param>
        /// <param name="months">A list of MM/YYYY strings as the months to search</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A Task </returns>
        public async Task<DepartureDateAndDurations[]> ProcessAsync(
            IEnumerable<int> departureAirportIDs,
            IEnumerable<int> arrivalAirportIDs,
            IEnumerable<string> months,
            CancellationToken cancellationToken)
        {
            var searchRequest = new FlightCacheSearchRequest
            {
                DepartureAirports = departureAirportIDs.ToList(),
                ArrivalAirports = arrivalAirportIDs.ToList(),
                Months = months.ToList()
            };

            return (await PostJsonAsync<SetResponse<DepartureDateAndDurations>>(
                JsonConvert.SerializeObject(searchRequest), cancellationToken)).Data;
        }
    }
}