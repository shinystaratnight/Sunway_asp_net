namespace Web.Template.Data.Lookup.Repositories.ConnectLookups.Flight
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
    /// Interface for flight classes, responsible for accessing flight classes.
    /// </summary>
    public class ConnectFlightClassRepository : ConnectLookupBase<FlightClass>, IFlightClassRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectFlightClassRepository"/> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectFlightClassRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of flight classes</returns>
        protected override List<FlightClass> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("flightclass");
            XDocument xDoc = xml.ToXDocument();

            var flightClasses = new List<FlightClass>();

            foreach (XElement xElement in xDoc.Element("Lookups")?.Element("FlightClasses").Elements("FlightClass"))
            {
                var flightClass = new FlightClass()
                                      {
                                          Id = (int)xElement.Element("FlightClassID"), 
                                          Name = (string)xElement.Element("FlightClass")
                                      };
                flightClasses.Add(flightClass);
            }

            return flightClasses;
        }
    }
}