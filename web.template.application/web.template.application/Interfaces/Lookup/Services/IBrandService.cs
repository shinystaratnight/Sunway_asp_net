namespace Web.Template.Application.Interfaces.Lookup.Services
{
    using System.Collections.Generic;
    using Web.Template.Domain.Entities.Booking;

    /// <summary>
    /// Service responsible for access to Brand Information
    /// </summary>
    public interface IBrandService
    {
        /// <summary>
        /// Gets the brands.
        /// </summary>
        /// <returns></returns>
        List<Brand> GetBrands();

        /// <summary>
        /// Gets the brand.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Brand GetBrand(int id);
    }
}