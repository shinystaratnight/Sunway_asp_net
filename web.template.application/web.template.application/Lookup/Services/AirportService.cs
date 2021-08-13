namespace Web.Template.Application.Lookup.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Caching;

    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.ViewModels.Flight;
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Geography;

    /// <summary>
    /// Airport Service responsible for data regarding Airports and Airport Groups
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Lookup.Services.IAirportService" />
    /// <seealso cref="IAirportService" />
    public class AirportService : IAirportService
    {
        /// <summary>
        /// The cache lock object
        /// </summary>
        private static readonly object CacheLockObject = new object();

        /// <summary>
        /// The airport group repository
        /// </summary>
        private readonly IAirportGroupRepository airportGroupRepository;

        /// <summary>
        /// The airport group airport repository
        /// </summary>
        private readonly IAirportGroupAirportRepository airportGroupAirportRepository;

        /// <summary>
        /// The airport group resort repository
        /// </summary>
        private readonly IAirportGroupResortRepository airportGroupResortRepository;

        /// <summary>
        /// The airport repository
        /// </summary>
        private readonly IAirportRepository airportRepository;

        /// <summary>
        /// The brand repository
        /// </summary>
        private readonly IBrandRepository brandRepository;

        /// <summary>
        /// The brand geography repository
        /// </summary>
        private readonly IBrandGeographyRepository brandGeographyRepository;

        /// <summary>
        /// The resort repository
        /// </summary>
        private readonly IResortRepository resortRepository;

        /// <summary>
        /// The route availability repository
        /// </summary>
        private readonly IRouteAvailabilityRepository routeAvailabilityRepository;

        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AirportService" /> class.
        /// </summary>
        /// <param name="airportRepository">The airport repository.</param>
        /// <param name="airportGroupRepository">The airport group repository.</param>
        /// <param name="airportGroupResortRepository">The airport group resort repository.</param>
        /// <param name="routeAvailabilityRepository">The route availability repository.</param>
        /// <param name="siteService">The site service.</param>
        /// <param name="resortRepository">The resort repository.</param>
        /// <param name="brandRepository">The brand repository.</param>
        /// <param name="brandGeographyRepository">The brand geography repository.</param>
        /// <param name="airportGroupAirportRepository">The airport group airport repository.</param>
        public AirportService(
            IAirportRepository airportRepository,
            IAirportGroupRepository airportGroupRepository,
            IAirportGroupResortRepository airportGroupResortRepository,
            IRouteAvailabilityRepository routeAvailabilityRepository,
            ISiteService siteService,
            IResortRepository resortRepository,
            IBrandRepository brandRepository,
            IBrandGeographyRepository brandGeographyRepository,
            IAirportGroupAirportRepository airportGroupAirportRepository)
        {
            this.airportRepository = airportRepository;
            this.airportGroupRepository = airportGroupRepository;
            this.airportGroupResortRepository = airportGroupResortRepository;
            this.routeAvailabilityRepository = routeAvailabilityRepository;
            this.siteService = siteService;
            this.resortRepository = resortRepository;
            this.brandRepository = brandRepository;
            this.brandGeographyRepository = brandGeographyRepository;
            this.airportGroupAirportRepository = airportGroupAirportRepository;
        }

        /// <summary>
        /// Gets the airport group by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// An Airport Group with the matching ID
        /// </returns>
        public AirportGroup GetAirportGroupById(int id)
        {
            return this.airportGroupRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the airport groups.
        /// </summary>
        /// <returns>
        /// All Airport Groups
        /// </returns>
        public List<AirportGroup> GetAirportGroups()
        {
            return this.airportGroupRepository.GetAll().ToList();
        }

        public List<AirportGroupResort> GetAirportGroupResorts()
        {
            return this.airportGroupResortRepository.GetAll().ToList();
        }

        public AirportGroupModel GetAirportsByGroup(bool groupByAirportGroup, bool sortByPreferred)
        {
            var airportGroupModel = new AirportGroupModel()
                                        {
                                            AirportGroups = new List<AirportGroup>(),
                                            Airports = new List<Airport>()
                                        };
            airportGroupModel.Airports = this.airportRepository.GetAll().ToList(); ;
            var airportGroupsCopy = new List<AirportGroup>();
            foreach (var airportGroup in this.airportGroupRepository.GetAll().ToList())
            {
                airportGroupsCopy.Add(airportGroup.ShallowCopy());
            }

            if (groupByAirportGroup)
            {
                this.GroupAirportsByAirportGroup(airportGroupsCopy, airportGroupModel, sortByPreferred);
            }

            if (sortByPreferred)
            {
                airportGroupsCopy = airportGroupsCopy.OrderByDescending(o => o.PreferredGroup).ThenBy(o => o.Name).ToList();
                airportGroupModel.Airports = airportGroupModel.Airports.OrderByDescending(o => o.PreferredAirport).ToList();
            }
            else
            {
                airportGroupsCopy = airportGroupsCopy.OrderBy(o => o.Name).ToList();
            }
            airportGroupModel.AirportGroups.AddRange(airportGroupsCopy);

            return airportGroupModel;
        }

        public void GroupAirportsByAirportGroup(List<AirportGroup> airportGroups, AirportGroupModel airportGroupModel, bool sortByPreferred)
        {
            var airports = airportGroupModel.Airports;
            foreach (var airportGroup in airportGroups.OrderBy(ag => ag.Id))
            {
                var airportGroupAirport =
                    this.airportGroupAirportRepository.FindBy(agAirport =>
                                                                agAirport.AirportGroupID == airportGroup.Id);
                var groupAirports =
                    airports.Where(newAirport => airportGroupAirport.Any(aga => aga.AirportID == newAirport.Id)
                        && !airportGroups.Any(ag => ag.Airports.Any(exAirport => exAirport.Id == newAirport.Id)));
                if (sortByPreferred)
                {
                    groupAirports = groupAirports.OrderByDescending(o => o.PreferredAirport).ThenBy(o => o.Name);
                }

                airportGroup.Airports = groupAirports.ToList();
            }
            var airportGroupAirports = this.airportGroupAirportRepository.GetAll().ToList();
            airportGroupModel.Airports = airports.Where(a =>
                                                    airportGroupAirports.Count(aga =>
                                                        aga.AirportID == a.Id) == 0).ToList();
        }

        public List<AirportResortGroupModel> GetAirportResortGroups()
        {
            var airportResortGroups = new List<AirportResortGroupModel>();

            var airportGroups = this.GetAirportGroups();
            var airportGroupResorts = this.GetAirportGroupResorts();
            var resorts = this.resortRepository.GetAll().ToList();

            foreach (var airportGroup in airportGroups)
            {
                IEnumerable<AirportGroupResort> selectedAirportGroupResorts =
                    airportGroupResorts.Where(agr => agr.AirportGroupId == airportGroup.Id).OrderBy(agr => agr.GeographyLevel3Id);

                var firstAirportGroupResort = selectedAirportGroupResorts.FirstOrDefault() ?? new AirportGroupResort() { GeographyLevel1Id = 0 };

                var airportResortGroup = new AirportResortGroupModel()
                {
                    AirportGroupId = airportGroup.Id,
                    AirportGroupName = airportGroup.Name,
                    DisplayOnSearch = airportGroup.DisplayOnSearch,
                    AssociatedGeographyLevel1Id = firstAirportGroupResort.GeographyLevel1Id,
                    Resorts = new List<AirportResortGroupResortModel>()
                };

                foreach (var selectedAirportGroupResort in selectedAirportGroupResorts)
                {
                    var resort = resorts.FirstOrDefault(r => r.Id == selectedAirportGroupResort.GeographyLevel3Id);
                    if (resort != null)
                    {
                        var airportResortGroupResort = new AirportResortGroupResortModel()
                        {
                            Id = selectedAirportGroupResort.GeographyLevel3Id,
                            GeographyLevel1Id = selectedAirportGroupResort.GeographyLevel1Id,
                            Name = resort.Name,
                            Code = resort.Code,
                        };
                        airportResortGroup.Resorts.Add(airportResortGroupResort);
                    }
                }

                airportResortGroups.Add(airportResortGroup);
            }

            return airportResortGroups;
        }

        /// <summary>
        /// Gets the airport by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// An Airport with the matching ID
        /// </returns>
        public Airport GetAirportById(int id)
        {
            return this.airportRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets a list of all airports, filters out airports that are not valid for this brand, as well as adding in valid departure airports
        /// </summary>
        /// <returns>
        /// All Airports
        /// </returns>
        public List<Airport> GetBrandAirports()
        {
            var cachekey = "brand_filtered_airports";
            var result = HttpRuntime.Cache[cachekey] as List<Airport>;
            if (result == null)
            {
                lock (CacheLockObject)
                {
                    result = HttpRuntime.Cache[cachekey] as List<Airport>;
                    if (result == null)
                    {
                        result = new List<Airport>();

                        var airports = this.airportRepository.GetAll().ToList();
                        int? brandId = this.siteService.GetSite(HttpContext.Current)?.BrandId;
                        var brandGeographylevel3SIds =
                            this.brandGeographyRepository.FindBy(x => x.BrandID == brandId)
                                .Select(bg => bg.Geographylevel3Id);
                        var brandDestinationAirports =
                            airports.Where(a => a.Resorts.Any(r => brandGeographylevel3SIds.Contains(r.Id))).ToList();

                        // Get airports that arent in valid brand resorts, but set a flag so we dont display them in the destiantion dropdown
                        var departureAirports =
                            airports.Where(
                                a =>
                                !a.Resorts.Any(r => brandGeographylevel3SIds.Contains(r.Id))
                                && a.Type.ToLower().Contains("departure")).ToList();
                        departureAirports.ForEach(a => a.BrandValidAirport = false);

                        result.AddRange(departureAirports);
                        result.AddRange(brandDestinationAirports);

                        HttpRuntime.Cache.Insert(cachekey, result, null, DateTime.Now.AddHours(12), TimeSpan.Zero);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the route availabilities.
        /// </summary>
        /// <returns>All route Availability</returns>
        public List<RouteAvailabilityModel> GetRouteAvailabilities()
        {
            var cachekey = "route_availbilities";

            var result = HttpRuntime.Cache[cachekey] as List<RouteAvailabilityModel>;
            if (result == null)
            {
                lock (CacheLockObject)
                {
                    result = HttpRuntime.Cache[cachekey] as List<RouteAvailabilityModel>;
                    if (result == null)
                    {
                        result = this.BuildModel();
                        var onRemove = new CacheItemRemovedCallback(this.RebuildCache);

                        ////, CacheItemPriority.Default, onRemove
                        HttpRuntime.Cache.Insert(cachekey, result, null, DateTime.Now.AddHours(12), TimeSpan.Zero);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the route availability by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A single route availability</returns>
        public RouteAvailability GetRouteAvailabilityById(int id)
        {
            return this.routeAvailabilityRepository.GetSingle(id);
        }

        /// <summary>
        /// Builds the model.
        /// </summary>
        /// <returns>
        /// A route availability model
        /// </returns>
        private List<RouteAvailabilityModel> BuildModel()
        {
            var departureAirports =
                this.airportRepository.GetAll()
                    .Where(airport => (airport.Type == "Departure and Arrival" || airport.Type == "Departure Only") && airport.PreferredAirport.GetValueOrDefault())                    
                    .Select(x => x.Id);

            var departureAirportGroups =
                this.airportGroupRepository.FindBy(
                    airport => airport.Type.Contains("Departure"))
                    .Select(x => x.Id);

            var routeAvailabilities = this.routeAvailabilityRepository.GetAll().ToList();

            var routeModels = new List<RouteAvailabilityModel>();

            foreach (int departureAirport in departureAirports)
            {
                var routeArrivalAirports =
                    routeAvailabilities.Where(a => a.DepartureAirportID == departureAirport).ToList();
                var model = this.BuildModel(routeArrivalAirports);
                model.DepartureAirportId = departureAirport;
                routeModels.Add(model);
            }

            foreach (int departureAirportGroup in departureAirportGroups)
            {
                var routeArrivalAirports =
                    routeAvailabilities.Where(a => a.AirportGroupID == departureAirportGroup).ToList();
                var model = this.BuildModel(routeArrivalAirports);
                model.DepartureAirportGroupId = departureAirportGroup;
                routeModels.Add(model);
            }

            return routeModels;
        }

        /// <summary>
        /// Builds the model.
        /// </summary>
        /// <param name="routeArrivalAirports">The route arrival airports.</param>
        /// <returns>A route Availability Model</returns>
        private RouteAvailabilityModel BuildModel(List<RouteAvailability> routeArrivalAirports)
        {
            var brandId = this.siteService.GetSite(HttpContext.Current) != null ? this.siteService.GetSite(HttpContext.Current).BrandId : 0;
            var brand = this.brandRepository.GetBrandWithGeography(brandId);
            var routeArrivalAirportIds = routeArrivalAirports.Select(x => x.ArrivalAirportID).ToList();

            var arrivalAirports = this.airportRepository.FindBy(a => routeArrivalAirportIds.Contains(a.Id));

            var arrivalResortIds = arrivalAirports.SelectMany(a => a.Resorts.Select(r => r.Id)).ToList();

            var arrivalResorts =
                this.resortRepository.FindBy(r => arrivalResortIds.Contains(r.Id))
                    .ToList();

            var arrivalRegions =
                arrivalResorts.GroupBy(resort => resort.RegionID)
                    .Select(
                        region =>
                        new RouteRegionModel
                        {
                            RegionId = region.Key,
                            ResortIds =
                                    region.ToList()
                                    .GroupBy(resort => resort.Id)
                                    .Select(resort => resort.Key)
                                    .ToList()
                        })
                    .ToList();

            var routeModel = new RouteAvailabilityModel()
            {
                ArrivalAirportIDs = routeArrivalAirportIds,
                ArrivalRegions = arrivalRegions
            };

            return routeModel;
        }

        /// <summary>
        /// Function Called When items fall out of cache, used to re-add mandatory items to cache.
        /// </summary>
        /// <param name="k">The k.</param>
        /// <param name="v">The v.</param>
        /// <param name="r">The r.</param>
        private void RebuildCache(string k, object v, CacheItemRemovedReason r)
        {
            this.GetRouteAvailabilities();
        }
    }
}