namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Flight
{
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    /// Flight Carrier Repository interface, defining what the repository that manages flight carriers must do.
    /// </summary>
    public interface IFlightCarrierRepository : ILookupRepository<FlightCarrier>
    {
    }
}