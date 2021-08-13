namespace DealFinder.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using DealFinder.Response;

    public interface ICalendarService
    {
        Task<PackagePriceByDate[]> ProcessAsync(
            int propertyReferenceID,
            int departureAirportID,
            DateTime startDate,
            DateTime endDate,
            int duration,
            int adults,
            int children,
            int mealBasisID,
            CancellationToken cancellationToken);
    }
}
