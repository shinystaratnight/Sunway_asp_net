namespace DealFinder.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using DealFinder.Request;
    using DealFinder.Response;
    using DealFinder.Services;
    using Intuitive;
    using MediatR;

    public class FlightCacheSearchHandler : IRequestHandler<FlightCacheSearchRequest, FlightCacheSearchResponse>
    {
        private readonly IFlightCacheSearchService _service;

        public FlightCacheSearchHandler(IFlightCacheSearchService service)
        {
            _service = Ensure.IsNotNull(service, nameof(service));
        }

        public async Task<FlightCacheSearchResponse> Handle(FlightCacheSearchRequest request, CancellationToken cancellationToken)
        {
            Ensure.IsNotNull(request, nameof(request));

            var dates = await _service.ProcessAsync(request.DepartureAirports, request.ArrivalAirports, request.Month, cancellationToken);
            return new FlightCacheSearchResponse(dates);
        }
    }
}
