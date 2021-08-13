namespace Web.Template.Data.Lookup.Repositories.Flight
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Linq;

    using Intuitive;

    using Web.Template.Data.Connect;
    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;

    /// <summary>
    ///     Airport Repository that is responsible for managing access to airports
    /// </summary>
    /// <seealso cref="IAirportRepository" />
    public class ConnectAirportGroupResortRepository : ConnectLookupBase<AirportGroupResort>, 
                                                       IAirportGroupResortRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectAirportGroupResortRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectAirportGroupResortRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>
        /// A list of the entity
        /// </returns>
        protected override List<AirportGroupResort> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("AirportGroupResort");
            XDocument xDoc = xml.ToXDocument();
            var groupResorts = new List<AirportGroupResort>();
            XElement element = xDoc.Element("Lookups")?.Element("AirportGroupResorts");

            if (element != null)
            {
                foreach (XElement xElement in element.Elements("AirportGroupResort"))
                {
                    var groupResort = new AirportGroupResort()
                                          {
                                              Id = (int)xElement.Element("AirportGroupResortID"), 
                                              AirportGroupId =
                                                  (int)xElement.Element("AirportGroupID"), 
                                              GeographyLevel3Id =
                                                  (int)xElement.Element("GeographyLevel3ID"), 
                                              GeographyLevel1Id =
                                                  (int)xElement.Element("GeographyLevel1ID")
                                          };

                    groupResorts.Add(groupResort);
                }
            }

            return groupResorts;
        }
    }
}