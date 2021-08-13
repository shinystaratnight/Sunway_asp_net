namespace DealFinder.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DealFinder.Records;
    using DealFinder.Response;
    using Intuitive;
    using Intuitive.Data;

    public class CalendarService : ICalendarService
    {
        private readonly ISql _sql;
        private readonly IPricingService _pricingService;

        public CalendarService(ISql sql, IPricingService pricingService)
        {
            _sql = Ensure.IsNotNull(sql, nameof(sql));
            _pricingService = Ensure.IsNotNull(pricingService, nameof(pricingService));
        }

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
            var settings = new CommandSettings()
                .IsStoredProcedure()
                .WithParameters(new
                {
                    iPropertyReferenceID = propertyReferenceID,
                    iDepartureAirportID = departureAirportID,
                    dStart = startDate,
                    dEnd = endDate,
                    iDuration = duration,
                    iAdults = adults,
                    iChildren = children,
                    iMealBasisID = mealBasisID
                })
                .WithCancellationToken(cancellationToken);

            var dates = await _sql.ReadAllAsync<PackageCostsByDateRecord>("Pack_Calendar", settings);

            return dates.Select(record =>
                {
                    var propertyPrice = _pricingService.ApplyMargin(record.GetPropertyPriceInfo(adults, children));
                    var flightPrice = _pricingService.ApplyMargin(record.GetFlightPriceInfo(adults, children));
                    return new PackagePriceByDate(record.PackageReference, record.DepartureDate, propertyPrice + flightPrice);
                })
                .ToArray();
        }
    }
}
