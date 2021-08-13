namespace Web.Booking.API.Lookup
{
    using System.Collections.Generic;
    using System.Web.Http;

    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Domain.Entities.Booking;

    /// <summary>
    /// Web API controller for retrieving information regarding brands
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class BrandController : ApiController
    {
        /// <summary>
        /// The brand service
        /// </summary>
        private readonly IBrandService brandService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrandController"/> class.
        /// </summary>
        /// <param name="brandService">The brand service.</param>
        public BrandController(IBrandService brandService)
        {
            this.brandService = brandService;
        }

        /// <summary>
        /// Gets the brands.
        /// </summary>
        /// <returns>All Brands</returns>
        [Route("api/Brand")]
        [HttpGet]
        public List<Brand> GetBrands()
        {
            List<Brand> brands = this.brandService.GetBrands();
            return brands;
        }

        /// <summary>
        /// Gets the brand.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A brand that matches the specified ID</returns>
        [Route("api/Brand/{id}")]
        [HttpGet]
        public Brand GetBrand(int id)
        {
            return this.brandService.GetBrand(id);
        }
    }
}