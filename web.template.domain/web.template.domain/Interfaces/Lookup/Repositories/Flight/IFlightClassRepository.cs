namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Flight
{
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    /// Interface for flight classes, responsible for accessing flight classes.
    /// </summary>
    public interface IFlightClassRepository : ILookupRepository<FlightClass>
    {
    }
}