namespace Web.Template.Application.Quote.Builders
{
    using System.Collections.Generic;
    using System.Linq;

    using iVectorConnectInterface;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Quote.Adaptors;
    using Web.Template.Application.Interfaces.Quote.Builders;
    using Web.Template.Application.Interfaces.Quote.Models;
    using Web.Template.Application.Search.SearchModels;

    /// <summary>
    /// Class QuoteRetrieveReturnBuilder.
    /// </summary>
    public class QuoteRetrieveReturnBuilder : IQuoteRetrieveReturnBuilder
    {
        /// <summary>
        /// The quote retrieve return
        /// </summary>
        private readonly IQuoteRetrieveReturn quoteRetrieveReturn;

        /// <summary>
        /// The search adaptor
        /// </summary>
        private readonly IQuoteRetrieveSearchAdaptor searchAdaptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteRetrieveReturnBuilder" /> class.
        /// </summary>
        /// <param name="quoteRetrieveReturn">The quote retrieve return.</param>
        /// <param name="searchAdaptor">The search adaptor.</param>
        public QuoteRetrieveReturnBuilder(
            IQuoteRetrieveReturn quoteRetrieveReturn,
            IQuoteRetrieveSearchAdaptor searchAdaptor)
        {
            this.quoteRetrieveReturn = quoteRetrieveReturn;
            this.searchAdaptor = searchAdaptor;
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <returns>THe Quote Retrieve Return.</returns>
        public IQuoteRetrieveReturn GetResult()
        {
            return this.quoteRetrieveReturn;
        }

        /// <summary>
        /// Sets the basket token.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        public void SetBasketToken(string basketToken)
        {
            this.quoteRetrieveReturn.BasketToken = basketToken;
        }

        /// <summary>
        /// Sets the search model.
        /// </summary>
        /// <param name="quoteRetrieveResponse">The quote retrieve response.</param>
        public void SetSearchModel(QuoteRetrieveResponse quoteRetrieveResponse)
        {
            this.quoteRetrieveReturn.SearchModel = new SearchModel();

            if (quoteRetrieveResponse.Properties.Any() && quoteRetrieveResponse.Flights.Any())
            {
                this.quoteRetrieveReturn.SearchModel.SearchMode = SearchMode.FlightPlusHotel;
            }
            else if (quoteRetrieveResponse.Properties.Any())
            {
                this.quoteRetrieveReturn.SearchModel.SearchMode = SearchMode.Hotel;
            }
            else if (quoteRetrieveResponse.Flights.Any())
            {
                this.quoteRetrieveReturn.SearchModel.SearchMode = SearchMode.Flight;
            }

            this.searchAdaptor.Create(this.quoteRetrieveReturn.SearchModel, quoteRetrieveResponse);
        }

        /// <summary>
        /// Sets the search result.
        /// </summary>
        /// <param name="searchResult">The search result.</param>
        public void SetSearchResult(List<IResultsModel> searchResult)
        {
            foreach (IResultsModel resultModel in searchResult)
            {
                this.quoteRetrieveReturn.ResultCounts.Add(resultModel.SearchMode.ToString(), resultModel.ResultsCollection.Count);
                this.quoteRetrieveReturn.ResultTokens.Add(resultModel.SearchMode.ToString(), resultModel.ResultToken);
                this.quoteRetrieveReturn.Warnings.AddRange(resultModel.WarningList);

                if (resultModel.SearchMode == SearchMode.Flight)
                {
                    var cheapestResult =
                        resultModel.ResultsCollection.OrderBy(result => result.TotalPrice).FirstOrDefault();
                    this.quoteRetrieveReturn.SelectedFlightToken = cheapestResult?.ComponentToken ?? 0;
                }
            }
        }

        /// <summary>
        /// Setups the components.
        /// </summary>
        /// <param name="quoteRetrieveResponse">The quote retrieve response.</param>
        public void SetupComponents(QuoteRetrieveResponse quoteRetrieveResponse)
        {
            this.quoteRetrieveReturn.PriceChange = 0;
            if (quoteRetrieveResponse.Flights.Any())
            {
                this.quoteRetrieveReturn.QuoteComponentTypes.Add(ComponentType.Flight);
                QuoteFlight flight = quoteRetrieveResponse.Flights.FirstOrDefault();
                if (flight != null && flight.ComponentRepriced)
                {
                    this.quoteRetrieveReturn.RepricedComponentTypes.Add(ComponentType.Flight);
                    this.quoteRetrieveReturn.PriceChange += (flight.TotalPrice - flight.QuotedTotalPrice); 
                }
            }

            if (quoteRetrieveResponse.Properties.Any())
            {
                this.quoteRetrieveReturn.QuoteComponentTypes.Add(ComponentType.Hotel);
                QuoteProperty property = quoteRetrieveResponse.Properties.FirstOrDefault();
                if (property != null)
                {
                    this.quoteRetrieveReturn.PropertyId = property.PropertyReferenceID;

                    if (property.ComponentRepriced)
                    {
                        this.quoteRetrieveReturn.RepricedComponentTypes.Add(ComponentType.Hotel);
                        this.quoteRetrieveReturn.PriceChange += (property.TotalPrice - property.QuotedTotalPrice);
                    }
                }
            }

            if (quoteRetrieveResponse.Transfers.Any())
            {
                this.quoteRetrieveReturn.QuoteComponentTypes.Add(ComponentType.Transfer);
                QuoteTransfer transfer = quoteRetrieveResponse.Transfers.FirstOrDefault();

                if (transfer != null && transfer.ComponentRepriced)
                {
                    this.quoteRetrieveReturn.RepricedComponentTypes.Add(ComponentType.Transfer);
                    this.quoteRetrieveReturn.PriceChange += (transfer.TotalPrice - transfer.QuotedTotalPrice);
                }
            }
        }
    }
}
