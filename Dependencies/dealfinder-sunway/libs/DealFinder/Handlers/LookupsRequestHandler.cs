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

    public class LookupsRequestHandler : IRequestHandler<LookupsRequest, LookupsResponse>
    {
        private readonly ILookupsService _service;

        public LookupsRequestHandler(ILookupsService service)
        {
            _service = Ensure.IsNotNull(service, nameof(service));
        }

        public async Task<LookupsResponse> Handle(LookupsRequest request, CancellationToken cancellationToken)
        {
            Ensure.IsNotNull(request, nameof(request));

            var lookups = await _service.ProcessAsync(request.Lookups, cancellationToken);
            return new LookupsResponse
            {
                Lookups = lookups
            };
        }
    }
}
