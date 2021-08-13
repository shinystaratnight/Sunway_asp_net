namespace Web.Template.Application.Search.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;

    using iVectorConnectInterface.Extra;

    using Intuitive;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Search;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Results.ResultModels;
    using Web.Template.Application.Search.Adaptor;
    using Web.Template.Application.Search.SearchModels;

    /// <summary>
    /// Class ExtraSearchService.
    /// </summary>
    public class ExtraSearchService : IExtraSearchService
    {
        /// <summary>
        /// The basket service
        /// </summary>
        private readonly IBasketService basketService;

        /// <summary>
        /// The extra search model adaptor
        /// </summary>
        private readonly IExtraSearchModelAdaptor extraSearchModelAdaptor;

        /// <summary>
        /// The log writer
        /// </summary>
        private readonly ILogWriter logWriter;

        /// <summary>
        /// The result service
        /// </summary>
        private readonly IResultService resultService;

        /// <summary>
        /// The search adaptor
        /// </summary>
        private readonly ISearchAdaptor searchAdaptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtraSearchService" /> class.
        /// </summary>
        /// <param name="extraSearchModelAdaptor">The extra search model adaptor.</param>
        /// <param name="basketService">The basket service.</param>
        /// <param name="searchAdaptor">The search adaptor.</param>
        /// <param name="logWriter">The log writer.</param>
        /// <param name="resultService">The result service.</param>
        public ExtraSearchService(
            IExtraSearchModelAdaptor extraSearchModelAdaptor,
            IBasketService basketService,
            ISearchAdaptor searchAdaptor,
            ILogWriter logWriter,
            IResultService resultService)
        {
            this.extraSearchModelAdaptor = extraSearchModelAdaptor;
            this.basketService = basketService;
            this.searchAdaptor = searchAdaptor;
            this.logWriter = logWriter;
            this.resultService = resultService;
        }

        /// <summary>
        /// Searches from basket.
        /// </summary>
        /// <param name="extraBasketSearchModel">The extra basket search model.</param>
        /// <returns>The result</returns>
        public async Task<IResultsModel> SearchFromBasket(IExtraBasketSearchModel extraBasketSearchModel)
        {
            IResultsModel resultsModel = new Results();

            try
            {
                IBasket basket = this.basketService.GetBasket(extraBasketSearchModel.BasketToken);
                IExtraSearchModel extraSearchModel = this.extraSearchModelAdaptor.Create(basket, extraBasketSearchModel);
                resultsModel = await this.searchAdaptor.ExtraSearch<SearchResponse>(extraSearchModel, HttpContext.Current);
                resultsModel.ExtraSearchModel = extraSearchModel;

                this.resultService.SaveResults(new List<IResultsModel> { resultsModel });
            }
            catch (Exception ex)
            {
                this.logWriter.Write("Search Service", "Exception Search", ex.ToString());
            }

            return resultsModel;
        }
    }
}
