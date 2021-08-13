namespace Web.Template.Data.Lookup.Repositories.ConnectLookups.Extras
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Linq;

    using Intuitive;

    using Web.Template.Data.Connect;
    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Extras;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Extras;

    /// <summary>
    /// Extra type repository that is responsible for managing access to extra types.
    /// </summary>
    public class ConnectExtraTypeRepository : ConnectLookupBase<ExtraType>, IExtraTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectExtraTypeRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectExtraTypeRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        protected override List<ExtraType> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("ExtraType");
            XDocument xDoc = xml.ToXDocument();

            var extraTypes = new List<ExtraType>();

            XElement element = xDoc.Element("Lookups")?.Element("ExtraTypes");
            if (element != null)
            {
                foreach (XElement xElement in element?.Elements("ExtraType"))
                {
                    var extraType = new ExtraType
                                        {
                                            Id = (int)xElement.Element("ExtraTypeID"),
                                            Name = (string)xElement.Element("ExtraType")
                                        };
                    extraTypes.Add(extraType);
                }
            }

            return extraTypes;
        }
    }
}
