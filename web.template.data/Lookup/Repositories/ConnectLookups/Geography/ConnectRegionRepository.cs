namespace Web.Template.Data.Lookup.Repositories.Flight
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    using Intuitive;

    using Web.Template.Data.Connect;
    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Geography;

    /// <summary>
    ///     Airport Repository that is responsible for managing access to airports
    /// </summary>
    /// <seealso cref="IAirportRepository" />
    public class ConnectRegionRepository : ConnectLookupBase<Region>, IRegionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectRegionRepository"/> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectRegionRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>
        /// A list of the entity
        /// </returns>
        protected override List<Region> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("Location");
            XDocument xDoc = xml.ToXDocument();
            var regions = new List<Region>();

            foreach (XElement xElement in xDoc.Element("Lookups")?.Element("Locations").Elements("Location"))
            {
                if (!regions.Exists(r => r.Id == (int)xElement.Element("GeographyLevel2ID")))
                {
                    var region = new Region()
                                     {
                                         Name = (string)xElement.Element("GeographyLevel2Name"), 
                                         Code = (string)xElement.Element("GeographyLevel2Code"), 
                                         Id = (int)xElement.Element("GeographyLevel2ID"), 
                                         CountryId = (int)xElement.Element("GeographyLevel3ID"), 
                                         Resorts = new List<Resort>()
                                     };

                    var resort = new Resort()
                                     {
                                         Name = (string)xElement.Element("GeographyLevel3Name"), 
                                         Code = (string)xElement.Element("GeographyLevel3Code"), 
                                         Id = (int)xElement.Element("GeographyLevel3ID"), 
                                         RegionID = (int)xElement.Element("GeographyLevel2ID")
                                     };

                    region.Resorts.Add(resort);

                    regions.Add(region);
                }
                else
                {
                    var region = regions.FirstOrDefault(r => r.Id == (int)xElement.Element("GeographyLevel2ID"));

                    var resort = new Resort()
                                     {
                                         Name = (string)xElement.Element("GeographyLevel3Name"), 
                                         Code = (string)xElement.Element("GeographyLevel3Code"), 
                                         Id = (int)xElement.Element("GeographyLevel3ID"), 
                                         RegionID = (int)xElement.Element("GeographyLevel2ID")
                                     };

                    region.Resorts.Add(resort);
                }
            }

            return regions;
        }
    }
}