namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Geography
{
    using Web.Template.Domain.Entities.Geography;

    /// <summary>
    /// Repository used to retrieve geography groups
    /// </summary>
    public interface IGeographyGroupingRepository
    {
        /// <summary>
        ///     Gets the geography by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A geography grouping</returns>
        GeographyGrouping GetGeographyGroupById(int id);
    }
}