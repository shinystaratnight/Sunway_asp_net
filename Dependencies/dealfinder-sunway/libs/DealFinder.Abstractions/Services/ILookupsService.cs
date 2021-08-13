namespace DealFinder.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using DealFinder.Response;

    public interface ILookupsService
    {
        Task<Lookup[]> ProcessAsync(IEnumerable<string> lookups, CancellationToken cancellationToken);
    }
}
