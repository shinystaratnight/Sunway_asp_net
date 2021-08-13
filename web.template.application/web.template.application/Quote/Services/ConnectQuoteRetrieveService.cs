namespace Web.Template.Application.Quote.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    using iVectorConnectInterface;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Quote.Adaptors;
    using Web.Template.Application.Interfaces.Quote.Builders;
    using Web.Template.Application.Interfaces.Quote.Processors;
    using Web.Template.Application.Interfaces.Quote.Services;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.IVectorConnect.Requests;
    using Web.Template.Application.Quote.Models;
    using Web.Template.Application.Search.SearchModels;

    using IQuoteRetrieveSearchAdaptor = Web.Template.Application.Interfaces.Quote.Adaptors.IQuoteRetrieveSearchAdaptor;

    /// <summary>
    /// Class ConnectQuoteRetrieveService.
    /// </summary>
    public class ConnectQuoteRetrieveService : IQuoteRetrieveService
    {
        /// <summary>
        /// The basket service
        /// </summary>
        private readonly IBasketService basketService;

        /// <summary>
        /// The connect login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory connectLoginDetailsFactory;

        /// <summary>
        /// The connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory connectRequestFactory;

        /// <summary>
        /// The search service
        /// </summary>
        private readonly ISearchService searchService;

        /// <summary>
        /// The quote retrieve return builder
        /// </summary>
        private readonly IQuoteRetrieveReturnBuilder quoteRetrieveReturnBuilder;

        /// <summary>
        /// The search adaptor
        /// </summary>
        private readonly IQuoteRetrieveSearchAdaptor searchAdaptor;

        /// <summary>
        /// The basket adaptor
        /// </summary>
        private readonly IQuoteRetrieveBasketAdaptor basketAdaptor;

        /// <summary>
        /// The quote retrieve response processor
        /// </summary>
        private readonly IQuoteRetrieveResponseProcessor quoteRetrieveResponseProcessor;


        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectQuoteRetrieveService" /> class.
        /// </summary>
        /// <param name="connectLoginDetailsFactory">The connect login details factory.</param>
        /// <param name="connectRequestFactory">The connect request factory.</param>
        /// <param name="searchService">The search service.</param>
        /// <param name="searchAdaptor">The search adaptor.</param>
        /// <param name="basketService">The basket service.</param>
        /// <param name="quoteRetrieveReturnBuilder">The quote retrieve return builder.</param>
        /// <param name="basketAdaptor">The basket adaptor.</param>
        /// <param name="quoteRetrieveResponseProcessor">The quote retrieve response processor.</param>
        public ConnectQuoteRetrieveService(
            IConnectLoginDetailsFactory connectLoginDetailsFactory,
            IIVectorConnectRequestFactory connectRequestFactory,
            ISearchService searchService,
            IQuoteRetrieveSearchAdaptor searchAdaptor,
            IBasketService basketService,
            IQuoteRetrieveReturnBuilder quoteRetrieveReturnBuilder,
            IQuoteRetrieveBasketAdaptor basketAdaptor,
            IQuoteRetrieveResponseProcessor quoteRetrieveResponseProcessor)
        {
            this.connectLoginDetailsFactory = connectLoginDetailsFactory;
            this.connectRequestFactory = connectRequestFactory;
            this.searchService = searchService;
            this.searchAdaptor = searchAdaptor;
            this.basketService = basketService;
            this.quoteRetrieveReturnBuilder = quoteRetrieveReturnBuilder;
            this.basketAdaptor = basketAdaptor;
            this.quoteRetrieveResponseProcessor = quoteRetrieveResponseProcessor;
        }

        /// <summary>
        /// Retrieves the specified quote reference.
        /// </summary>
        /// <param name="quoteReference">The quote reference.</param>
        /// <returns>The Quote Retrieve Return.</returns>
        public async Task<QuoteRetrieveReturn> Retrieve(string quoteReference)
        {
            QuoteRetrieveRequest quoteRetrieveRequest = this.BuildRetrieveRequest(quoteReference);

            Intuitive.FileFunctions.AddLogEntry("Quote", "RetrieveRequest", Intuitive.Serializer.Serialize(quoteRetrieveRequest).InnerXml);
            QuoteRetrieveResponse quoteRetrieveResponse = this.GetResponse(quoteRetrieveRequest);
            Intuitive.FileFunctions.AddLogEntry("Quote", "RetrieveResponse", Intuitive.Serializer.Serialize(quoteRetrieveResponse).InnerXml);

            QuoteRetrieveReturn quoteRetrieveReturn = await this.ProcessResponse(quoteRetrieveResponse);
            return quoteRetrieveReturn;
        }

        /// <summary>
        /// Builds the retrieve request.
        /// </summary>
        /// <param name="quoteReference">The quote reference.</param>
        /// <returns>The Quote Retrieve Request.</returns>
        private QuoteRetrieveRequest BuildRetrieveRequest(string quoteReference)
        {
            var quoteRetrieveRequest = new QuoteRetrieveRequest()
            {
                LoginDetails = this.connectLoginDetailsFactory.Create(HttpContext.Current),
                QuoteReference = quoteReference,
                RepriceQuote = true
            };
            return quoteRetrieveRequest;
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="quoteRetrieveRequest">The quote retrieve request.</param>
        /// <returns>The Quote Retrieve Response.</returns>
        private QuoteRetrieveResponse GetResponse(QuoteRetrieveRequest quoteRetrieveRequest)
        {
            var connectRequest = this.connectRequestFactory.Create(quoteRetrieveRequest, HttpContext.Current);
            var response = connectRequest.Go<QuoteRetrieveResponse>(true);
            return response;
        }

        /// <summary>
        /// Processes the response.
        /// </summary>
        /// <param name="quoteRetrieveResponse">The quote retrieve response.</param>
        /// <returns>The Quote Retrieve Return.</returns>
        private async Task<QuoteRetrieveReturn> ProcessResponse(QuoteRetrieveResponse quoteRetrieveResponse)
        {
            var quoteRetrieveReturn = new QuoteRetrieveReturn();
            if (quoteRetrieveResponse.ReturnStatus.Success)
            {
                quoteRetrieveReturn.Success = true;
                this.quoteRetrieveReturnBuilder.SetSearchModel(quoteRetrieveResponse);
                this.quoteRetrieveReturnBuilder.SetupComponents(quoteRetrieveResponse);

                var basketToken = this.basketService.CreateBasket().ToString();
                this.quoteRetrieveReturnBuilder.SetBasketToken(basketToken);

                quoteRetrieveReturn = (QuoteRetrieveReturn)this.quoteRetrieveReturnBuilder.GetResult();

                bool hasFlight = quoteRetrieveReturn.QuoteComponentTypes.Contains(ComponentType.Flight);
                bool hasAlternativeFlight = false;

                bool flightSearchRequired = hasFlight && !quoteRetrieveReturn.RepricedComponentTypes.Contains(ComponentType.Flight);

                bool hasHotel = quoteRetrieveReturn.QuoteComponentTypes.Contains(ComponentType.Hotel);
                bool hasAlternativeHotel = false;
                bool hotelSearchRequired = hasHotel && !quoteRetrieveReturn.RepricedComponentTypes.Contains(ComponentType.Hotel);


                if (!(flightSearchRequired && hotelSearchRequired) && (flightSearchRequired || hotelSearchRequired))
                {
                    List<IResultsModel> searchReturn =
                        await
                        this.GetAlternativeResults(quoteRetrieveResponse, flightSearchRequired, hotelSearchRequired);
                    this.quoteRetrieveReturnBuilder.SetSearchResult(searchReturn);
                    quoteRetrieveReturn = (QuoteRetrieveReturn)this.quoteRetrieveReturnBuilder.GetResult();

                    hasAlternativeFlight = flightSearchRequired
                                           && quoteRetrieveReturn.ResultCounts.ContainsKey("Flight")
                                           && quoteRetrieveReturn.ResultCounts["Flight"] > 0;

                    hasAlternativeHotel = hotelSearchRequired
                                            && quoteRetrieveReturn.ResultCounts.ContainsKey("Hotel")
                                            && quoteRetrieveReturn.ResultCounts["Hotel"] > 0;
                }


                if (hasHotel && !hotelSearchRequired && (!hasFlight || !flightSearchRequired || hasAlternativeFlight))
                {
                    foreach (var quoteProperty in quoteRetrieveResponse.Properties)
                    {
                        if (quoteProperty.ComponentRepriced)
                        {
                            IBasketComponent basketComponent = this.basketAdaptor.CreatePropertyComponent(quoteProperty, quoteRetrieveResponse.GuestDetails);
                            this.basketService.AddBasketComponent(basketToken, basketComponent);
                        }
                    }
                }

                if (hasFlight && !flightSearchRequired && (!hasHotel || !hotelSearchRequired || hasAlternativeHotel))
                {
                    foreach (var quoteFlight in quoteRetrieveResponse.Flights)
                    {
                        if (quoteFlight.ComponentRepriced)
                        {
                            IBasketComponent basketComponent = this.basketAdaptor.CreateFlightComponent(quoteFlight, quoteRetrieveResponse.GuestDetails);
                            this.basketService.AddBasketComponent(basketToken, basketComponent);
                        }
                    }
                }

                if ((!hasHotel || !hotelSearchRequired) && (!hasFlight || !flightSearchRequired))
                {
                    foreach (var quoteTransfer in quoteRetrieveResponse.Transfers)
                    {
                        if (quoteTransfer.ComponentRepriced)
                        {
                            IBasketComponent basketComponent = this.basketAdaptor.CreateTransferComponent(quoteTransfer, quoteRetrieveResponse.GuestDetails);
                            this.basketService.AddBasketComponent(basketToken, basketComponent);
                        }
                    }
                }

                IBasket basket = this.basketService.GetBasket(basketToken);
                this.quoteRetrieveResponseProcessor.Process(quoteRetrieveResponse, basket);
            }
            else
            {
                quoteRetrieveReturn.Warnings.AddRange(quoteRetrieveResponse.ReturnStatus.Exceptions);
            }
           
            return quoteRetrieveReturn;
        }

        /// <summary>
        /// Gets the alternative results.
        /// </summary>
        /// <param name="quoteRetrieveResponse">The quote retrieve response.</param>
        /// <param name="flightSearchRequired">if set to <c>true</c> [flight search required].</param>
        /// <param name="hotelSearchRequired">if set to <c>true</c> [hotel search required].</param>
        /// <returns>The results</returns>
        private async Task<List<IResultsModel>> GetAlternativeResults(
            QuoteRetrieveResponse quoteRetrieveResponse,
            bool flightSearchRequired,
            bool hotelSearchRequired)
        {
            ISearchModel searchModel = new SearchModel();

            if (flightSearchRequired && hotelSearchRequired)
            {
                searchModel.SearchMode = SearchMode.FlightPlusHotel;
            }
            else if (hotelSearchRequired)
            {
                searchModel.SearchMode = SearchMode.Hotel;
            }
            else if (flightSearchRequired)
            {
                searchModel.SearchMode = SearchMode.Flight;
            }

            this.searchAdaptor.Create(searchModel, quoteRetrieveResponse);
            return await this.searchService.Search(searchModel);
        }
    }
}
