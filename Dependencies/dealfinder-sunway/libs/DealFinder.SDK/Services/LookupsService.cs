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
    /// A service for finding details on various lookup types used by the DealFinder API
    /// </summary>
    public class LookupsService : DealFinderSDKServiceBase<Lookup>, ILookupsService
    {
        /// <summary>
        /// Initialises a new instance of <see cref="LookupsService"/>
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use</param>
        /// <param name="baseUrl">The URL of the DealFinder API</param>
        public LookupsService(HttpClient httpClient, string baseUrl) : base(httpClient, baseUrl) { }

        protected override string RelativeUrl => "/lookups";

        /// <summary>
        /// Sends a LookupsRequest to the DealFinder API
        /// </summary>
        /// <param name="lookups">A collection of lookup strings to search for</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        public async Task<Lookup[]> ProcessAsync(IEnumerable<string> lookups, CancellationToken cancellationToken)
        {
            string query = $"lookuptypes={string.Join(",", lookups)}";

            var result = await GetAsync<SetResponse<Lookup>>(query, cancellationToken);

            return result.Data;
        }
    }
}
