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
    public class ConnectAirportGroupRepository : ConnectLookupBase<AirportGroup>, IAirportGroupRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectAirportGroupRepository"/> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectAirportGroupRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>
        /// A list of the entity
        /// </returns>
        protected override List<AirportGroup> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("AirportGroup");
            XDocument xDoc = xml.ToXDocument();
            var groups = new List<AirportGroup>();

            foreach (XElement xElement in xDoc.Element("Lookups")?.Element("AirportGroups").Elements("AirportGroup"))
            {
                var group = new AirportGroup()
                                {
                                    Id = (int)xElement.Element("AirportGroupID"), 
                                    Name = (string)xElement.Element("AirportGroup"), 
                                    DisplayOnSearch = (bool)xElement.Element("DisplayOnSearch"), 
                                    Type = (string)xElement.Element("Type"),
                                    PreferredGroup = (bool)xElement.Element("PreferredGroup"),
                                    Airports = new List<Airport>()
                                };

                groups.Add(group);
            }

            return groups;
        }
    }
}