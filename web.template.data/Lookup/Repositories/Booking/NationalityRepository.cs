namespace Web.Template.Data.Lookup.Repositories.Booking
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;

    /// <summary>
    /// Cached Repository Exposing Nationality.
    /// </summary>
    /// <seealso cref="Web.Template.Data.Lookup.Repositories.Generic.CachedLookupRepository{Web.Template.Domain.Entities.Booking.Nationality}" />
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Booking.INationalityRepository" />
    public class NationalityRepository : CachedLookupRepository<Nationality>, INationalityRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NationalityRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public NationalityRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}