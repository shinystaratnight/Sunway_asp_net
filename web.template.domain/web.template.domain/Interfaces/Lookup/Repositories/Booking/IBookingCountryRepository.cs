namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Booking
{
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    /// Lookup Repository of Booking Country
    /// </summary>
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Generic.ILookupRepository{Web.Template.Domain.Entities.Booking.BookingCountry}" />
    public interface IBookingCountryRepository : ILookupRepository<BookingCountry>
    {
    }
}