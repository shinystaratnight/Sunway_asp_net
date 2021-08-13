namespace Web.Template.Data.Lookup.Repositories.Flight
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Linq;

    using Intuitive;

    using Web.Template.Data.Connect;
    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Property;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Property;

    /// <summary>
    ///     Filter facility Repository that is responsible for managing access to airports
    /// </summary>
    public class ConnectProductAttributeRepository : ConnectLookupBase<ProductAttribute>, IProductAttributeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectProductAttributeRepository"/> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectProductAttributeRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of Filter Facility</returns>
        protected override List<ProductAttribute> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("ProductAttributes");
            XDocument xDoc = xml.ToXDocument();
            var attributes = new List<ProductAttribute>();

            XElement element = xDoc.Element("Lookups")?.Element("ProductAttributes");
            if (element != null)
            {
                foreach (XElement xElement in element?.Elements("ProductAttribute"))
                {
                    var attribute = new ProductAttribute()
                                        {
                                            Id = (int)xElement.Element("ProductAttributeID"), 
                                            Name = (string)xElement.Element("ProductAttribute"), 
                                            GroupID = (int)xElement.Element("ProductAttributeGroupID"), 
                                            GroupName =
                                                (string)xElement.Element("ProductAttributeGroup"), 
                                            Type = (string)xElement.Element("ProductAttributeType"), 
                                        };
                    attributes.Add(attribute);
                }
            }

            return attributes;
        }
    }
}