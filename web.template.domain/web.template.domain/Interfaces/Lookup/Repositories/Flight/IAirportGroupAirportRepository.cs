namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Flight
{
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    ///     Interface for airport repository, responsible for accessing airports.
    /// </summary>
    public interface IAirportGroupAirportRepository : ILookupRepository<AirportGroupAirport>
    {
    }
}