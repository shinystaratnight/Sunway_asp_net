namespace Web.Template.API.BookingJourney
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
    using Web.Template.Models.Application;

    /// <summary>
    ///     Controller used to carry out searches
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class SearchController : ApiController
    {
        /// <summary>
        ///     The search model factory
        /// </summary>
        private readonly ISearchModelFactory searchModelFactory;

        /// <summary>
        ///     The search service
        /// </summary>
        private readonly ISearchService searchService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController" /> class.
        /// </summary>
        /// <param name="searchService">The search service.</param>
        /// <param name="searchModelFactory">The search model factory.</param>
        public SearchController(ISearchService searchService, ISearchModelFactory searchModelFactory)
        {
            this.searchService = searchService;
            this.searchModelFactory = searchModelFactory;
        }

        /// <summary>
        ///     Flights the plus hotel search.
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
        /// <param name="oneway">if set to <c>true</c> [one way].</param>
        /// <param name="direct">if set to <c>true</c> [direct].</param>
        /// <param name="mealBasisId">The meal basis identifier.</param>
        /// <param name="minRating">The minimum rating.</param>
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
            bool oneway = false, 
            bool direct = false, 
            int mealBasisId = 0, 
            int minRating = 0)
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
                direct: direct);

            List<IResultsModel> searchReturn = await this.searchService.Search(searchModel);

            this.Request.RegisterForDispose(this.searchService);

            SearchResponseViewModel searchResponseViewModel = this.SetupSearchResponseViewModel(searchReturn);
            return searchResponseViewModel;
        }

        /// <summary>
        ///     Flights the search.
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
        /// <param name="oneway">if set to <c>true</c> [oneway].</param>
        /// <param name="direct">if set to <c>true</c> [direct].</param>
        /// <param name="childAges">a string containing child ages split by _</param>
        /// <returns></returns>
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
            bool oneway = false, 
            bool direct = false)
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
                direct: direct);

            List<IResultsModel> searchReturn = await this.searchService.Search(searchModel);
            this.Request.RegisterForDispose(this.searchService);

            SearchResponseViewModel searchResponseViewModel = this.SetupSearchResponseViewModel(searchReturn);
            return searchResponseViewModel;
        }

        /// <summary>
        ///     Hotels the search.
        /// </summary>
        /// <param name="arrivalType">Type of the arrival.</param>
        /// <param name="arrivalID">The arrival identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="rooms">The rooms.</param>
        /// <param name="adults">The adults.</param>
        /// <param name="children">The children.</param>
        /// <param name="infants">The infants.</param>
        /// <param name="childages">The childages.</param>
        /// <param name="mealBasisID">The meal basis identifier.</param>
        /// <param name="minRating">The minimum rating.</param>
        /// <returns></returns>
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
            int mealBasisID = 0, 
            int minRating = 0)
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
        /// Flights the plus hotel search.
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
        /// <param name="mealBasisID">The meal basis identifier.</param>
        /// <param name="minRating">The minimum rating.</param>
        /// <returns>Search results</returns>
        [Route("searchapi/transfer/{departureType}/{departureID}/{arrivalType}/{arrivalID}/{date}/{departureTime}/{duration}/{ReturnTime}/{rooms}/{adults}/{children}/{infants}/{childAges}")]
        [HttpGet]
        public async Task<SearchResponseViewModel> TransferSearch(
            LocationType departureType, 
            int departureID, 
            LocationType arrivalType, 
            int arrivalID, 
            DateTime date, 
            int duration, 
            int rooms, 
            int adults, 
            int children, 
            int infants, 
            string childAges, 
            string departureTime, 
            string returnTime, 
            bool oneway = false, 
            bool direct = false, 
            int mealBasisID = 0, 
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
                mealBasisId: mealBasisID, 
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

            foreach (IResultsModel resultModel in searchReturn)
            {
                searchResponseViewModel.Success = searchResponseViewModel.Success && resultModel.ResultsCollection.Count > 0;
                searchResponseViewModel.ResultCounts.Add(resultModel.SearchMode.ToString(), resultModel.ResultsCollection.Count);
                searchResponseViewModel.ResultTokens.Add(resultModel.SearchMode.ToString(), resultModel.ResultToken);
                searchResponseViewModel.Warnings.AddRange(resultModel.WarningList);
            }

            return searchResponseViewModel;
        }
    }
}