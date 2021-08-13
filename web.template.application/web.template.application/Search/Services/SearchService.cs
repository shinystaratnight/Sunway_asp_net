namespace Web.Template.Application.Search.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    using iVectorConnectInterface.Property;

    using Intuitive;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Services;

    /// <summary>
    ///     Service used to carry out searches
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Services.ISearchService" />
    public class SearchService : ISearchService, IDisposable
    {
        /// <summary>
        /// The result service
        /// </summary>
        private readonly IResultService resultService;

        /// <summary>
        /// The log writer
        /// </summary>
        private ILogWriter logWriter;

        /// <summary>
        ///     The property search adaptor
        /// </summary>
        private ISearchAdaptor searchAdaptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchService" /> class.
        /// </summary>
        /// <param name="searchAdaptor">The search adaptor.</param>
        /// <param name="resultService">The result service.</param>
        /// <param name="logWriter">The log writer.</param>
        public SearchService(ISearchAdaptor searchAdaptor, IResultService resultService, ILogWriter logWriter)
        {
            this.searchAdaptor = searchAdaptor;
            this.resultService = resultService;
            this.logWriter = logWriter;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.searchAdaptor = null;
            GC.Collect();
        }

        /// <summary>
        ///     Searches the specified search model.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <returns>
        ///     A results model
        /// </returns>
        public async Task<List<IResultsModel>> Search(ISearchModel searchModel)
        {
            var resultsModelList = new List<IResultsModel>();
            
            try
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                using (tokenSource)
                {
                    CancellationToken token = tokenSource.Token;
                    HttpContext context = HttpContext.Current;

                    //////Todo implent search strategy
                    switch (searchModel.SearchMode)
                    {
                        case SearchMode.Hotel:
                            IResultsModel propertyResults = await this.searchAdaptor.Search<SearchResponse>(searchModel, token, context);
                            resultsModelList.Add(propertyResults);
                            break;

                        case SearchMode.Flight:
                            IResultsModel flightResults = await this.searchAdaptor.Search<iVectorConnectInterface.Flight.SearchResponse>(searchModel, token, context);
                            resultsModelList.Add(flightResults);
                            break;

                        case SearchMode.FlightPlusHotel:

                            if (searchModel.IsPackageSearch)
                            {
                                IPackageResultsModel packageResults = await this.searchAdaptor.PackageSearch<iVectorConnectInterface.Package.SearchResponse>(searchModel, token, context);
                                resultsModelList.AddRange(packageResults.ResultsCollection);
                            }
                            else
                            {
                                List<Task<IResultsModel>> taskList = new List<Task<IResultsModel>>();
                                TaskFactory factory = new TaskFactory(token);
                                taskList.Add(await factory.StartNew(() => this.searchAdaptor.Search<SearchResponse>(searchModel, token, context)));

                                taskList.Add(await factory.StartNew(() => this.searchAdaptor.Search<iVectorConnectInterface.Flight.SearchResponse>(searchModel, token, context)));

                                IResultsModel firstResult = await Task.WhenAny(taskList).Result;

                                if (firstResult.ResultsCollection?.Count == 0)
                                {
                                    tokenSource.Cancel();
                                }

                                IResultsModel[] results = await Task.WhenAll(taskList);

                                foreach (var result in results)
                                {
                                    resultsModelList.Add(result);
                                }
                            }

                            break;
                        case SearchMode.Transfer:
                            IResultsModel transferResults = await this.searchAdaptor.Search<iVectorConnectInterface.Transfer.SearchResponse>(searchModel, token, context);
                            resultsModelList.Add(transferResults);
                            break;
                    }
                }

                this.resultService.SaveResults(resultsModelList);
            }
            catch (Exception ex)
            {
                this.logWriter.Write("Search Service", "Exception Search", ex.ToString());
            }

            return resultsModelList;
        }
    }
}