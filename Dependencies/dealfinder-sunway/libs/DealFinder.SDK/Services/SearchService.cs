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
    /// A service for performing property searches using the DealFinder API
    /// </summary>
    public class SearchService : DealFinderSDKServiceBase<Property>, ISearchService
    {
        /// <summary>
        /// Initialises a new instance of <see cref="SearchService"/>
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use</param>
        /// <param name="baseUrl">The URL of the DealFinder API</param>
        public SearchService(HttpClient httpClient, string baseUrl) : base(httpClient, baseUrl) { }

        protected override string RelativeUrl => "/search";

        /// <summary>
        /// Sends a SearchRequest to the DealFinder API.
        /// </summary>
        /// <param name="departureAirportIDs">A CSV string of departure airport ids to search</param>
        /// <param name="geographyLevel1IDs">A CSV string of geography level 1 ids to search</param>
        /// <param name="geographyLevel2IDs">A CSV string of geography level 2 ids to search</param>
        /// <param name="geographyLevel3IDs">A CSV string of geography level 3 ids to search</param>
        /// <param name="propertyReferenceIDs">A CSV string of property reference ids to search</param>
        /// <param name="productAttributeIDs">A CSV string of product attribute ids to search</param>
        /// <param name="facilityIDs">A CSV of facility ids to search</param>
        /// <param name="mealBasisIDs">A CSV of meal basis ids to search</param>
        /// <param name="ratings">A CSV of ratings to search</param>
        /// <param name="adults">The number of adults to search</param>
        /// <param name="children">The number of children to search</param>
        /// <param name="startDate">The start date for the search</param>
        /// <param name="endDate">The end date for the search</param>
        /// <param name="durations">A CSV string of the durations to search</param>
        /// <param name="minPrice">The minimum price to search</param>
        /// <param name="maxPrice">The maximum price to search</param>
        /// <param name="minInterestness">The minimum interestingness to search</param>
        /// <param name="results">The number of results</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        public async Task<Property[]> ProcessAsync(
            string departureAirportIDs,
            string geographyLevel1IDs,
            string geographyLevel2IDs,
            string geographyLevel3IDs,
            string propertyReferenceIDs, 
            string productAttributeIDs,
            string facilityIDs, 
            string mealBasisIDs,
            string ratings, 
            int adults,
            int children,
            DateTime startDate,
            DateTime endDate,
            string durations, 
            decimal minPrice, 
            decimal maxPrice, 
            int minInterestness, 
            int results, 
            CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["airports"] = departureAirportIDs,
                ["g1s"] = geographyLevel1IDs,
                ["g2s"] = geographyLevel2IDs,
                ["g3s"] = geographyLevel3IDs,
                ["properties"] = propertyReferenceIDs,
                ["attributes"] = productAttributeIDs,
                ["facilities"] = facilityIDs,
                ["mealbases"] = mealBasisIDs,
                ["rating"] = ratings,
                ["adults"] = adults.ToString(),
                ["children"] = (children <= 0 ? null : children.ToString()),
                ["start"] = startDate.ToString("yyyy-MM-dd"),
                ["end"] = (endDate == DateTime.MinValue ? null : endDate.ToString("yyyy-MM-dd")),
                ["durations"] = durations,
                ["minprice"] = (minPrice == 0 ? null : minPrice.ToString()),
                ["maxprice"] = (maxPrice == 0 ? null : maxPrice.ToString()),
                ["mininterestness"] = (minInterestness == 0 ? null : minInterestness.ToString()),
                ["results"] = (results <= 0 ? null : results.ToString())
            };

            string query = string.Join(
                "&",
                parameters.Where(p => p.Value is object).Select(p => $"{p.Key}={p.Value}"));

            return (await GetAsync<SetResponse<Property>>(query, cancellationToken)).Data;
        }
    }
}
