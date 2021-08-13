namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Booking
{
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    /// Lookup repository for brand geographies
    /// </summary>
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Generic.ILookupRepository{Web.Template.Domain.Entities.Booking.BrandGeography}" />
    public interface IBrandGeographyRepository : ILookupRepository<BrandGeography>
    {
    }
}