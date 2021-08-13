namespace DealFinder.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using DealFinder.Response;

    public interface ISearchService
    {
        Task<Property[]> ProcessAsync(
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
            CancellationToken cancellationToken);
    }
}
