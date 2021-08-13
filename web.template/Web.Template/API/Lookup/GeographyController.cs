namespace Web.Template.API.Lookup
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Models.Application;

    /// <summary>
    /// Web API controller for retrieving information regarding geographies
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class GeographyController : ApiController
    {
        /// <summary>
        /// The geography service
        /// </summary>
        private readonly IGeographyService geographyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeographyController"/> class.
        /// </summary>
        /// <param name="geographyService">The geography service.</param>
        public GeographyController(IGeographyService geographyService)
        {
            this.geographyService = geographyService;
        }

        /// <summary>
        /// Gets the countries.
        /// </summary>
        /// <returns>All Countries</returns>
        [Route("api/Geography/Country")]
        [HttpGet]
        public List<Country> GetCountries()
        {
            List<Country> countries = this.geographyService.GetCountries();
            return countries;
        }

        /// <summary>
        /// Gets the country.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A country that matches the specified ID</returns>
        [Route("api/Geography/Country/{id}")]
        [HttpGet]
        public Country GetCountry(int id)
        {
            return this.geographyService.GetCountry(id);
        }

        /// <summary>
        /// Gets the region.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A Region that matches the provided Id</returns>
        [Route("api/Geography/Region/{id}")]
        [HttpGet]
        public Region GetRegion(int id)
        {
            return this.geographyService.GetRegion(id);
        }

        /// <summary>
        /// Gets the regions.
        /// </summary>
        /// <returns>All Regions</returns>
        [Route("api/Geography/Region")]
        [HttpGet]
        public List<Region> GetRegions()
        {
            return this.geographyService.GetRegions();
        }

        /// <summary>
        /// Gets the resort.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A Resort that matches the provided Id</returns>
        [Route("api/Geography/Resort/{id}")]
        [HttpGet]
        public Resort GetResort(int id)
        {
            return this.geographyService.GetResort(id);
        }

        /// <summary>
        /// Gets the resorts.
        /// </summary>
        /// <returns>All Resorts</returns>
        [Route("api/Geography/Resort")]
        [HttpGet]
        public List<Resort> GetResorts()
        {
            return this.geographyService.GetResorts();
        }

        /// <summary>
        /// Gets the region and resorts.
        /// </summary>
        /// <returns>All region and resorts</returns>
        [Route("api/Geography/RegionResort")]
        [HttpGet]
        public List<SearchDestination> GetRegionResorts()
        {
            var destinations = new List<SearchDestination>();
            List<Region> regions = this.geographyService.GetCountries().SelectMany(c => c.Regions).ToList();

            foreach (var region in regions)
            {
                var regionDestination = new SearchDestination() { Id = region.Id, Name = region.Name, };
                destinations.Add(regionDestination);

                foreach (var resort in region.Resorts)
                {
                    var resortDestination = new SearchDestination()
                    {
                        Id = resort.Id * -1,
                        Name = $"{resort.Name}, {region.Name}"
                    };
                    destinations.Add(resortDestination);
                }
            }

            return destinations;
        }
    }
}