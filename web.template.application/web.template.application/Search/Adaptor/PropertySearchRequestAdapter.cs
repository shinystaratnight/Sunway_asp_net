namespace Web.Template.Application.Search.Adaptor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using iVectorConnectInterface.Interfaces;
    using iVectorConnectInterface.Property;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Search;
    using Web.Template.Application.IVectorConnect.Requests;
    using Web.Template.Application.Search.SearchModels;
    using Web.Template.Application.Support;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Geography;

    using ivci = iVectorConnectInterface;

    /// <summary>
    ///     Class that builds Property search requests
    /// </summary>
    /// <seealso cref="ISearchRequestAdapter" />
    public class PropertySearchRequestAdapter : ISearchRequestAdapter
    {
        /// <summary>
        ///     The configuration settings
        /// </summary>
        private readonly Configuration configurationSettings;

        /// <summary>
        /// The connect login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory connectLoginDetailsFactory;

        /// <summary>
        ///     The geography grouping repository
        /// </summary>
        private readonly IGeographyGroupingRepository geographyGroupingRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertySearchRequestAdapter" /> class.
        /// </summary>
        /// <param name="geographyGroupingRepository">The geography grouping repository.</param>
        /// <param name="connectLoginDetailsFactory">The connect login details factory.</param>
        public PropertySearchRequestAdapter(IGeographyGroupingRepository geographyGroupingRepository,
            IConnectLoginDetailsFactory connectLoginDetailsFactory)
        {
            this.configurationSettings = new Configuration();
            this.geographyGroupingRepository = geographyGroupingRepository;
            this.connectLoginDetailsFactory = connectLoginDetailsFactory;
        }

        /// <summary>
        ///     Gets the type of the request.
        /// </summary>
        /// <value>
        ///     The type of the request.
        /// </value>
        public Type ResponseType => typeof(SearchResponse);

        /// <summary>
        /// Creates a search connect search request using a WebTemplate search model.
        /// </summary>
        /// <param name="searchmodel">The search model.</param>
        /// <param name="context">The context.</param>
        /// <returns>A Connect Property search request</returns>
        public iVectorConnectRequest Create(ISearchModel searchmodel, HttpContext context)
        {
            SearchRequest connectRequest = this.SetupConnectRequest(searchmodel, context);

            this.SetupSearchRooms(searchmodel, connectRequest);

            this.SetArrivalValues(searchmodel, connectRequest);

            if (searchmodel.SearchMode == SearchMode.FlightPlusHotel)
            {
                this.SetupFlightDetails(searchmodel, connectRequest);
            }

            return connectRequest;
        }

        /// <summary>
        ///     Sets the arrival values.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <param name="connectRequest">The connect request.</param>
        private void SetArrivalValues(ISearchModel searchModel, SearchRequest connectRequest)
        {
            switch (searchModel.ArrivalType)
            {
                case LocationType.Airport:
                    connectRequest.AirportID = searchModel.ArrivalID;
                    break;

                case LocationType.AirportGroup:
                    connectRequest.AirportGroupID = searchModel.ArrivalID;
                    if (searchModel.SecondaryArrivalIDs != null)
                    {
                        connectRequest.Resorts.AddRange(searchModel.SecondaryArrivalIDs);
                    }
                    break;
                case LocationType.Resort:
                    connectRequest.Resorts.Add(searchModel.ArrivalID);
                    break;

                case LocationType.Region:
                    connectRequest.RegionID = searchModel.ArrivalID;
                    break;

                case LocationType.Property:
                    connectRequest.PropertyReferenceID = searchModel.ArrivalID;
                    break;

                case LocationType.GeoCode:
                    connectRequest.Longitude = searchModel.ArrivalLongitude;
                    connectRequest.Latitude = searchModel.ArrivalLatitude;
                    connectRequest.Radius = searchModel.ArrivalRadius;
                    break;

                case LocationType.GeographyGroup:

                    ////Todo
                    ////GeographyGrouping geographygroups = this.geographyGroupingRepository.GetGeographyGroupById(searchModel.ArrivalID);
                    ////if (geographygroups != null)
                    ////{
                    ////    connectRequest.Resorts.AddRange(geographygroups.Geographies.Select(geo => geo.Level3Id).ToList());
                    ////}
                    break;
            }
        }

        /// <summary>
        /// Setups the connect request.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A bare bones connect request
        /// </returns>
        private SearchRequest SetupConnectRequest(ISearchModel searchModel, HttpContext context)
        {
            var connectRequest = new SearchRequest
                                     {
                                         LoginDetails = this.connectLoginDetailsFactory.Create(context), 
                                         SearchMode = searchModel.SearchMode.ToString(), 
                                         ArrivalDate = searchModel.DepartureDate, 
                                         Duration = searchModel.Duration, 
                                         MealBasisID = searchModel.MealBasisID, 
                                         MinStarRating = searchModel.MinRating, 
                                         RoomRequests = new List<SearchRequest.RoomRequest>()
                                     };
            return connectRequest;
        }

        /// <summary>
        ///     Setups the flight details.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <param name="connectRequest">The connect request.</param>
        private void SetupFlightDetails(ISearchModel searchModel, SearchRequest connectRequest)
        {
            var flightDetails = new SearchRequest.FlightDetailsDef { FlightAndHotel = true };

            switch (searchModel.DepartureType)
            {
                case LocationType.Airport:
                    flightDetails.DepartureAirportID = searchModel.DepartureID;
                    break;

                case LocationType.AirportGroup:
                    flightDetails.DepartureAirportGroupID = searchModel.DepartureID;
                    break;
            }

            connectRequest.FlightDetails = flightDetails;
        }

        /// <summary>
        ///     Setups the search rooms.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <param name="connectRequest">The connect request.</param>
        private void SetupSearchRooms(ISearchModel searchModel, SearchRequest connectRequest)
        {
            if (searchModel.Rooms != null)
            {
                foreach (Room room in searchModel.Rooms)
                {
                    var guestConfiguration = new ivci.Support.GuestConfiguration { Adults = room.Adults, Children = room.Children, Infants = room.Infants, ChildAges = room.ChildAges };

                    var requestRoom = new SearchRequest.RoomRequest { GuestConfiguration = guestConfiguration };
                    connectRequest.RoomRequests.Add(requestRoom);
                }
            }
        }
    }
}