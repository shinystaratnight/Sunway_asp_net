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
    public class ConnectFlightCarrierRepository : ConnectLookupBase<FlightCarrier>, IFlightCarrierRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectFlightCarrierRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectFlightCarrierRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>
        /// A list of the entity
        /// </returns>
        protected override List<FlightCarrier> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("flightcarrier");
            XDocument xDoc = xml.ToXDocument();

            var carriers = new List<FlightCarrier>();

            XElement element = xDoc.Element("Lookups")?.Element("FlightCarriers");
            if (element != null)
            {
                foreach (XElement xElement in element?.Elements("FlightCarrier"))
                {
                    var carrier = new FlightCarrier()
                    {
                        CarrierType = (string)xElement.Element("CarrierType"),
                        Logo = (string)xElement.Element("Logo"),
                        Id = (int)xElement.Element("FlightCarrierID"),
                        Name = (string)xElement.Element("FlightCarrier"),
                        WebDescription = (string)xElement.Element("WebDescription")
                    };

                    carriers.Add(carrier);
                }
            }

            return carriers;
        }
    }
}