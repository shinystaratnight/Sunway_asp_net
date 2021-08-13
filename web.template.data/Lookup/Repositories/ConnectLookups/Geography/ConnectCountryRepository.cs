namespace Web.Template.Data.Lookup.Repositories.ConnectLookups.Geography
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    using Intuitive;

    using Web.Template.Data.Connect;
    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Geography;

    /// <summary>
    /// connect Country repository
    /// </summary>
    /// <seealso cref="Web.Template.Data.Lookup.Repositories.Generic.ConnectLookupBase{Web.Template.Domain.Entities.Geography.Country}" />
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Geography.ICountryRepository" />
    public class ConnectCountryRepository : ConnectLookupBase<Country>, ICountryRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectCountryRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectCountryRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>
        /// A list of the entity
        /// </returns>
        protected override List<Country> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("Location");
            XDocument xDoc = xml.ToXDocument();
            var countries = new List<Country>();

            IEnumerable<XElement> xElements = xDoc.Element("Lookups")?.Element("Locations")?.Elements("Location");
            if (xElements != null)
            {
                foreach (XElement xElement in xElements)
                {
                    if (!countries.Exists(c => c.Id == (int)xElement.Element("GeographyLevel1ID")))
                    {
                        var country = new Country()
                                          {
                                              Name = (string)xElement.Element("GeographyLevel1Name"), 
                                              Code = (string)xElement.Element("GeographyLevel1Code"), 
                                              Id = (int)xElement.Element("GeographyLevel1ID"), 
                                              Regions = new List<Region>()
                                          };

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
                        country.Regions.Add(region);
                        countries.Add(country);
                    }
                    else
                    {
                        var country = countries.FirstOrDefault(c => c.Id == (int)xElement.Element("GeographyLevel1ID"));

                        if (country != null
                            && !country.Regions.Exists(r => r.Id == (int)xElement.Element("GeographyLevel2ID")))
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

                            country.Regions.Add(region);
                        }
                        else
                        {
                            var region =
                                country?.Regions.FirstOrDefault(r => r.Id == (int)xElement.Element("GeographyLevel2ID"));

                            var resort = new Resort()
                                             {
                                                 Name = (string)xElement.Element("GeographyLevel3Name"), 
                                                 Code = (string)xElement.Element("GeographyLevel3Code"), 
                                                 Id = (int)xElement.Element("GeographyLevel3ID"), 
                                                 RegionID = (int)xElement.Element("GeographyLevel2ID")
                                             };

                            region?.Resorts.Add(resort);
                        }
                    }
                }
            }

            return countries;
        }
    }
}