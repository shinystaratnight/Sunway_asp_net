namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Booking
{
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    ///  Lookup Repository of Sales Channel.
    /// </summary>
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Generic.ILookupRepository{Web.Template.Domain.Entities.Booking.SalesChannel}" />
    public interface ISalesChannelRepository : ILookupRepository<SalesChannel>
    {
    }
}