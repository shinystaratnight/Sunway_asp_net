namespace Web.Template.Data.Lookup.Repositories.Booking
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;

    /// <summary>
    /// Cached Repository Exposing Sales Channel.
    /// </summary>
    /// <seealso cref="Web.Template.Data.Lookup.Repositories.Generic.CachedLookupRepository{Web.Template.Domain.Entities.Booking.SalesChannel}" />
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Booking.ISalesChannelRepository" />
    public class SalesChannelRepository : CachedLookupRepository<SalesChannel>, ISalesChannelRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SalesChannelRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public SalesChannelRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}