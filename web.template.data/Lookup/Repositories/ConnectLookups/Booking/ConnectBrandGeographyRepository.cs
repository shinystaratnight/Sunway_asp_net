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
    public class ConnectBrandGeographyRepository : ConnectLookupBase<BrandGeography>, IBrandGeographyRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectBrandGeographyRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">A class using intuitive async cache to get lookup xml from connect.</param>
        public ConnectBrandGeographyRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of Booking Country</returns>
        protected override List<BrandGeography> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("BrandGeography");

            XDocument xDoc = xml.ToXDocument();
            var geos = new List<BrandGeography>();

            foreach (
                XElement xElement in xDoc.Element("Lookups")?.Element("BrandGeographies").Elements("BrandGeography"))
            {
                var bg = new BrandGeography()
                {
                    BrandID = (int)xElement.Element("BrandID"),
                    Geographylevel3Id = (int)xElement.Element("GeographyLevel3ID"),
                };
                geos.Add(bg);
            }

            return geos;
        }
    }
}