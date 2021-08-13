namespace Web.Template.Data.Lookup.Repositories.Flight
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;

    /// <summary>
    ///     Vehicle Repository that is responsible for managing access to vehicles
    /// </summary>
    /// <seealso cref="IVehicleRepository" />
    public class VehicleRepository : CachedLookupRepository<Vehicle>, IVehicleRepository
    {
        /// <summary>
        ///     The database context.
        /// </summary>
        private readonly DbContext dbcontext;

        /// <summary>
        ///     Initializes a new instance of the <see cref="VehicleRepository" /> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public VehicleRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}