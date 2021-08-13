namespace Web.Template.Data.Lookup.Repositories.Booking
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;

    /// <summary>
    /// Cached Repository Exposing Booking Documentation.
    /// </summary>
    /// <seealso cref="Web.Template.Data.Lookup.Repositories.Generic.CachedLookupRepository{Web.Template.Domain.Entities.Booking.BookingDocumentation}" />
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Booking.IBookingDocumentationRepository" />
    public class BookingDocumentationRepository : CachedLookupRepository<BookingDocumentation>, 
                                                  IBookingDocumentationRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BookingDocumentationRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public BookingDocumentationRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}