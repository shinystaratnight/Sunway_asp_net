namespace Web.Template.Application.Interfaces.Lookup.Services
{
    using System.Collections.Generic;

    using Web.Template.Domain.Entities.Property;

    /// <summary>
    /// Property service interface, defining a class responsible for managing access to property information.
    /// </summary>
    public interface IPropertyService
    {
        /// <summary>
        /// Gets all meal basis.
        /// </summary>
        /// <returns>A list of all meal basis.</returns>
        List<MealBasis> GetAllMealBasis();

        /// <summary>
        /// Gets all product attributes.
        /// </summary>
        /// <returns>A list of all product attributes.</returns>
        List<ProductAttribute> GetAllProductAttributes();

        /// <summary>
        /// Gets all property references.
        /// </summary>
        /// <returns>All properties</returns>
        List<PropertyReference> GetAllPropertyReferences();

        /// <summary>
        /// Gets the filter facilities.
        /// </summary>
        /// <returns>A list of all filter facilities.</returns>
        List<FilterFacility> GetFilterFacilities();

        /// <summary>
        /// Gets the meal basis.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A single meal basis.</returns>
        MealBasis GetMealBasis(int id);

        /// <summary>
        /// Gets the product attribute.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A single product attribute.</returns>
        ProductAttribute GetProductAttribute(int id);

        /// <summary>
        /// Gets the property reference.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A single property</returns>
        PropertyReference GetPropertyReference(int id);
    }
}