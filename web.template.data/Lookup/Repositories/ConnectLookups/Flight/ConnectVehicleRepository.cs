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
    /// <seealso cref="IVehicleRepository" />
    public class ConnectVehicleRepository : ConnectLookupBase<Vehicle>, IVehicleRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectVehicleRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectVehicleRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>
        /// A list of the entity
        /// </returns>
        protected override List<Vehicle> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("Vehicle");
            XDocument xDoc = xml.ToXDocument();

            var vehicles = new List<Vehicle>();

            XElement element = xDoc.Element("Lookups")?.Element("Vehicles");
            if (element != null)
            {
                foreach (XElement xElement in element?.Elements("Vehicle"))
                {
                    var vehicle = new Vehicle()
                    {
                        Id = (int)xElement.Element("VehicleID"),
                        Name = (string)xElement.Element("VehicleName")
                    };

                    vehicles.Add(vehicle);
                }
            }

            return vehicles;
        }
    }
}