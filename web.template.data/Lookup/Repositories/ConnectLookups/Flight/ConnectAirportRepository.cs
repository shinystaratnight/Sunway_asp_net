namespace Web.Template.Data.Lookup.Repositories.Flight
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    using Intuitive;

    using Web.Template.Data.Connect;
    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;

    /// <summary>
    ///     Airport Repository that is responsible for managing access to airports
    /// </summary>
    /// <seealso cref="IAirportRepository" />
    public class ConnectAirportRepository : ConnectLookupBase<Airport>, IAirportRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectAirportRepository"/> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectAirportRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>
        /// A list of the entity
        /// </returns>
        protected override List<Airport> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("airport");
            XDocument xDoc = xml.ToXDocument();
            XDocument xLocations = this.GetLookupsXml("location").ToXDocument();

            var airports = new List<Airport>();

            var airportGeographyxml = this.GetLookupsXml("AirportGeography");
            var airportGeographyXDoc = airportGeographyxml.ToXDocument();

            foreach (XElement xElement in xDoc.Element("Lookups")?.Element("Airports").Elements("Airport"))
            {
                var airport = new Airport()
                {
                    GeographyLevel1ID = (int)xElement.Element("GeographyLevel1ID"),
                    IATACode = (string)xElement.Element("IATACode"),
                    Id = (int)xElement.Element("AirportID"),
                    Latitude = (string)xElement.Element("Latitude"),
                    Longitude = (string)xElement.Element("Longitude"),
                    Name = (string)xElement.Element("Airport"),
                    OffsetDays = (int)xElement.Element("OffsetDays"),
                    PreferredAirport = (bool)xElement.Element("PreferredAirport"),
                    Resorts = new List<Resort>(),
                    Type = (string)xElement.Element("Type")
                };

                foreach (
                    XElement innerXElement in
                        airportGeographyXDoc.Element("Lookups")?.Element("AirportGeographies")
                        .Elements("AirportGeography")
                            .Where(node => (int)node.Element("AirportID") == airport.Id))
                {
                    var resortId = (int)innerXElement.Element("GeographyLevel3ID");
                    var resort = new Resort()
                    {
                        Id = resortId,
                        RegionID = (int)xLocations.Element("Lookups")
                                        ?.Element("Locations")
                                        .Elements("Location")
                                        .FirstOrDefault(node => (int)node.Element("GeographyLevel3ID") == resortId)
                                        ?.Element("GeographyLevel2ID")
                    };
                    airport.Resorts.Add(resort);
                }

                airports.Add(airport);
            }

            return airports;
        }
    }
}