namespace Web.Template.API.Lookup
{
    using System.Collections.Generic;
    using System.Web.Http;

    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Domain.Entities.Property;

    /// <summary>
    /// Controller responsible for all property based lookup access
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class PropertyController : ApiController
    {
        /// <summary>
        /// The property service
        /// </summary>
        private readonly IPropertyService propertyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyController"/> class.
        /// </summary>
        /// <param name="propertyService">The property service.</param>
        public PropertyController(IPropertyService propertyService)
        {
            this.propertyService = propertyService;
        }

        /// <summary>
        /// Gets all meal basis.
        /// </summary>
        /// <returns>All Meal basis</returns>
        [Route("api/property/mealbasis")]
        [HttpGet]
        public List<MealBasis> GetAllMealBasis()
        {
            return this.propertyService.GetAllMealBasis();
        }

        /// <summary>
        /// Gets all product attributes.
        /// </summary>
        /// <returns>All product attributes</returns>
        [Route("api/property/productattribute")]
        [HttpGet]
        public List<ProductAttribute> GetAllProductAttributes()
        {
            return this.propertyService.GetAllProductAttributes();
        }

        /// <summary>
        /// Gets all property references.
        /// </summary>
        /// <returns>all property references</returns>
        [Route("api/property/propertyReference")]
        [HttpGet]
        public List<PropertyReference> GetAllPropertyReferences()
        {
            return this.propertyService.GetAllPropertyReferences();
        }

        /// <summary>
        /// Gets the filter facilities.
        /// </summary>
        /// <returns>All filter facilities</returns>
        [Route("api/property/filterfacility")]
        [HttpGet]
        public List<FilterFacility> GetFilterFacilities()
        {
            return this.propertyService.GetFilterFacilities();
        }

        /// <summary>
        /// Gets the meal basis.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Meal basis that match the provided id</returns>
        [Route("api/property/mealbasis/{id}")]
        [HttpGet]
        public MealBasis GetMealBasis(int id)
        {
            return this.propertyService.GetMealBasis(id);
        }

        /// <summary>
        /// Gets the product attribute.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>product attribute that matches the provided id</returns>
        [Route("api/property/productattribute/{id}")]
        [HttpGet]
        public ProductAttribute GetProductAttribute(int id)
        {
            return this.propertyService.GetProductAttribute(id);
        }

        /// <summary>
        /// Gets the property reference.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A single property</returns>
        [Route("api/property/propertyReference/{id}")]
        [HttpGet]
        public PropertyReference GetPropertyReference(int id)
        {
            return this.propertyService.GetPropertyReference(id);
        }
    }
}