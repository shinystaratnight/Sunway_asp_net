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

    public class CalendarRequestHandler : IRequestHandler<CalendarRequest, CalendarResponse>
    {
        private readonly ICalendarService _service;

        public CalendarRequestHandler(ICalendarService service)
        {
            _service = Ensure.IsNotNull(service, nameof(service));
        }

        public async Task<CalendarResponse> Handle(CalendarRequest request, CancellationToken cancellationToken)
        {
            Ensure.IsNotNull(request, nameof(request));

            var dates = await _service.ProcessAsync(request.PropertyReferenceID, request.DepartureAirportID, request.StartDate, 
                                                    request.EndDate, request.Duration, request.Adults, request.Children, 
                                                    request.MealBasisID, cancellationToken);

            return new CalendarResponse
            {
                Dates = dates
            };
        }
    }
}
