namespace Web.Template.Application.Search.Adaptor
{
    using System;
    using System.Linq;
    using System.Web;

    using iVectorConnectInterface.Flight;
    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Search;
    using Web.Template.Application.IVectorConnect.Requests;
    using Web.Template.Application.Support;

    using ivci = iVectorConnectInterface;

    /// <summary>
    ///     Class that builds Property search requests
    /// </summary>
    /// <seealso cref="ISearchRequestAdapter" />
    public class FlightSearchRequestAdaptor : ISearchRequestAdapter
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
        /// Initializes a new instance of the <see cref="FlightSearchRequestAdaptor" /> class.
        /// </summary>
        /// <param name="connectLoginDetailsFactory">The connect login details factory.</param>
        public FlightSearchRequestAdaptor(IConnectLoginDetailsFactory connectLoginDetailsFactory)
        {
            this.connectLoginDetailsFactory = connectLoginDetailsFactory;
            this.configurationSettings = new Configuration();
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
        /// <returns>
        /// A Connect Property search request
        /// </returns>
        public iVectorConnectRequest Create(ISearchModel searchmodel, HttpContext context)
        {
            SearchRequest connectRequest = this.SetupConnectRequest(searchmodel, context);

            this.SetArrivalValues(searchmodel, connectRequest);
            this.SetDepartureValues(searchmodel, connectRequest);

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
                    connectRequest.ArrivalAirportID = searchModel.ArrivalID;
                    break;

                case LocationType.AirportGroup:
                    connectRequest.ArrivalAirportGroupID = searchModel.ArrivalID;
                    break;

                case LocationType.Resort:
                    connectRequest.Resorts.Add(searchModel.ArrivalID);
                    break;

                case LocationType.Region:
                    connectRequest.RegionID = searchModel.ArrivalID;
                    break;
            }
        }

        /// <summary>
        ///     Sets the arrival values.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <param name="connectRequest">The connect request.</param>
        private void SetDepartureValues(ISearchModel searchModel, SearchRequest connectRequest)
        {
            switch (searchModel.DepartureType)
            {
                case LocationType.Airport:
                    connectRequest.DepartureAirportID = searchModel.DepartureID;
                    break;

                case LocationType.AirportGroup:
                    connectRequest.DepartureAirportGroupID = searchModel.DepartureID;
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
                                         DepartureDate = searchModel.DepartureDate, 
                                         Duration = searchModel.Duration, 
                                         OneWay = searchModel.OneWay, 
                                         AllowMultisectorFlights = !searchModel.Direct, 
                                         FlightAndHotel = searchModel.SearchMode == SearchMode.FlightPlusHotel,
                                         FlightClassID = searchModel.FlightClassId
                                     };
            if (searchModel.Rooms != null)
            {
                connectRequest.GuestConfiguration = new ivci.Support.GuestConfiguration
                                                        {
                                                            Adults = searchModel.Rooms.Sum(room => room.Adults), 
                                                            Children = searchModel.Rooms.Sum(room => room.Children), 
                                                            Infants = searchModel.Rooms.Sum(room => room.Infants), 
                                                            ChildAges = searchModel.Rooms.SelectMany(room => room.ChildAges).ToList()
                                                        };
            }

            return connectRequest;
        }
    }
}