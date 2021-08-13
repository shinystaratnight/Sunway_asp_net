namespace DealFinder.Services
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DealFinder.Records;
    using DealFinder.Response;
    using Intuitive;
    using Intuitive.Data;

    public class SearchService : ISearchService
    {
        private readonly ISql _sql;
        private readonly IPricingService _pricingService;

        public SearchService(ISql sql, IPricingService pricingService)
        {
            _sql = Ensure.IsNotNull(sql, nameof(sql));
            _pricingService = Ensure.IsNotNull(pricingService, nameof(pricingService));
        }

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
            var settings = new CommandSettings()
                .IsStoredProcedure()
                .WithParameters(new
                {
                    sAirport = departureAirportIDs,
                    sG1 = geographyLevel1IDs,
                    sG2 = geographyLevel2IDs,
                    sG3 = geographyLevel3IDs,
                    sProperty = propertyReferenceIDs,
                    sTheme = productAttributeIDs,
                    sFacility = facilityIDs,
                    sMealBasis = mealBasisIDs,
                    sRating = ratings,
                    iAdults = adults,
                    iChildren = children,
                    dStart = startDate,
                    dEnd = endDate == DateTime.MinValue ? (DateTime?)null : endDate,
                    sDuration = durations,
                    iMinPrice = minPrice,
                    iMaxPrice = maxPrice == 0 ? 9999 : maxPrice,
                    iMinimumInterestingNess = minInterestness,
                    iNumberOfProperties = results
                })
                .WithCancellationToken(cancellationToken);

            var properties = await _sql.ReadAllAsync<PropertyRecord>("Pack_Search", settings);
            return properties.Select(record =>
            {
                decimal flightPrice = _pricingService.ApplyMargin(record.GetFlightPriceInfo(adults, children));
                decimal propertyPrice = _pricingService.ApplyMargin(record.GetPropertyPriceInfo(adults, children));

                return new Property(record.ArrivalAirportID, record.DepartureAirportID, record.DepartureDate,
                                    record.Duration, record.Interestingness, record.MealBasisID,
                                    record.PackageReference, flightPrice + propertyPrice, record.PropertyReferenceID);
            })
            .ToArray();
        }
    }
}
