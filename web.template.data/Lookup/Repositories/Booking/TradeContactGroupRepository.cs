namespace Web.Template.Data.Lookup.Repositories.Booking
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Booking;

    /// <summary>
    /// Cached Repository Exposing Trade Contacts
    /// </summary>
    /// <seealso cref="TradeContact" />
    /// <seealso cref="ITradeContactRepository" />
    public class TradeContactGroupRepository : CachedLookupRepository<TradeContactGroup>, ITradeContactGroupRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TradeContactGroupRepository" /> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public TradeContactGroupRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}