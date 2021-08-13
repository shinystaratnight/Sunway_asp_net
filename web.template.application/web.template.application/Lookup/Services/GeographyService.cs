namespace Web.Template.Application.Lookup.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Geography;

    /// <summary>
    /// Service responsible for access to geography data
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Lookup.Services.IGeographyService" />
    public class GeographyService : IGeographyService
    {
        /// <summary>
        /// The country repository
        /// </summary>
        private readonly ICountryRepository countryRepository;

        /// <summary>
        /// The region repository
        /// </summary>
        private readonly IRegionRepository regionRepository;

        /// <summary>
        /// The resort repository
        /// </summary>
        private readonly IResortRepository resortRepository;

        /// <summary>
        /// The brand geography repository
        /// </summary>
        private readonly IBrandGeographyRepository brandGeographyRepository;

        /// <summary>
        /// The geography grouping repository
        /// </summary>
        private readonly IGeographyGroupingRepository geographyGroupingRepository;

        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// Gets the country cache key.
        /// </summary>
        /// <value>
        /// The country cache key.
        /// </value>
        private string countryCacheKey => "geographiesFilteredByBrand";

        /// <summary>
        /// Initializes a new instance of the <see cref="GeographyService" /> class.
        /// </summary>
        /// <param name="resortRepository">The resort repository.</param>
        /// <param name="regionRepository">The region repository.</param>
        /// <param name="countryRepository">The country repository.</param>
        /// <param name="brandGeographyRepository">The brand geography repository.</param>
        /// <param name="geographyGroupingRepository">The geography grouping repository.</param>
        /// <param name="siteService">The site service.</param>
        public GeographyService(IResortRepository resortRepository, IRegionRepository regionRepository,
            ICountryRepository countryRepository, IBrandGeographyRepository brandGeographyRepository,
            IGeographyGroupingRepository geographyGroupingRepository, ISiteService siteService)
        {
            this.resortRepository = resortRepository;
            this.regionRepository = regionRepository;
            this.countryRepository = countryRepository;
            this.brandGeographyRepository = brandGeographyRepository;
            this.geographyGroupingRepository = geographyGroupingRepository;
            this.siteService = siteService;
        }

        /// <summary>
        /// Gets the countries.
        /// </summary>
        /// <returns>
        /// All countries
        /// </returns>
        public List<Country> GetCountries()
        {
            List<Country> countries;
            if (HttpContext.Current.Cache[this.countryCacheKey] != null)
            {
                countries = (List<Country>)HttpContext.Current.Cache[this.countryCacheKey];
            }
            else
            {
                countries = this.countryRepository.GetAll().ToList();
                HttpContext.Current.Cache[this.countryCacheKey] = countries;
            }

            return countries;
        }

        /// <summary>
        /// Gets the country.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// A country that matches the given ID
        /// </returns>
        public Country GetCountry(int id)
        {
            return this.countryRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the region.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// A region that matches the given identifier
        /// </returns>
        public Region GetRegion(int id)
        {
            return this.regionRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the regions.
        /// </summary>
        /// <returns>
        /// All regions
        /// </returns>
        public List<Region> GetRegions()
        {
            return this.regionRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets the resort.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// A resort that matches the given identifier
        /// </returns>
        public Resort GetResort(int id)
        {
            return this.resortRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the resorts.
        /// </summary>
        /// <returns>
        /// All resorts
        /// </returns>
        public List<Resort> GetResorts()
        {
            return this.resortRepository.GetAll().ToList();
        }

        /// <summary>
        /// Filters resorts.
        /// </summary>
        private void FilterResorts()
        {
            var countries = this.countryRepository.GetAll().ToList();
            ISite site = this.siteService.GetSite(HttpContext.Current);
            var brandId = site.BrandId;
            var brandGeographylevel3S = this.brandGeographyRepository.FindBy(x => x.BrandID == brandId);

            foreach (Country country in countries)
            {
                foreach (Region region in country.Regions)
                {
                    region.Resorts.RemoveAll(r => brandGeographylevel3S.All(b => b.Geographylevel3Id != r.Id));
                }
                country.Regions.RemoveAll(r => r.Resorts.Count == 0);
            }

            countries.RemoveAll(c => c.Regions.Count == 0);
        }
    }
}