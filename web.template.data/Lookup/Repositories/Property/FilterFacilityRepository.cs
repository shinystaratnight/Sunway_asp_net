namespace Web.Template.Data.Lookup.Repositories.Property
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Property;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Property;

    /// <summary>
    /// Cached Repository Exposing Filter Facilities.
    /// </summary>
    /// <seealso cref="Web.Template.Data.Lookup.Repositories.Generic.CachedLookupRepository{Web.Template.Domain.Entities.Property.FilterFacility}" />
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Property.IFilterFacilityRepository" />
    public class FilterFacilityRepository : CachedLookupRepository<FilterFacility>, IFilterFacilityRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterFacilityRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public FilterFacilityRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}