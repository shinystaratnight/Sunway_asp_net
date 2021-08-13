namespace Web.Template.Data.Lookup.Repositories.Booking
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;

    /// <summary>
    /// Cached Repository Exposing Booking Country
    /// </summary>
    /// <seealso cref="Web.Template.Data.Lookup.Repositories.Generic.CachedLookupRepository{Web.Template.Domain.Entities.Booking.BookingCountry}" />
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Booking.IBookingCountryRepository" />
    public class BookingCountryRepository : CachedLookupRepository<BookingCountry>, IBookingCountryRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BookingCountryRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public BookingCountryRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}