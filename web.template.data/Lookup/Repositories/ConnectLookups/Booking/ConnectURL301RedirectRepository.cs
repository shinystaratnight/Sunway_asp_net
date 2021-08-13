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
    public class ConnectURL301RedirectRepository : ConnectLookupBase<URL301Redirect>, IURL301RedirectRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectURL301RedirectRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">A class using intuitive async cache to get lookup xml from connect.</param>
        public ConnectURL301RedirectRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of Booking Country</returns>
        protected override List<URL301Redirect> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("URL301redirect", true);

            XDocument xDoc = xml.ToXDocument();
            var redirects = new List<URL301Redirect>();

            XElement element = xDoc.Element("Lookups")?.Element("URL301Redirects");

            if (element != null)
            {
                foreach (
                    XElement xElement in element.Elements("URL301Redirect"))
                {
                    var redirect = new URL301Redirect()
                    {
                        Id = (int)xElement.Element("URL301RedirectID"),
                        BrandID = (int)xElement.Element("BrandID"),
                        URL = (string)xElement.Element("URL"),
                        RedirectURL = (string)xElement.Element("RedirectURL"),
                    };
                    redirects.Add(redirect);
                }
            }

            return redirects;
        }
    }
}