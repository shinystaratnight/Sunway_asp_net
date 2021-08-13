namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Flight
{
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    /// Interface for route availability repository, responsible for accessing routes.
    /// </summary>
    /// <seealso cref="RouteAvailability" />
    public interface IRouteAvailabilityRepository : ILookupRepository<RouteAvailability>
    {
    }
}