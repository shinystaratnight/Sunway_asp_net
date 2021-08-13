namespace DealFinder.Handlers
{
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;
    using DealFinder.Request;
    using DealFinder.Response;
    using DealFinder.Services;
    using Intuitive;
    using MediatR;

    public class SearchRequestHandler : IRequestHandler<SearchRequest, SearchResponse>
    {
        private readonly ISearchService _service;

        public SearchRequestHandler(ISearchService service)
        {
            _service = Ensure.IsNotNull(service, nameof(service));
        }

        public async Task<SearchResponse> Handle(SearchRequest request, CancellationToken cancellationToken)
        {
            Ensure.IsNotNull(request, nameof(request));

            var properties = await _service.ProcessAsync(request.DepartureAirportIDs, request.GeographyLevel1IDs,
                                                            request.GeographyLevel2IDs, request.GeographyLevel3IDs,
                                                            request.PropertyReferenceIDs, request.ProductAttributeIDs,
                                                            request.FacilityIDs, request.MealBasisIDs, request.Ratings,
                                                            request.Adults, request.Children, request.StartDate,
                                                            request.EndDate, request.Durations, request.MinPrice,
                                                            request.MaxPrice, request.MinInterestness, request.Results,
                                                            cancellationToken);
            return new SearchResponse
            {
                Properties = properties
            };
        }
    }
}
