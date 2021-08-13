namespace Web.Template.Data.Lookup.Repositories.Flight
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;

    /// <summary>
    /// Airport Repository that is responsible for managing access to airports
    /// </summary>
    /// <seealso cref="Web.Template.Data.Repositories.LookupRepository{Web.Template.Domain.Entities.Flight.AirportGroup}" />
    /// <seealso cref="IAirportGroupRepository" />
    /// <seealso cref="IAirportRepository" />
    public class AirportGroupRepository : CachedLookupRepository<AirportGroup>, IAirportGroupRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AirportGroupRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public AirportGroupRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}