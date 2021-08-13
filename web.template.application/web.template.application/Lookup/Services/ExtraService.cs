namespace Web.Template.Application.Lookup.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Domain.Entities.Extras;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Extras;

    /// <summary>
    /// Extra service responsible for access to extra information
    /// </summary>
    public class ExtraService : IExtraService
    {
        /// <summary>
        /// The extra type repository
        /// </summary>
        private readonly IExtraTypeRepository extraTypeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtraService"/> class.
        /// </summary>
        /// <param name="extraTypeRepository">The extra type repository.</param>
        public ExtraService(IExtraTypeRepository extraTypeRepository)
        {
            this.extraTypeRepository = extraTypeRepository;
        }

        /// <summary>
        /// Gets the extra type by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The ExtraType.</returns>
        public ExtraType GetExtraTypeById(int id)
        {
            return this.extraTypeRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the extra types.
        /// </summary>
        /// <returns>List of extra types.</returns>
        public List<ExtraType> GetExtraTypes()
        {
            return this.extraTypeRepository.GetAll().ToList();
        }
    }
}
