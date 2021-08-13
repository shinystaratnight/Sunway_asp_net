namespace DealFinder.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DealFinder.Records;
    using DealFinder.Response;
    using Intuitive;
    using Intuitive.Data;

    public class FlightCacheSearchService : IFlightCacheSearchService
    {
        private readonly ISql _sql;

        public FlightCacheSearchService(ISql sql)
        {
            _sql = Ensure.IsNotNull(sql, nameof(sql));
        }

        public async Task<DepartureDateAndDurations[]> ProcessAsync(
            IEnumerable<int> departureAirportIds, 
            IEnumerable<int> arrivalAirportIds, 
            string month, 
            CancellationToken cancellationToken)
        {
            var settings = new CommandSettings()
                .IsStoredProcedure()
                .WithParameters(new
                {
                    sDepartureAirportIDs = string.Join(",", departureAirportIds),
                    sArrivalAirportIDs = string.Join(",", arrivalAirportIds),
                    sMonth = month
                })
                .WithCancellationToken(cancellationToken);

            var records = await _sql.ReadAllAsync<DepartureDateAndDurationsRecord>("Pack_FlightCache", settings);
            return records.Select(record => new DepartureDateAndDurations(record.DepartureDate, record.DurationsCSV)).ToArray();
        }
    }
}
