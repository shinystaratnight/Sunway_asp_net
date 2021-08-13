namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Geography
{
    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    ///     Interface for region repository, responsible for accessing region data.
    /// </summary>
    public interface IRegionRepository : ILookupRepository<Region>
    {
    }
}