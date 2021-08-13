namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Flight
{
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    ///     Interface for vehicle repository, responsible for accessing vehicles.
    /// </summary>
    public interface IVehicleRepository : ILookupRepository<Vehicle>
    {
    }
}