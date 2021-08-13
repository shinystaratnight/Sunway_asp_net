namespace Web.Template.Data.Lookup.Repositories.Flight
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Linq;

    using Intuitive;

    using Web.Template.Data.Connect;
    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Geography;

    /// <summary>
    ///     Airport Repository that is responsible for managing access to airports
    /// </summary>
    /// <seealso cref="IAirportRepository" />
    public class ConnectResortRepository : ConnectLookupBase<Resort>, IResortRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectResortRepository"/> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectResortRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>
        /// A list of the entity
        /// </returns>
        protected override List<Resort> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("Location");
            XDocument xDoc = xml.ToXDocument();
            var resorts = new List<Resort>();

            foreach (XElement xElement in xDoc.Element("Lookups")?.Element("Locations").Elements("Location"))
            {
                if (!resorts.Exists(r => r.Id == (int)xElement.Element("GeographyLevel3ID")))
                {
                    var resort = new Resort()
                                     {
                                         Name = (string)xElement.Element("GeographyLevel3Name"), 
                                         Code = (string)xElement.Element("GeographyLevel3Code"), 
                                         Id = (int)xElement.Element("GeographyLevel3ID"), 
                                         RegionID = (int)xElement.Element("GeographyLevel2ID")
                                     };
                    resorts.Add(resort);
                }
            }

            return resorts;
        }
    }
}