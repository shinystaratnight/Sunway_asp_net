namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Geography
{
    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    ///     Interface for port repository, responsible for accessing ports.
    /// </summary>
    public interface IPortRepository : ILookupRepository<Port>
    {
    }
}