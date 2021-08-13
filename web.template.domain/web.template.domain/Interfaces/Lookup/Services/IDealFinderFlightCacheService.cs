namespace Web.Template.Domain.Interfaces.Lookup.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DealFinder.Response;

    public interface IDealFinderFlightCacheService
    {
        Task<DepartureDateAndDurations[]> GetCacheDatesAsync(int departureAirportId, List<int> arrivalAirportIds, string month);
    }
}
