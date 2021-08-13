namespace Web.Template.Application.Net.IVectorConnect
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Xml;

    using iVectorConnectInterface.Interfaces;
    using iVectorConnectInterface.Property;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Results;
    using Web.Template.Application.Interfaces.Search;
    using Web.Template.Application.Results.ResultModels;
    using Web.Template.Application.Search.Adaptor;
    using Web.Template.Application.Search.SearchModels;

    /// <summary>
    ///     Adapts to a connect search request
    /// </summary>
    public class ConnectSearchAdaptor : ISearchAdaptor
    {
        /// <summary>
        /// The connect results adaptor factory
        /// </summary>
        private readonly IIVConnectResultsAdaptorFactory connectResultsAdaptorFactory;

        /// <summary>
        /// The ivector connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory ivectorConnectRequestFactory;

        /// <summary>
        /// The property search request factory
        /// </summary>
        private readonly ISearchRequestAdaptorFactory searchRequestAdapterFactory;

        /// <summary>
        /// The extra search request adaptor
        /// </summary>
        private readonly IExtraSearchRequestAdaptor extraSearchRequestAdaptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectSearchAdaptor" /> class.
        /// </summary>
        /// <param name="ivectorConnectRequestFactory">The ivector connect request factory.</param>
        /// <param name="searchRequestAdapterFactory">The request adapter factory.</param>
        /// <param name="connectResultsAdaptorFactory">The connect results adaptor factory.</param>
        public ConnectSearchAdaptor(
            IIVectorConnectRequestFactory ivectorConnectRequestFactory,
            ISearchRequestAdaptorFactory searchRequestAdapterFactory,
            IIVConnectResultsAdaptorFactory connectResultsAdaptorFactory,
            IExtraSearchRequestAdaptor extraSearchRequestAdaptor)
        {
            this.ivectorConnectRequestFactory = ivectorConnectRequestFactory;
            this.searchRequestAdapterFactory = searchRequestAdapterFactory;
            this.connectResultsAdaptorFactory = connectResultsAdaptorFactory;
            this.extraSearchRequestAdaptor = extraSearchRequestAdaptor;
        }

        /// <summary>
        /// Perform a package search
        /// </summary>
        /// <typeparam name="T">The search response type</typeparam>
        /// <param name="searchModel">The search model.</param>
        /// <param name="token">The token.</param>
        /// <param name="context">The context.</param>
        /// <returns>A package results model</returns>
        public async Task<IPackageResultsModel> PackageSearch<T>(ISearchModel searchModel, CancellationToken token, HttpContext context) where T : class, iVectorConnectResponse, new()
        {
            var resultsModel = new PackageResultsModel();
            try
            {
                ISearchRequestAdapter searchRequestAdapter = this.searchRequestAdapterFactory.CreateAdaptorByResponseType(typeof(T));
                iVectorConnectRequest requestBody = searchRequestAdapter.Create(searchModel, context);

                IIVectorConnectRequest ivcRequest = this.ivectorConnectRequestFactory.Create(requestBody, context);
                T searchResponse = await ivcRequest.GoAsync<T>();
                
                IConnectResultsAdaptor connectResultsAdaptor = this.connectResultsAdaptorFactory.CreateAdaptorByResponseType(typeof(T));
                resultsModel.ResultsCollection = connectResultsAdaptor.Create(searchResponse, searchModel, context);

                resultsModel.Success = true;

                foreach (IResultsModel result in resultsModel.ResultsCollection)
                {
                    result.SearchModel = searchModel;
                }
             }
            catch (Exception ex)
            {
                Intuitive.FileFunctions.AddLogEntry("SearchAdaptor", "search Exception", ex.ToString());
                resultsModel.WarningList.Add(ex.ToString());
                resultsModel.Success = false;
                throw;
            }

            return resultsModel;
        }

        /// <summary>
        /// Hotels the search.
        /// </summary>
        /// <typeparam name="T">The type of the connect response</typeparam>
        /// <param name="searchModel">The search model.</param>
        /// <param name="token">Cancellation token allowing results of one search to cancel another</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// ResultsCollection Model
        /// </returns>
        public async Task<IResultsModel> Search<T>(ISearchModel searchModel, CancellationToken token, HttpContext context) where T : class, iVectorConnectResponse, new()
        {
            var resultsModel = new Results();
            resultsModel.SearchModel = searchModel;

            if (searchModel.SearchMode == SearchMode.FlightPlusHotel)
            {
                SetSearchMode<T>(resultsModel);
            }
            else
            {
                resultsModel.SearchMode = searchModel.SearchMode;
            }

            try
            {
                ISearchRequestAdapter searchRequestAdapter = this.searchRequestAdapterFactory.CreateAdaptorByResponseType(typeof(T));
                iVectorConnectRequest requestBody = searchRequestAdapter.Create(searchModel, context);

                List<string> warnings = requestBody.Validate();
                if (warnings.Count > 0)
                {
                    resultsModel.Success = false;
                    resultsModel.WarningList = warnings;
                }
                else
                {
                    IIVectorConnectRequest ivcRequest = this.ivectorConnectRequestFactory.Create(requestBody, context);

                    T searchResponse = await ivcRequest.GoAsync<T>();

                    IConnectResultsAdaptor connectResultsAdaptor = this.connectResultsAdaptorFactory.CreateAdaptorByResponseType(typeof(T));

                    List<IResultsModel> resultModels = connectResultsAdaptor.Create(searchResponse, searchModel, context);
                    resultsModel.ResultsCollection = resultModels?.FirstOrDefault()?.ResultsCollection;
                    resultsModel.Success = true;
                }
            }
            catch (Exception ex)
            {
                Intuitive.FileFunctions.AddLogEntry("SearchAdaptor", "search Exception", ex.ToString());
                resultsModel.WarningList.Add(ex.ToString());
                resultsModel.Success = false;
            }

            return resultsModel;
        }

        /// <summary>
        /// Searches the specified search model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchModel">The search model.</param>
        /// <param name="context">The context.</param>
        /// <returns>The result</returns>
        public async Task<IResultsModel> ExtraSearch<T>(IExtraSearchModel searchModel, HttpContext context) where T : class, iVectorConnectResponse, new()
        {
            var resultsModel = new Results { SearchMode = SearchMode.Extra };

            try
            {
                iVectorConnectRequest requestBody = this.extraSearchRequestAdaptor.Create(searchModel, context);
                List<string> warnings = requestBody.Validate();
                if (warnings.Count > 0)
                {
                    resultsModel.Success = false;
                    resultsModel.WarningList = warnings;
                }
                else
                {
                    IIVectorConnectRequest ivcRequest = this.ivectorConnectRequestFactory.Create(requestBody, context);

                    T searchResponse = await ivcRequest.GoAsync<T>();

                    IConnectResultsAdaptor connectResultsAdaptor = this.connectResultsAdaptorFactory.CreateAdaptorByResponseType(typeof(T));

                    List<IResultsModel> resultModels = connectResultsAdaptor.Create(searchResponse, new SearchModel(), context);
                    resultsModel.ResultsCollection = resultModels?.FirstOrDefault()?.ResultsCollection;
                    resultsModel.Success = true;
                }
            }
            catch (Exception ex)
            {
                Intuitive.FileFunctions.AddLogEntry("SearchAdaptor", "search Exception", ex.ToString());
                resultsModel.WarningList.Add(ex.ToString());
                resultsModel.Success = false;
            }


            return resultsModel;
        }

        /// <summary>
        /// Sets the search mode based on the search type
        /// </summary>
        /// <typeparam name="T">Type of the search response</typeparam>
        /// <param name="resultsModel">The results model.</param>
        private static void SetSearchMode<T>(Results resultsModel) where T : class, iVectorConnectResponse, new()
        {
            resultsModel.SearchMode = typeof(T) == typeof(SearchResponse) ? SearchMode.Hotel : SearchMode.Flight;
        }
    }
}