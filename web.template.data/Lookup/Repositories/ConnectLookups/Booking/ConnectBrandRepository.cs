namespace Web.Template.Data.Lookup.Repositories.ConnectLookups.Booking
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Xml;
    using System.Xml.Linq;

    using Intuitive;

    using Web.Template.Data.Connect;
    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;

    /// <summary>
    /// Class ConnectBrandRepository.
    /// </summary>
    public class ConnectBrandRepository : ConnectLookupBase<Brand>, IBrandRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectBrandRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">A class using intuitive async cache to get lookup xml from connect.</param>
        public ConnectBrandRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Gets the brands with geography.
        /// </summary>
        /// <returns>the brands with geography</returns>
        public IEnumerable<Brand> GetBrandsWithGeography()
        {
            return this.GetAll(brand => brand.Include(b => b.BrandGeography));
        }

        /// <summary>
        /// Gets the country with regions and resorts.
        /// </summary>
        /// <param name="brandId">The brand identifier.</param>
        /// <returns>A Country with its regions and resorts filled in.</returns>
        public Brand GetBrandWithGeography(int brandId)
        {
            return this.GetSingle(brandId, brand => brand.Include(b => b.BrandGeography));
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of the entity</returns>
        protected override List<Brand> Setup()
        {
            XmlDocument brandXml = this.GetLookupsXml("Brand");
            XDocument brandXDoc = brandXml.ToXDocument();

            XmlDocument brandGeographyXml = this.GetLookupsXml("BrandGeography");
            XDocument brandGeographyXDoc = brandGeographyXml.ToXDocument();

            XElement brandElement = brandXDoc.Element("Lookups")?.Element("Brands");
            XElement brandGeographyElement = brandGeographyXDoc.Element("Lookups")?.Element("BrandGeographies");

            var brands = new List<Brand>();

            if (brandElement != null)
            {
                foreach (XElement xElement in brandElement.Elements("Brand"))
                {
                    var brand = new Brand()
                                    {
                                        BrandCode = (string)xElement.Element("BrandCode"), 
                                        DefaultBrand = (bool)xElement.Element("DefaultBrand"), 
                                        DefaultCountryOfResidenceGL1Id =
                                            (int)xElement.Element("DefaultCountryOfResidenceGL1ID"), 
                                        DefaultCustomerCurrencyId =
                                            (int)xElement.Element("DefaultCustomerCurrencyID"), 
                                        DefaultNationalityId = (int)xElement.Element("DefaultNationalityID"), 
                                        Id = (int)xElement.Element("BrandID"), 
                                        IncludeNationalityAndResidency =
                                            (bool)xElement.Element("IncludeNationalityAndResidency"), 
                                        Name = (string)xElement.Element("BrandName"), 
                                        SellingGeographyLevel1Id =
                                            (int)xElement.Element("SellingGeographyLevel1ID")
                                    };

                    if (brandGeographyElement != null)
                    {
                        brand.BrandGeography = new List<Resort>();
                        foreach (XElement brandGeography in brandGeographyElement?.Elements("BrandGeography"))
                        {
                            var brandId = (int)brandGeography.Element("BrandID");
                            if (brandId == brand.Id)
                            {
                                var resort = new Resort() { Id = (int)brandGeography.Element("GeographyLevel3ID") };
                                brand.BrandGeography.Add(resort);
                            }
                        }
                    }

                    brands.Add(brand);
                }
            }

            return brands;
        }
    }
}