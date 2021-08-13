namespace DealFinder.Services
{
    using System;
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
    /// A service for finding the package prices for specified dates using the DealFinder API
    /// </summary>
    public class CalendarService : DealFinderSDKServiceBase<PackagePriceByDate>, ICalendarService
    {
        /// <summary>
        /// Initialises a new instance of <see cref="CalendarService"/>
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use</param>
        /// <param name="baseUrl">The URL of the DealFinder API</param>
        public CalendarService(HttpClient httpClient, string baseUrl) : base(httpClient, baseUrl) { }

        protected override string RelativeUrl => "/calendar";

        /// <summary>
        /// Sends a CalendarRequest to the DealFinder API
        /// </summary>
        /// <param name="propertyReferenceID">The property reference id to search for</param>
        /// <param name="departureAirportID">The departure airport id to search for</param>
        /// <param name="startDate">The start date of the calendar</param>
        /// <param name="endDate">The end date of the calendar</param>
        /// <param name="duration">The duration to search for</param>
        /// <param name="adults">The number of adults to search for</param>
        /// <param name="children">The number of children to search for</param>
        /// <param name="mealBasisID">The meal basis id to search for</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        public async Task<PackagePriceByDate[]> ProcessAsync(
            int propertyReferenceID,
            int departureAirportID,
            DateTime startDate,
            DateTime endDate,
            int duration,
            int adults,
            int children,
            int mealBasisID,
            CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["airport"] = departureAirportID.ToString(),
                ["property"] = propertyReferenceID.ToString(),
                ["adults"] = adults.ToString(),
                ["children"] = (children <= 0 ? null : children.ToString()),
                ["start"] = startDate.ToString("yyyy-MM-dd"),
                ["end"] = endDate.ToString("yyyy-MM-dd"),
                ["duration"] = duration.ToString(),
                ["mealbasis"] = mealBasisID.ToString()
            };

            string query = string.Join(
                "&",
                parameters.Where(p => p.Value is object).Select(p => $"{p.Key}={p.Value}"));

            return (await GetAsync<SetResponse<PackagePriceByDate>>(query, cancellationToken)).Data;
        }
    }
}
