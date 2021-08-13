namespace Web.Template.Data.Lookup.Repositories.Geography
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Geography;

    /// <summary>
    /// Repository responsible for accessing relating relating to resorts.
    /// </summary>
    /// <seealso cref="Web.Template.Data.Repositories.LookupRepository{Web.Template.Domain.Entities.Geography.Region}" />
    /// <seealso cref="IRegionRepository" />
    public class ResortRepository : CachedLookupRepository<Resort>, IResortRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResortRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public ResortRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}