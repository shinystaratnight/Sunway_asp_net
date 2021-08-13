namespace Web.Template.Data.Lookup.Repositories.Flight
{
    using System.Collections.Generic;
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;

    /// <summary>
    /// Route availability repository
    /// </summary>
    /// <seealso cref="Web.Template.Data.Lookup.Repositories.Generic.CachedLookupRepository{Web.Template.Domain.Entities.Flight.RouteAvailability}" />
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Flight.IRouteAvailabilityRepository" />
    public class RouteAvailabilityRepository : CachedLookupRepository<RouteAvailability>, IRouteAvailabilityRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteAvailabilityRepository" /> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public RouteAvailabilityRepository(DbContext dbContext)
            : base(dbContext)
        {
        }

        /// <summary>
        /// Gets the routes with airports.
        /// </summary>
        /// <returns>
        /// A list of all route availability with their airports populated
        /// </returns>
        public IEnumerable<RouteAvailability> GetRoutesWithAirports()
        {
            return
                this.GetAll(
                    route =>
                    route.Include(r => r.DepartureAirport)
                        .Include(r => r.AirportGroup)
                        .Include(r => r.ArrivalAirport.Resorts));
        }

        /// <summary>
        /// Gets the route with airports.
        /// </summary>
        /// <param name="routeId">The route identifier.</param>
        /// <returns>
        /// the route with airports
        /// </returns>
        public RouteAvailability GetRouteWithAirports(int routeId)
        {
            return this.GetSingle(
                routeId, 
                route =>
                route.Include(r => r.DepartureAirport)
                    .Include(r => r.AirportGroup)
                    .Include(r => r.ArrivalAirport.Resorts));
        }
    }
}