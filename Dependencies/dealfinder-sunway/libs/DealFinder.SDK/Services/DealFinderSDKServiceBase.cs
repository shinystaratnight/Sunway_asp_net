namespace DealFinder.Services
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Web.Api;
    using Newtonsoft.Json;

    public abstract class DealFinderSDKServiceBase<T> where T : class
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public DealFinderSDKServiceBase(HttpClient httpClient, string baseUrl)
        {
            _baseUrl = Ensure.IsNotNull(baseUrl, nameof(baseUrl));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
        }

        protected abstract string RelativeUrl { get; }
        private string Url => _baseUrl + RelativeUrl;

        protected Task<U> PostJsonAsync<U>(string json, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Url);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return SendAsync<U>(request, cancellationToken);
        }

        protected Task<U> GetAsync<U>(string query, CancellationToken cancellationToken)
        {
            var builder = new UriBuilder(Url);
            builder.Query = query;

            var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);

            return SendAsync<U>(request, cancellationToken);
        }

        protected async Task<U> SendAsync<U>(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseJson = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<U>(responseJson);
            }

            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseJson);
            var error = errorResponse.Errors[0];
            throw new InvalidOperationException(error.Title);
        }
    }
}
