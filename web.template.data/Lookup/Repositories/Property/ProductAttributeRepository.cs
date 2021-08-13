namespace Web.Template.Data.Lookup.Repositories.Property
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Property;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Property;

    /// <summary>
    /// Cached Repository Exposing Product Attributes.
    /// </summary>
    /// <seealso cref="Web.Template.Data.Lookup.Repositories.Generic.CachedLookupRepository{Web.Template.Domain.Entities.Property.ProductAttribute}" />
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Property.IProductAttributeRepository" />
    public class ProductAttributeRepository : CachedLookupRepository<ProductAttribute>, IProductAttributeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductAttributeRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public ProductAttributeRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}