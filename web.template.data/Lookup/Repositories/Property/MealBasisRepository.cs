namespace Web.Template.Data.Lookup.Repositories.Property
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Property;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Property;

    /// <summary>
    /// Cached Repository Exposing Meal Basis.
    /// </summary>
    /// <seealso cref="Web.Template.Data.Lookup.Repositories.Generic.CachedLookupRepository{Web.Template.Domain.Entities.Property.MealBasis}" />
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Property.IMealBasisRepository" />
    public class MealBasisRepository : CachedLookupRepository<MealBasis>, IMealBasisRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MealBasisRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public MealBasisRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}