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
    public class ConnectBookingCountryRepository : ConnectLookupBase<BookingCountry>, IBookingCountryRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectBookingCountryRepository"/> class.
        /// </summary>
        /// <param name="asyncLookup">A class using intuitive async cache to get lookup xml from connect.</param>
        public ConnectBookingCountryRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of Booking Country</returns>
        protected override List<BookingCountry> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("BookingCountry");
            XDocument xDoc = xml.ToXDocument();
            var countries = new List<BookingCountry>();

            foreach (
                XElement xElement in xDoc.Element("Lookups")?.Element("BookingCountries").Elements("BookingCountry"))
            {
                var country = new BookingCountry()
                                  {
                                      Id = (int)xElement.Element("BookingCountryID"), 
                                      Name = (string)xElement.Element("BookingCountry"), 
                                      DefaultCountry = (bool)xElement.Element("DefaultCountry"), 
                                      ISOCode = (string)xElement.Element("ISOCode")
                                  };
                countries.Add(country);
            }

            return countries;
        }
    }
}