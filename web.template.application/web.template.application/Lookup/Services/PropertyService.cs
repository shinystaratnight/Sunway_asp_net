namespace Web.Template.Application.Lookup.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Domain.Entities.Property;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Property;

    /// <summary>
    /// Service responsible for retrieving information concerning property lookups.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Lookup.Services.IPropertyService" />
    public class PropertyService : IPropertyService
    {
        /// <summary>
        /// The filter facility repository
        /// </summary>
        private readonly IFilterFacilityRepository filterFacilityRepository;

        /// <summary>
        /// The meal basis repository
        /// </summary>
        private readonly IMealBasisRepository mealBasisRepository;

        /// <summary>
        /// The product attribute repository
        /// </summary>
        private readonly IProductAttributeRepository productAttributeRepository;

        /// <summary>
        /// The property reference repository
        /// </summary>
        private readonly IPropertyReferenceRepository propertyReferenceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyService" /> class.
        /// </summary>
        /// <param name="filterFacilityRepository">The filter facility repository.</param>
        /// <param name="mealBasisRepository">The meal basis repository.</param>
        /// <param name="productAttributeRepository">The product attribute repository.</param>
        /// <param name="propertyReferenceRepository">The property reference repository.</param>
        public PropertyService(IFilterFacilityRepository filterFacilityRepository, IMealBasisRepository mealBasisRepository, IProductAttributeRepository productAttributeRepository, IPropertyReferenceRepository propertyReferenceRepository)
        {
            this.filterFacilityRepository = filterFacilityRepository;
            this.mealBasisRepository = mealBasisRepository;
            this.productAttributeRepository = productAttributeRepository;
            this.propertyReferenceRepository = propertyReferenceRepository;
        }

        /// <summary>
        /// Gets all meal basis.
        /// </summary>
        /// <returns>
        /// A list of all meal basis.
        /// </returns>
        public List<MealBasis> GetAllMealBasis()
        {
            return this.mealBasisRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets all product attributes.
        /// </summary>
        /// <returns>
        /// A list of all product attributes.
        /// </returns>
        public List<ProductAttribute> GetAllProductAttributes()
        {
            return this.productAttributeRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets all property references.
        /// </summary>
        /// <returns>All properties</returns>
        public List<PropertyReference> GetAllPropertyReferences()
        {
            return this.propertyReferenceRepository.GetAll().Where(propertyreference => propertyreference.Current).ToList();
        }

        /// <summary>
        /// Gets the filter facilities.
        /// </summary>
        /// <returns>
        /// A list of all filter facilities.
        /// </returns>
        public List<FilterFacility> GetFilterFacilities()
        {
            return this.filterFacilityRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets the meal basis.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// A single meal basis.
        /// </returns>
        public MealBasis GetMealBasis(int id)
        {
            return this.mealBasisRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the product attribute.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// A single product attribute.
        /// </returns>
        public ProductAttribute GetProductAttribute(int id)
        {
            return this.productAttributeRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the property reference.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A single property</returns>
        public PropertyReference GetPropertyReference(int id)
        {
            return this.propertyReferenceRepository.GetSingle(id);
        }
    }
}