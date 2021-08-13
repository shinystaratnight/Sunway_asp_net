namespace DealFinder.Services
{
    using System.Threading.Tasks;
    using DealFinder.Response;
    using System.Threading;
    using Intuitive;
    using Intuitive.Data;
    using System.Collections.Generic;
    using System.Linq;

    public class LookupsService : ILookupsService
    {
        private readonly ISql _sql;

        public LookupsService(ISql sql)
        {
            _sql = Ensure.IsNotNull(sql, nameof(sql));
        }

        public async Task<Lookup[]> ProcessAsync(IEnumerable<string> lookups, CancellationToken cancellationToken)
        {
            string query = string.Format("select * from Pack_Lookup where LookupGroup in ({0})",
                                            string.Join(",", lookups.Select(lookup => "'" + lookup + "'")));
            var settings = new CommandSettings()
                .WithCancellationToken(cancellationToken);

            return await _sql.ReadAllAsync<Lookup>(query, settings);
        }
    }
}
