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
    public class PropertyReferenceRepository : CachedLookupRepository<PropertyReference>, IPropertyReferenceRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyReferenceRepository" /> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public PropertyReferenceRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}