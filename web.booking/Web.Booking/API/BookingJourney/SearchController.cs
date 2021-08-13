namespace Web.Booking.API.BookingJourney
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Interfaces;
    using Web.Booking.Models.Application;
    using Web.Template.Application.BookingAdjustment.Models;
    using Web.Template.Application.Interfaces.BookingAdjustment;
    using Web.Template.Application.Interfaces.Search;
    using Web.Template.Application.Search.SearchModels;

    /// <summary>
    ///     Controller used to carry out searches
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class SearchController : ApiController
    {
        /// <summary>
        /// The booking adjustment service
        /// </summary>
        private readonly IBookingAdjustmentService bookingAdjustmentService;

        /// <summary>
        ///     The search model factory
        /// </summary>
        private readonly ISearchModelFactory searchModelFactory;

        /// <summary>
        ///     The search service
        /// </summary>
        private readonly ISearchService searchService;

        /// <summary>
        /// The extra search service
        /// </summary>
        private readonly IExtraSearchService extraSearchService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController" /> class.
        /// </summary>
        /// <param name="searchService">The search service.</param>
        /// <param name="searchModelFactory">The search model factory.</param>
        /// <param name="bookingAdjustmentService">The booking adjustment service.</param>
        /// <param name="extraSearchService">The extra search service.</param>
        public SearchController(
            ISearchService searchService,
            ISearchModelFactory searchModelFactory,
            IBookingAdjustmentService bookingAdjustmentService,
            IExtraSearchService extraSearchService)
        {
            this.searchService = searchService;
            this.searchModelFactory = searchModelFactory;
            this.bookingAdjustmentService = bookingAdjustmentService;
            this.extraSearchService = extraSearchService;
        }

        /// <summary>
        /// Search for booking adjustments.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <returns>The result</returns>
        [Route("searchapi/bookingadjustment")]
        public IBookingAdjustmentSearchReturn BookingAdjustmentSearch([FromBody] BookingAdjustmentSearchModel searchModel)
        {
            return this.bookingAdjustmentService.Search(searchModel);
        }

        /// <summary>
        /// Flights the plus hotel search.
        /// </summary>
        /// <param name="departureType">Type of the departure.</param>
        /// <param name="departureId">The departure identifier.</param>
        /// <param name="arrivalType">Type of the arrival.</param>
        /// <param name="arrivalId">The arrival identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="rooms">The rooms.</param>
        /// <param name="adults">The adults.</param>
        /// <param name="children">The children.</param>
        /// <param name="infants">The infants.</param>
        /// <param name="childAges">The child ages.</param>
        /// <param name="packageSearch">if set to <c>true</c> [package search].</param>
        /// <param name="oneway">if set to <c>true</c> [one way].</param>
        /// <param name="direct">if set to <c>true</c> [direct].</param>
        /// <param name="mealBasisId">The meal basis identifier.</param>
        /// <param name="minRating">The minimum rating.</param>
        /// <param name="flightClassId">The flight class identifier.</param>
        /// <returns>Search results</returns>
        [Route("searchapi/flightplushotel/{departureType}/{departureId}/{arrivalType}/{arrivalId}/{date}/{duration}/{rooms}/{adults}/{children}/{infants}/{childAges}")]
        [HttpGet]
        public async Task<SearchResponseViewModel> FlightPlusHotelSearch(
            LocationType departureType,
            int departureId,
            LocationType arrivalType,
            int arrivalId,
            DateTime date,
            int duration,
            int rooms,
            string adults,
            string children,
            string infants,
            string childAges,
            [FromUri] bool packageSearch = false,
            [FromUri] bool oneway = false,
            [FromUri] bool direct = false,
            [FromUri] int mealBasisId = 0,
            [FromUri] int minRating = 0,
            [FromUri] int flightClassId = 0)
        {
            ISearchModel searchModel = this.searchModelFactory.Create(
                arrivalId, 
                arrivalType, 
                date, 
                duration, 
                SearchMode.FlightPlusHotel, 
                rooms, 
                adults, 
                children, 
                infants, 
                childAges, 
                departureId, 
                departureType, 
                mealBasisId: mealBasisId, 
                minRating: minRating, 
                oneWay: oneway, 
                direct: direct,
                isPackageSearch: packageSearch,
                flightClassId: flightClassId);

            List<IResultsModel> searchReturn = await this.searchService.Search(searchModel);

            this.Request.RegisterForDispose(this.searchService);

            SearchResponseViewModel searchResponseViewModel = this.SetupSearchResponseViewModel(searchReturn);
            return searchResponseViewModel;
        }

        /// <summary>
        /// Performs a flight search
        /// </summary>
        /// <param name="departureType">Type of the departure.</param>
        /// <param name="departureID">The departure identifier.</param>
        /// <param name="arrivalType">Type of the arrival.</param>
        /// <param name="arrivalID">The arrival identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="adults">The adults.</param>
        /// <param name="children">The children.</param>
        /// <param name="infants">The infants.</param>
        /// <param name="childAges">a string containing child ages split by _</param>
        /// <param name="oneway">if set to <c>true</c> [oneway].</param>
        /// <param name="direct">if set to <c>true</c> [direct].</param>
        /// <param name="flightClassId">The flight class identifier.</param>
        /// <returns>The search response</returns>
        [Route("searchapi/flight/{departureType}/{departureID}/{arrivalType}/{arrivalID}/{date}/{duration}/{adults}/{children}/{infants}/{childAges}")]
        [HttpGet]
        public async Task<SearchResponseViewModel> FlightSearch(
            LocationType departureType, 
            int departureID, 
            LocationType arrivalType, 
            int arrivalID, 
            DateTime date, 
            int duration, 
            int adults, 
            int children, 
            int infants, 
            string childAges, 
            [FromUri] bool oneway = false,
            [FromUri] bool direct = false,
            [FromUri] int flightClassId = 0)
        {
            ISearchModel searchModel = this.searchModelFactory.Create(
                arrivalID, 
                arrivalType, 
                date, 
                duration, 
                SearchMode.Flight, 
                adults, 
                children, 
                infants, 
                childAges, 
                departureID, 
                departureType, 
                oneWay: oneway, 
                direct: direct,
                flightClassId: flightClassId);

            List<IResultsModel> searchReturn = await this.searchService.Search(searchModel);
            this.Request.RegisterForDispose(this.searchService);

            SearchResponseViewModel searchResponseViewModel = this.SetupSearchResponseViewModel(searchReturn);
            return searchResponseViewModel;
        }

        /// <summary>
        ///     Performs a hotel search
        /// </summary>
        /// <param name="arrivalType">Type of the arrival.</param>
        /// <param name="arrivalID">The arrival identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="rooms">The rooms.</param>
        /// <param name="adults">The adults.</param>
        /// <param name="children">The children.</param>
        /// <param name="infants">The infants.</param>
        /// <param name="childAges">The child ages.</param>
        /// <param name="mealBasisID">The meal basis identifier.</param>
        /// <param name="minRating">The minimum rating.</param>
        /// <returns>The search response</returns>
        [Route("searchapi/hotel/{arrivalType}/{arrivalID}/{date}/{duration}/{rooms}/{adults}/{children}/{infants}/{childAges}")]
        [HttpGet] // ?{mealbasis}{minrating}
        public async Task<SearchResponseViewModel> HotelSearch(
            LocationType arrivalType, 
            int arrivalID, 
            DateTime date, 
            int duration, 
            int rooms, 
            string adults, 
            string children, 
            string infants, 
            string childAges, 
            [FromUri] int mealBasisID = 0,
            [FromUri] int minRating = 0)
        {
            ISearchModel searchModel = this.searchModelFactory.Create(
                arrivalID, 
                arrivalType, 
                date, 
                duration, 
                SearchMode.Hotel, 
                rooms, 
                adults, 
                children, 
                infants, 
                childAges, 
                mealBasisId: mealBasisID, 
                minRating: minRating);

            List<IResultsModel> searchReturn = await this.searchService.Search(searchModel);
            this.Request.RegisterForDispose(this.searchService);

            SearchResponseViewModel searchResponseViewModel = this.SetupSearchResponseViewModel(searchReturn);
            return searchResponseViewModel;
        }

        /// <summary>
        /// Performs a package search
        /// </summary>
        /// <param name="departureAirportId">The departure airport identifier.</param>
        /// <param name="arrivalType">Type of the arrival.</param>
        /// <param name="arrivalId">The arrival identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="rooms">The rooms.</param>
        /// <param name="adults">The adults.</param>
        /// <param name="children">The children.</param>
        /// <param name="infants">The infants.</param>
        /// <param name="childAges">The child ages.</param>
        /// <param name="mealBasisId">The meal basis identifier.</param>
        /// <param name="minRating">The minimum rating.</param>
        /// <param name="flightClassId">The flight class identifier.</param>
        /// <returns>The search response</returns>
        [Route("searchapi/package/{departureAirportId}/{arrivalType}/{arrivalId}/{date}/{duration}/{rooms}/{adults}/{children}/{infants}/{childAges}")]
        [HttpGet] // ?{mealbasisId}{minrating}
        public async Task<SearchResponseViewModel> PackageSearch(
            int departureAirportId,
            LocationType arrivalType, 
            int arrivalId, 
            DateTime date, 
            int duration, 
            int rooms, 
            string adults, 
            string children, 
            string infants, 
            string childAges, 
            [FromUri] int mealBasisId = 0,
            [FromUri] int minRating = 0,
            [FromUri] int flightClassId = 0)
        {
            ISearchModel searchModel = this.searchModelFactory.Create(
                arrivalId, 
                arrivalType, 
                date, 
                duration, 
                SearchMode.FlightPlusHotel, 
                rooms, 
                adults, 
                children, 
                infants, 
                childAges,
                departureAirportId,
                LocationType.Airport,
                isPackageSearch: true,
                mealBasisId: mealBasisId,
                minRating: minRating,
                flightClassId: flightClassId);

            List<IResultsModel> searchReturn = await this.searchService.Search(searchModel);
            this.Request.RegisterForDispose(this.searchService);

            SearchResponseViewModel searchResponseViewModel = this.SetupSearchResponseViewModel(searchReturn);
            return searchResponseViewModel;
        }

        /// <summary>
        /// Performs a transfer search
        /// </summary>
        /// <param name="departureType">Type of the departure.</param>
        /// <param name="departureID">The departure identifier.</param>
        /// <param name="arrivalType">Type of the arrival.</param>
        /// <param name="arrivalID">The arrival identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="rooms">The rooms.</param>
        /// <param name="adults">The adults.</param>
        /// <param name="children">The children.</param>
        /// <param name="infants">The infants.</param>
        /// <param name="childAges">The child ages.</param>
        /// <param name="departureTime">The departure time.</param>
        /// <param name="returnTime">The return time.</param>
        /// <param name="oneway">if set to <c>true</c> [one way].</param>
        /// <param name="direct">if set to <c>true</c> [direct].</param>
        /// <param name="mealBasisId">The meal basis identifier.</param>
        /// <param name="minRating">The minimum rating.</param>
        /// <returns>Search results</returns>
        [Route("searchapi/transfer/{departureType}/{departureID}/{arrivalType}/{arrivalID}/{date}/{departureTime}/{duration}/{returnTime}/{rooms}/{adults}/{children}/{infants}/{childAges}")]
        [HttpGet]
        public async Task<SearchResponseViewModel> TransferSearch(
            LocationType departureType, 
            int departureID, 
            LocationType arrivalType, 
            int arrivalID, 
            DateTime date, 
            string departureTime,
            int duration,
            string returnTime,
            int rooms, 
            int adults, 
            int children, 
            int infants, 
            string childAges,
            bool oneway = false, 
            bool direct = false, 
            int mealBasisId = 0, 
            int minRating = 0)
        {
            ISearchModel searchModel = this.searchModelFactory.Create(
                arrivalID, 
                arrivalType, 
                date, 
                duration, 
                SearchMode.Transfer, 
                adults, 
                children, 
                infants, 
                childAges, 
                departureID, 
                departureType, 
                mealBasisId: mealBasisId, 
                minRating: minRating, 
                oneWay: oneway, 
                direct: direct, 
                departureTime: departureTime, 
                returnTime: returnTime);

            List<IResultsModel> searchReturn = await this.searchService.Search(searchModel);
            this.Request.RegisterForDispose(this.searchService);


            SearchResponseViewModel searchResponseViewModel = this.SetupSearchResponseViewModel(searchReturn);
            return searchResponseViewModel;
        }

        /// <summary>
        /// Extras the basket search.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="extraId">The extra identifier.</param>
        /// <param name="extraGroupId">The extra group identifier.</param>
        /// <param name="extraTypes">The extra types.</param>
        /// <returns>The search response</returns>
        [Route("searchapi/extra/basket/{basketToken}/{extraId}/{extraGroupId}/{extraTypes}")]
        [HttpGet]
        public async Task<SearchResponseViewModel> ExtraBasketSearch(
            string basketToken,
            int extraId,
            int extraGroupId,
            string extraTypes)
        {
            var extraBasketSearchModel  = new ExtraBasketSearchModel()
                                              {
                                                  BasketToken = basketToken,
                                                  ExtraId = extraId,
                                                  ExtraGroupId = extraGroupId,
                                                  ExtraTypes = new List<int>()
                                              };
            foreach (string extraType in extraTypes.Split(','))
            {
                extraBasketSearchModel.ExtraTypes.Add(int.Parse(extraType));
            }

            IResultsModel searchReturn = await this.extraSearchService.SearchFromBasket(extraBasketSearchModel);

            SearchResponseViewModel searchResponseViewModel = this.SetupSearchResponseViewModel(new List<IResultsModel> { searchReturn });
            return searchResponseViewModel;
        }
 

        /// <summary>
        /// Setups the search response view model.
        /// </summary>
        /// <param name="searchReturn">The search return.</param>
        /// <returns>The SearchResponseViewModel.</returns>
        private SearchResponseViewModel SetupSearchResponseViewModel(List<IResultsModel> searchReturn)
        {
            SearchResponseViewModel searchResponseViewModel = new SearchResponseViewModel
                                                                    {
                                                                        Success = true,
                                                                        ResultCounts = new Dictionary<string, int>(),
                                                                        ResultTokens = new Dictionary<string, string>(),
                                                                        Warnings = new List<string>()
                                                                    };
            if (searchReturn.Count > 0)
            {
                foreach (IResultsModel resultModel in searchReturn)
                {
                    searchResponseViewModel.Success = searchResponseViewModel.Success && resultModel.ResultsCollection.Count > 0;
                    searchResponseViewModel.ResultCounts.Add(resultModel.SearchMode.ToString(), resultModel.ResultsCollection.Count);
                    searchResponseViewModel.ResultTokens.Add(resultModel.SearchMode.ToString(), resultModel.ResultToken);
                    searchResponseViewModel.Warnings.AddRange(resultModel.WarningList);
                }
            }
            else
            {
                searchResponseViewModel.Success = false;
            }

            return searchResponseViewModel;
        }
    }
}