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
    ///     Airport Repository that is responsible for managing access to airport group airports
    /// </summary>
    /// <seealso cref="IAirportRepository" />
    public class ConnectAirportGroupAirportRepository : ConnectLookupBase<AirportGroupAirport>, IAirportGroupAirportRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectAirportGroupRepository"/> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectAirportGroupAirportRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>
        /// A list of the entity
        /// </returns>
        protected override List<AirportGroupAirport> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("AirportGroupAirport");
            XDocument xDoc = xml.ToXDocument();
            var groups = new List<AirportGroupAirport>();

            foreach (XElement xElement in xDoc.Element("Lookups")?.Element("AirportGroupAirports").Elements("AirportGroupAirport"))
            {
                var group = new AirportGroupAirport()
                                {
                                    Id = (int)xElement.Element("AirportGroupAirportID"), 
                                    AirportGroupID = (int)xElement.Element("AirportGroupID"), 
                                    AirportID = (int)xElement.Element("AirportID")
                                };

                groups.Add(group);
            }

            return groups;
        }
    }
}