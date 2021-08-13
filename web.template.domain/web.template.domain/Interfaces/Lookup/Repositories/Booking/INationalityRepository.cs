namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Booking
{
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    ///  Lookup Repository of Nationality.
    /// </summary>
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Generic.ILookupRepository{Web.Template.Domain.Entities.Booking.Nationality}" />
    public interface INationalityRepository : ILookupRepository<Nationality>
    {
    }
}