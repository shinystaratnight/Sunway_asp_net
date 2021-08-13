namespace Web.Template.Application.Lookup.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;

    /// <summary>
    /// Service responsible for access to brand data
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Lookup.Services.IBrandService" />
    public class BrandService : IBrandService
    {
        /// <summary>
        /// The brand repository
        /// </summary>
        private readonly IBrandRepository brandRepository;

        /// <summary>
        /// Gets the brand cache key.
        /// </summary>
        /// <value>
        /// The brand cache key.
        /// </value>
        private string brandCacheKey => "brandCache";

        /// <summary>
        /// Initializes a new instance of the <see cref="BrandService" /> class.
        /// </summary>
        /// <param name="brandRepository">The brand repository.</param>
        public BrandService(IBrandRepository brandRepository)
        {
            this.brandRepository = brandRepository;
        }

        /// <summary>
        /// Gets the brands.
        /// </summary>
        /// <returns></returns>
        public List<Brand> GetBrands()
        {
            List<Brand> brands;
            if (HttpContext.Current.Cache[this.brandCacheKey] != null)
            {
                brands = (List<Brand>)HttpContext.Current.Cache[this.brandCacheKey];
            }
            else
            {
                brands = this.brandRepository.GetAll().ToList();
            }

            return brands;
        }

        /// <summary>
        /// Gets the brand.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Brand GetBrand(int id)
        {
            return this.brandRepository.GetSingle(id);
        }
    }
}