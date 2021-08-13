namespace Web.Template.Application.Interfaces.Lookup.Services
{
    using System.Collections.Generic;

    using Web.Template.Domain.Entities.Extras;

    /// <summary>
    /// Extra service responsible for access to extra information
    /// </summary>
    public interface IExtraService
    {
        /// <summary>
        /// Gets the extra type by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The ExtraType.</returns>
        ExtraType GetExtraTypeById(int id);

        /// <summary>
        /// Gets the extra types.
        /// </summary>
        /// <returns>List of extra types.</returns>
        List<ExtraType> GetExtraTypes();
    }
}
