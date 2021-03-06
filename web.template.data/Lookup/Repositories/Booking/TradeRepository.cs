namespace Web.Template.Data.Lookup.Repositories.Booking
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Booking;

    /// <summary>
    /// Cached Repository Exposing Trade Contacts
    /// </summary>
    /// <seealso cref="Trade" />
    /// <seealso cref="ITradeRepository" />
    public class TradeRepository : CachedLookupRepository<Trade>, ITradeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TradeRepository" /> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public TradeRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}