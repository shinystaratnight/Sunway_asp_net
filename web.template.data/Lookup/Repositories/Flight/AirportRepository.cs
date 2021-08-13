namespace Web.Template.Data.Lookup.Repositories.Flight
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;

    /// <summary>
    ///     Airport Repository that is responsible for managing access to airports
    /// </summary>
    /// <seealso cref="IAirportRepository" />
    public class AirportRepository : CachedLookupRepository<Airport>, IAirportRepository
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AirportRepository" /> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public AirportRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}