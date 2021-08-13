namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Property
{
    using Web.Template.Domain.Entities.Property;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    ///  Lookup Repository of Product Attributes.
    /// </summary>
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Generic.ILookupRepository{Web.Template.Domain.Entities.Property.ProductAttribute}" />
    public interface IProductAttributeRepository : ILookupRepository<ProductAttribute>
    {
    }
}