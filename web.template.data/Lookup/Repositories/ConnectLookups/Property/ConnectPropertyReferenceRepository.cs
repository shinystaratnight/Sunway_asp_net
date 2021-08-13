namespace Web.Template.Data.Lookup.Repositories.ConnectLookups.Property
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
    /// Class ConnectPropertyReferenceRepository.
    /// </summary>
    public class ConnectPropertyReferenceRepository : ConnectLookupBase<PropertyReference>, IPropertyReferenceRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectPropertyReferenceRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">A class using intuitive async cache to get lookup xml from connect.</param>
        public ConnectPropertyReferenceRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of the entity</returns>
        protected override List<PropertyReference> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("PropertyReference");
            XDocument xdoc = xml.ToXDocument();
            var propertyReferences = new List<PropertyReference>();

            XElement element = xdoc.Element("Lookups")?.Element("PropertyReferences");
            if (element != null)
            {
                foreach (XElement xElement in element.Elements("PropertyReference"))
                {
                    var propertyReference = new PropertyReference()
                                                {
                                                    Current =
                                                        (bool)
                                                        xElement.Element("CurrentPropertyReference"), 
                                                    Id =
                                                        (int)xElement.Element("PropertyReferenceID"), 
                                                    Name = (string)xElement.Element("PropertyName")
                                                };
                    propertyReferences.Add(propertyReference);
                }
            }

            return propertyReferences;
        }
    }
}