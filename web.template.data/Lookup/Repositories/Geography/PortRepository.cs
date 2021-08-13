namespace Web.Template.Data.Lookup.Repositories.Geography
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Geography;

    /// <summary>
    /// Port Repository that is responsible for managing access to ports
    /// </summary>
    /// <seealso cref="Web.Template.Data.Repositories.LookupRepository{Web.Template.Domain.Entities.Geography.Port}" />
    /// <seealso cref="IPortRepository" />
    public class PortRepository : CachedLookupRepository<Port>, IPortRepository
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PortRepository" /> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public PortRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}