namespace Web.Booking.API.BookingJourney
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using Intuitive;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Results.ResultModels;
    using Web.Booking.Models.Application;

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
                    case SearchMode.Transfer:
                        searchResultsViewModel = this.SetupTransferResults(results);
                        break;
                    case SearchMode.Extra:
                        searchResultsViewModel = this.SetupExtraResults(results);
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
                                                        "FlightSupplierId",
                                                        flightResult.FlightSupplierId
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
                                                        "OutboundTimeOfDay",
                                                        this.FlightTimeOfDay(flightResult.OutboundFlightDetails.DepartureTime)
                                                    },
                                                    {
                                                        "ReturnDepartureTime",
                                                        flightResult.ReturnFlightDetails.DepartureTime
                                                    },
                                                    {
                                                        "ReturnTimeOfDay",
                                                        this.FlightTimeOfDay(flightResult.ReturnFlightDetails.DepartureTime)
                                                    },
                                                    {
                                                        "MaxStops",
                                                        flightResult.MaxStops
                                                    },
                                                    {
                                                        "FlightSectors",
                                                        flightResult.FlightSectors
                                                    },
                                                    {
                                                        "IncludesSupplierBaggage",
                                                        flightResult.IncludesSupplierBaggage
                                                    },
                                                    {
                                                        "Id",
                                                        flightResult.Id
                                                    },
                                                    {
                                                        "Source",
                                                        flightResult.Source
                                                    },
                                                    {
                                                        "BaggageDescription",
                                                        flightResult.BaggageDescription
                                                    },
                                                    {
                                                        "IncludedBaggageAllowance",
                                                        flightResult.IncludedBaggageAllowance
                                                    },
                                                    {
                                                        "IncludedBaggageWeight",
                                                        flightResult.IncludedBaggageWeight
                                                    },
                                                    {
                                                        "IncludedBaggageText",
                                                        flightResult.IncludedBaggageText
                                                    }
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
                searchResult.ComponentToken = propertyResult.ComponentToken;

                searchResult.SubResults = new List<ISubResult>();
                searchResult.SubResults.AddRange(propertyResult.SubResults);

                searchResult.MetaData = new Dictionary<string, object>
                                            {
                                                { "ArrivalDate", propertyResult.ArrivalDate },
                                                { "Duration", propertyResult.Duration },
                                                { "FacilityFlag", propertyResult.FacilityFlag },
                                                { "GeographyLevel1Id", propertyResult.GeographyLevel1Id },
                                                { "GeographyLevel2Id", propertyResult.GeographyLevel2Id },
                                                { "GeographyLevel3Id", propertyResult.GeographyLevel3Id },
                                                { "OSReference", propertyResult.OSReference },
                                                { "PropertyReferenceId", propertyResult.PropertyReferenceId },
                                                { "Rating", propertyResult.Rating },
                                                { "MainImage", propertyResult.MainImage },
                                                { "Images", propertyResult.Images },
                                                { "Summary", propertyResult.Summary },
                                                { "URL", propertyResult.URL },
                                                { "ProductAttributes", propertyResult.ProductAttributes },
                                                { "ReviewAverageScore", propertyResult.ReviewAverageScore },
                                                { "VideoCode", propertyResult.VideoCode },
                                                { "Latitude", propertyResult.Latitude },
                                                { "Longitude", propertyResult.Longitude }
                                            };

                searchResultsViewModel.SearchResults.Add(searchResult);
            }

            return searchResultsViewModel;
        }

        /// <summary>
        /// Setups the transfer results.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>The search results view model.</returns>
        private SearchResultsViewModel SetupTransferResults(IResultsModel results)
        {
            SearchResultsViewModel searchResultsViewModel = new SearchResultsViewModel();
            searchResultsViewModel.SearchResults = new List<SearchResult>();

            foreach (IResult result in results.ResultsCollection)
            {
                TransferResult transferResult = (TransferResult)result;

                SearchResult searchResult = new SearchResult();

                searchResult.DisplayName = transferResult.Vehicle;
                searchResult.Display = true;
                searchResult.ComponentToken = transferResult.ComponentToken;
                searchResult.Price = transferResult.TotalPrice;

                searchResult.SubResults = new List<ISubResult>();

                searchResult.MetaData = new Dictionary<string, object>
                                            {
                                                { "VehicleName", transferResult.Vehicle },
                                                { "Quantity", transferResult.VehicleQuantity },
                                            };

                searchResultsViewModel.SearchResults.Add(searchResult);
            }

            return searchResultsViewModel;
        }

        /// <summary>
        /// Setups the extra results.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>The search results view model.</returns>
        private SearchResultsViewModel SetupExtraResults(IResultsModel results)
        {
            SearchResultsViewModel searchResultsViewModel = new SearchResultsViewModel();
            searchResultsViewModel.SearchResults = new List<SearchResult>();

            foreach (IResult result in results.ResultsCollection)
            {
                var extraResult = (ExtraResult)result;

                SearchResult searchResult = new SearchResult
                                                {
                                                    DisplayName = extraResult.ExtraName,
                                                    Display = true,
                                                    ComponentToken = extraResult.ComponentToken,
                                                    Price = extraResult.TotalPrice,
                                                    SubResults = new List<ISubResult>()
                                                };
                searchResult.SubResults.AddRange(extraResult.SubResults);

                searchResultsViewModel.SearchResults.Add(searchResult);
            }

            return searchResultsViewModel;
        }

        /// <summary>
        /// Flights the time of day.
        /// </summary>
        /// <param name="flightTime">The flight time.</param>
        /// <returns>System.String.</returns>
        private string FlightTimeOfDay(string flightTime)
        {
            var timeOfDay = "Night";
            if (DateFunctions.InTimeRange(flightTime, "06:00", "11:59"))
            {
                timeOfDay = "Morning";
            }
            else if (DateFunctions.InTimeRange(flightTime, "12:00", "17:59"))
            {
                timeOfDay = "Afternoon";
            }
            else if (DateFunctions.InTimeRange(flightTime, "18:00", "21:59"))
            {
                timeOfDay = "Evening";
            }
            return timeOfDay;
        }
    }
}