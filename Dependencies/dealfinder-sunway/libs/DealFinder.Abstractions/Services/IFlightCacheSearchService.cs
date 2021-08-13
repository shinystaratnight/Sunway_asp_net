using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DealFinder.Response;

namespace DealFinder.Services
{
    public interface IFlightCacheSearchService
    {
        Task<DepartureDateAndDurations[]> ProcessAsync(
            IEnumerable<int> departureAirportIDs, 
            IEnumerable<int> arrivalAirportIDs,
            IEnumerable<string> months,
            CancellationToken cancellationToken);
    }
}
