namespace Web.Template.Data.Lookup.Repositories.Booking
{
    using System.Collections.Generic;
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;

    /// <summary>
    /// Cached Repository Exposing Brand
    /// </summary>
    /// <seealso cref="Brand" />
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Booking.IBrandRepository" />
    public class BrandRepository : CachedLookupRepository<Brand>, IBrandRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrandRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public BrandRepository(DbContext dbContext)
            : base(dbContext)
        {
        }

        /// <summary>
        /// Gets the countries with regions and resort.
        /// </summary>
        /// <returns>
        /// All countries with their regions and resorts filled in.
        /// </returns>
        public IEnumerable<Brand> GetBrandsWithGeography()
        {
            return this.GetAll(brand => brand.Include(b => b.BrandGeography));
        }

        /// <summary>
        /// Gets the country with regions and resorts.
        /// </summary>
        /// <param name="brandId">The brand identifier.</param>
        /// <returns>
        /// A Country with its regions and resorts filled in.
        /// </returns>
        public Brand GetBrandWithGeography(int brandId)
        {
            return this.GetSingle(brandId, brand => brand.Include(b => b.BrandGeography));
        }
    }
}