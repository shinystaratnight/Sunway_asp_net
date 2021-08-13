namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Property
{
    using Web.Template.Domain.Entities.Property;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    ///  Lookup Repository of Filter Facilities.
    /// </summary>
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Generic.ILookupRepository{Web.Template.Domain.Entities.Property.FilterFacility}" />
    public interface IFilterFacilityRepository : ILookupRepository<FilterFacility>
    {
    }
}