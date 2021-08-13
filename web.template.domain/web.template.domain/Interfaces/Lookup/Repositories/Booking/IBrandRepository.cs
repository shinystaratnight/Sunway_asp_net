namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Booking
{
    using System.Collections.Generic;

    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    ///  Lookup Repository of Brand
    /// </summary>
    /// <seealso cref="Brand" />
    public interface IBrandRepository : ILookupRepository<Brand>
    {
        /// <summary>
        /// Gets the brands with geography.
        /// </summary>
        /// <returns>the brands with geography</returns>
        IEnumerable<Brand> GetBrandsWithGeography();

        /// <summary>
        /// Gets the country with regions and resorts.
        /// </summary>
        /// <param name="brandId">The brand identifier.</param>
        /// <returns>
        /// A Country with its regions and resorts filled in.
        /// </returns>
        Brand GetBrandWithGeography(int brandId);
    }
}