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
    /// Class ConnectNationalityRepository.
    /// </summary>
    public class ConnectNationalityRepository : ConnectLookupBase<Nationality>, INationalityRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectNationalityRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">A class using intuitive async cache to get lookup xml from connect.</param>
        public ConnectNationalityRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of the entity</returns>
        protected override List<Nationality> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("Nationality");
            XDocument xdoc = xml.ToXDocument();

            var nationalities = new List<Nationality>();

            XElement element = xdoc.Element("Lookups")?.Element("Nationalities");
            if (element != null)
            {
                foreach (XElement xElement in element.Elements("Nationality"))
                {
                    var nationality = new Nationality()
                                          {
                                              Id = (int)xElement.Element("NationalityID"), 
                                              ISOCode = (string)xElement.Element("ISOCode"), 
                                              Name = (string)xElement.Element("Nationality"), 
                                              SellingGeographyLevel1ID =
                                                  (int)xElement.Element("SellingGeographyLevel1ID")
                                          };
                    nationalities.Add(nationality);
                }
            }

            return nationalities;
        }
    }
}