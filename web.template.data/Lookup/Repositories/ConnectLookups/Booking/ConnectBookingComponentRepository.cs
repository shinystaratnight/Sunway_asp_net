namespace Web.Template.Data.Lookup.Repositories.ConnectLookups.Booking
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Linq;

    using Intuitive;

    using Web.Template.Data.Connect;
    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;

    /// <summary>
    ///     Booking Country Repository that is responsible for managing access to airports
    /// </summary>
    public class ConnectBookingComponentRepository : ConnectLookupBase<BookingComponent>, IBookingComponentRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectBookingComponentRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">A class using intuitive async cache to get lookup xml from connect.</param>
        public ConnectBookingComponentRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of Booking Country</returns>
        protected override List<BookingComponent> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("BookingComponent");
            XDocument xDoc = xml.ToXDocument();
            var components = new List<BookingComponent>();

            foreach (
                XElement xElement in xDoc.Element("Lookups")?.Element("BookingComponents").Elements("BookingComponent"))
            {
                var component = new BookingComponent()
                                    {
                                        Id = (int)xElement.Element("BookingComponentID"), 
                                        Name = (string)xElement.Element("BookingComponent"), 
                                    };
                components.Add(component);
            }

            return components;
        }
    }
}