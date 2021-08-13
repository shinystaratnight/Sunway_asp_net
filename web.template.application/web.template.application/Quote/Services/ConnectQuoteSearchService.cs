namespace Web.Template.Application.Quote.Services
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using Interfaces.Quote.Models;

    using iVectorConnectInterface;

    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Quote.Services;
    using Web.Template.Application.IVectorConnect.Requests;
    using Web.Template.Application.Quote.Models;

    /// <summary>
    /// Class ConnectQuoteSearchService.
    /// </summary>
    public class ConnectQuoteSearchService : IQuoteSearchService
    {
        /// <summary>
        /// The connect login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory connectLoginDetailsFactory;

        /// <summary>
        /// The connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory connectRequestFactory;

        /// <summary>
        /// The geography service
        /// </summary>
        private readonly IGeographyService geographyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectQuoteSearchService" /> class.
        /// </summary>
        /// <param name="connectLoginDetailsFactory">The connect login details factory.</param>
        /// <param name="connectRequestFactory">The connect request factory.</param>
        /// <param name="geographyService">The geography service.</param>
        public ConnectQuoteSearchService(
            IConnectLoginDetailsFactory connectLoginDetailsFactory,
            IIVectorConnectRequestFactory connectRequestFactory,
            IGeographyService geographyService)
        {
            this.connectLoginDetailsFactory = connectLoginDetailsFactory;
            this.connectRequestFactory = connectRequestFactory;
            this.geographyService = geographyService;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        /// <param name="quoteSearch">The quote search.</param>
        /// <returns>The Quote Search Return.</returns>
        public IQuoteSearchReturn Search(IQuoteSearch quoteSearch)
        {
            QuoteSearchRequest quoteSearchRequest = this.BuildSearchRequest(quoteSearch);
            QuoteSearchResponse quoteSearchResponse = this.GetResponse(quoteSearchRequest);

            IQuoteSearchReturn quoteSearchReturn = this.ProcessResponse(quoteSearchResponse);
            return quoteSearchReturn;
        }

        /// <summary>
        /// Builds the search request.
        /// </summary>
        /// <param name="quoteSearch">The quote search.</param>
        /// <returns>The Quote Search Request.</returns>
        private QuoteSearchRequest BuildSearchRequest(IQuoteSearch quoteSearch)
        {
            var connectRequest = new QuoteSearchRequest()
                                     {
                                         LoginDetails = this.connectLoginDetailsFactory.Create(HttpContext.Current),
                                         CustomerID = quoteSearch.CustomerId,
                                         TradeContactID = quoteSearch.TradeContactId,
                                         QuoteReference = quoteSearch.QuoteReference,
                                         TradeReference = quoteSearch.TradeReference,
                                         EarliestBookingDate = quoteSearch.EarliestBookingDate.Date,
                                         EarliestBookingTime = quoteSearch.EarliestBookingTime,
                                         LatestBookingDate = quoteSearch.LatestBookingDate.Date,
                                         LatestBookingTime = quoteSearch.LatestBookingTime,
                                         EarliestDepartureDate = quoteSearch.EarliestDepartureDate.Date,
                                         LatestDepartureDate = quoteSearch.LatestDepartureDate.Date,
                                         Duration = quoteSearch.Duration,
                                         BrandIDs = quoteSearch.BrandIds,
                                         Source = quoteSearch.Source
                                     };
            return connectRequest;
        }
 
        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="connectRequestBody">The connect request body.</param>
        /// <returns>The Quote Search Response.</returns>
        private QuoteSearchResponse GetResponse(QuoteSearchRequest connectRequestBody)
        {
            var connectRequest = this.connectRequestFactory.Create(connectRequestBody, HttpContext.Current);
            var quoteResponse = connectRequest.Go<QuoteSearchResponse>(true);
            return quoteResponse;
        }

        /// <summary>
        /// Processes the response.
        /// </summary>
        /// <param name="quoteSearchResponse">The quote search response.</param>
        /// <returns>THe Quote Search Return.</returns>
        private QuoteSearchReturn ProcessResponse(QuoteSearchResponse quoteSearchResponse)
        {
            var quoteSearchReturn = new QuoteSearchReturn { Quotes = new List<IQuote>() };

            foreach (var quote in quoteSearchResponse.Quotes)
            {
                var quoteModel = new Models.Quote()
                                     {
                                        QuoteReference = quote.QuoteReference,
                                        TradeReference = quote.TradeReference,
                                        Status = quote.Status,
                                        AccountStatus = quote.AccountStatus,
                                        LeadCustomerFirstName = quote.LeadCustomerFirstName,
                                        LeadCustomerLastName = quote.LeadCustomerLastName,
                                        TotalPassengers = quote.TotalPax,
                                        BookingDate = quote.BookingDate,
                                        ArrivalDate = quote.ArrivalDate,
                                        Resort = this.geographyService.GetResort(quote.GeographyLevel3ID).Name,
                                        Duration = quote.Duration,
                                        LastReturnDate = quote.LastReturnDate,
                                        TotalPrice = quote.TotalPrice,
                                        TotalCommission = quote.TotalCommission,
                                        Components = new List<IQuoteComponent>()
                                     };
                foreach (var component in quote.ComponentSummary)
                {
                    var quoteComponent = new QuoteComponent()
                                             {
                                                 ComponentType = component.ComponentType,
                                                 Status = component.Status,
                                                 Reference = component.Reference,
                                                 Name = component.hlpComponentName
                                             };
                    quoteModel.Components.Add(quoteComponent);
                }
                quoteSearchReturn.Quotes.Add(quoteModel);
            }
            return quoteSearchReturn;
        }
    }
}
