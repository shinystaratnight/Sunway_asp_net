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
    public class ConnectRouteAvailabilityRepository : ConnectLookupBase<RouteAvailability>, IRouteAvailabilityRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectRouteAvailabilityRepository"/> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectRouteAvailabilityRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>
        /// A list of the entity
        /// </returns>
        protected override List<RouteAvailability> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("RouteAvailability");
            XDocument xDoc = xml.ToXDocument();
            var routes = new List<RouteAvailability>();

            var routeAvailabilities = xDoc.Element("Lookups")?.Element("RouteAvailabilities");
            if (routeAvailabilities != null)
            {
                foreach (XElement xElement in routeAvailabilities.Elements("RouteAvailability"))
                {
                    var route = new RouteAvailability()
                                    {
                                        DepartureAirportID =
                                            (int)xElement.Element("DepartureAirportID"), 
                                        ArrivalAirportID = (int)xElement.Element("ArrivalAirportID")
                                    };

                    routes.Add(route);
                }
            }

            xml = this.GetLookupsXml("AirportGroupRouteAvailability");
            xDoc = xml.ToXDocument();

            var airportGroupRouteAvailabilities = xDoc.Element("Lookups")?.Element("AirportGroupRouteAvailabilities");
            if (airportGroupRouteAvailabilities != null)
            {
                foreach (XElement xElement in airportGroupRouteAvailabilities.Elements("AirportGroupRouteAvailability"))
                {
                    var route = new RouteAvailability()
                                    {
                                        DepartureAirportID =
                                            (int)xElement.Element("DepartureAirportID"), 
                                        ArrivalAirportID = (int)xElement.Element("ArrivalAirportID"), 
                                        AirportGroupID = (int)xElement.Element("AirportGroupID")
                                    };
                    routes.Add(route);
                }
            }

            return routes;
        }
    }
}