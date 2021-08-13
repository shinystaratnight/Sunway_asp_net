namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Geography
{
    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    ///     Interface for country repository, responsible for accessing country data.
    /// </summary>
    public interface ICountryRepository : ILookupRepository<Country>
    {
    }
}