namespace Web.Template.API.BookingJourney
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Results.ResultModels;
    using Web.Template.Models.Application;

    /// <summary>
    ///     Controller used to get results
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class ResultController : ApiController
    {
        /// <summary>
        /// The result service
        /// </summary>
        private readonly IResultService resultService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultController"/> class.
        /// </summary>
        /// <param name="resultService">The result service.</param>
        public ResultController(IResultService resultService)
        {
            this.resultService = resultService;
        }

        /// <summary>
        /// Searches the results.
        /// </summary>
        /// <param name="searchToken">The search token.</param>
        /// <returns>The search results</returns>
        [Route("api/searchresults")]
        [HttpGet]
        public SearchResultsViewModel SearchResults([FromUri] string searchToken)
        {
            SearchResultsViewModel searchResultsViewModel = new SearchResultsViewModel();

            searchToken = searchToken.Replace(" ", "+");
            IResultsModel results = this.resultService.RetrieveResults(searchToken);

            if (results != null)
            {
                switch (results.SearchMode)
                {
                    case SearchMode.Hotel:
                        searchResultsViewModel = this.SetupHotelResults(results);
                        break;
                    case SearchMode.Flight:
                        searchResultsViewModel = this.SetupFlightResults(results);
                        break;
                    default:
                        break;
                }
            }

            return searchResultsViewModel;
        }

        /// <summary>
        /// Setups the flight results.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>The search results view model.</returns>
        private SearchResultsViewModel SetupFlightResults(IResultsModel results)
        {
            SearchResultsViewModel searchResultsViewModel = new SearchResultsViewModel();
            searchResultsViewModel.SearchResults = new List<SearchResult>();

            foreach (IResult result in results.ResultsCollection)
            {
                FlightResult flightResult = (FlightResult)result;

                if (flightResult.ExactMatch)
                {
                    SearchResult searchResult = new SearchResult();
                    searchResult.ComponentToken = flightResult.ComponentToken;
                    searchResult.Display = true;
                    searchResult.Price = flightResult.TotalPrice;

                    searchResult.MetaData = new Dictionary<string, object>
                                                {
                                                    {
                                                        "DepartureAirportId",
                                                        flightResult.DepartureAirportId
                                                    },
                                                    {
                                                        "ArrivalAirportId",
                                                        flightResult.ArrivalAirportId
                                                    },
                                                    {
                                                        "FlightCarrierId",
                                                        flightResult.FlightCarrierId
                                                    },
                                                    {
                                                        "OutboundFlightClassId",
                                                        flightResult.OutboundFlightDetails.FlightClassId
                                                    },
                                                    {
                                                        "ReturnFlightClassId",
                                                        flightResult.ReturnFlightDetails.FlightClassId
                                                    },
                                                    {
                                                        "OutboundDepartureDate",
                                                        flightResult.OutboundFlightDetails.DepartureDate
                                                    },
                                                    {
                                                        "OutboundDepartureTime",
                                                        flightResult.OutboundFlightDetails.DepartureTime
                                                    },
                                                    {
                                                        "ReturnDepartureTime",
                                                        flightResult.ReturnFlightDetails.DepartureTime
                                                    },
                                                    { "MaxStops", flightResult.MaxStops },
                                                    {
                                                        "FlightSectors", flightResult.FlightSectors
                                                    },
                                                    {
                                                        "IncludesSupplierBaggage",
                                                        flightResult.IncludesSupplierBaggage
                                                    },
                                                };

                    searchResultsViewModel.SearchResults.Add(searchResult);
                }
            }

            return searchResultsViewModel;
        }

        /// <summary>
        /// Setups the hotel results.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>The search results view model.</returns>
        private SearchResultsViewModel SetupHotelResults(IResultsModel results)
        {
            SearchResultsViewModel searchResultsViewModel = new SearchResultsViewModel();
            searchResultsViewModel.SearchResults = new List<SearchResult>();

            foreach (IResult result in results.ResultsCollection)
            {
                PropertyResult propertyResult = (PropertyResult)result;

                SearchResult searchResult = new SearchResult();

                searchResult.DisplayName = propertyResult.Name;
                searchResult.Display = true;

                searchResult.SubResults = new List<ISubResult>();
                searchResult.SubResults.AddRange(propertyResult.SubResults);

                searchResult.MetaData = new Dictionary<string, object>
                                            {
                                                { "FacilityFlag", propertyResult.FacilityFlag }, 
                                                { "GeographyLevel1Id", propertyResult.GeographyLevel1Id }, 
                                                { "GeographyLevel2Id", propertyResult.GeographyLevel2Id }, 
                                                { "GeographyLevel3Id", propertyResult.GeographyLevel3Id }, 
                                                { "PropertyReferenceId", propertyResult.PropertyReferenceId }, 
                                                { "Rating", propertyResult.Rating }, 
                                                { "MainImage", propertyResult.MainImage }, 
                                                { "Summary", propertyResult.Summary }
                                            };

                searchResultsViewModel.SearchResults.Add(searchResult);
            }

            return searchResultsViewModel;
        }
    }
}