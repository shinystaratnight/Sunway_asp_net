namespace Web.Template.Data.Lookup.Repositories.Flight
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;

    /// <summary>
    /// Flight carrier repository, responsible for managing access to flight carriers
    /// </summary>
    /// <seealso cref="IFlightCarrierRepository" />
    public class FlightCarrierRepository : LookupRepository<FlightCarrier>, IFlightCarrierRepository
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly DbContext dbcontext;

        /// <summary>
        /// The database set
        /// </summary>
        private readonly IDbSet<FlightCarrier> dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightCarrierRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public FlightCarrierRepository(DbContext dbContext)
            : base(dbContext)
        {
            this.dbcontext = dbContext;
            this.dbSet = this.dbcontext.Set<FlightCarrier>();
        }
    }
}