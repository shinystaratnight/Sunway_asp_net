namespace Web.Template.Data.Lookup.Repositories.Booking
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;

    /// <summary>
    /// Cached Repository Exposing Language.
    /// </summary>
    /// <seealso cref="Web.Template.Data.Lookup.Repositories.Generic.CachedLookupRepository{Web.Template.Domain.Entities.Booking.Language}" />
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Booking.ILanguageRepository" />
    public class LanguageRepository : CachedLookupRepository<Language>, ILanguageRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public LanguageRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}