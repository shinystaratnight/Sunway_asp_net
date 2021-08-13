namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Flight
{
    using System.Collections.Generic;
    using Web.Template.Domain.Entities.Flight;

    /// <summary>
    /// Interface IFlightCacheRouteRepository
    /// </summary>
    public interface IFlightCacheRouteRepository
    {
        IEnumerable<FlightCacheRoute> GetAll();
    }
}