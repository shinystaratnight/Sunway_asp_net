namespace Web.Template.Application.Results.Adaptors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using iVectorConnectInterface.Interfaces;
    using iVectorConnectInterface.Package;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Results;
    using Web.Template.Application.Results.ResultModels;

    /// <summary>
    /// A class that takes in Connect flight results and converts them to our result model.
    /// </summary>
    /// <seealso cref="IConnectResultsAdaptor" />
    public class ConnectPackageResultAdaptor : IConnectResultsAdaptor
    {
        /// <summary>
        /// The airport repository
        /// </summary>
        private readonly ConnectFlightAdaptor connectFlightAdaptor;

        /// <summary>
        /// The flight carrier repository
        /// </summary>
        private readonly ConnectPropertyAdaptor connectPropertyAdaptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectPackageResultAdaptor" /> class.
        /// </summary>
        /// <param name="connectPropertyAdaptor">The connect property adaptor.</param>
        /// <param name="connectFlightAdaptor">The connect flight adaptor.</param>
        public ConnectPackageResultAdaptor(ConnectPropertyAdaptor connectPropertyAdaptor, ConnectFlightAdaptor connectFlightAdaptor)
        {
            this.connectPropertyAdaptor = connectPropertyAdaptor;
            this.connectFlightAdaptor = connectFlightAdaptor;
        }

        /// <summary>
        /// Gets the type of the response.
        /// </summary>
        /// <value>
        /// The type of the response.
        /// </value>
        public Type ResponseType => typeof(SearchResponse);

        /// <summary>
        /// Creates the specified connect result.
        /// </summary>
        /// <param name="connectResponse">The connect response.</param>
        /// <param name="searchModel">The search model.</param>
        /// <param name="context">The context.</param>
        /// <returns>A single property result</returns>
        List<IResultsModel> IConnectResultsAdaptor.Create(iVectorConnectResponse connectResponse, ISearchModel searchModel, HttpContext context)
        {
            var packageResponse = (SearchResponse)connectResponse;

            var results = new List<IResultsModel>();

            var flightResults = new Results { SearchMode = SearchMode.Flight };
            var propertyResults = new Results { SearchMode = SearchMode.Hotel };
            this.connectPropertyAdaptor.SetArrivalDate(packageResponse.ArrivalDate);
            this.connectPropertyAdaptor.SetDuration(packageResponse.Duration);

            packageResponse.Flights?.ForEach(f => flightResults.ResultsCollection.Add(this.connectFlightAdaptor.Create(f, searchModel.SearchMode, context)));

            packageResponse.PropertyResults?.ForEach(
                p =>
                    {
                        var propertyResult = this.connectPropertyAdaptor.Create(p, searchModel.SearchMode, context);
                        if (propertyResult.SubResults.Select(subResult => ((RoomOption)subResult).Sequence).Distinct().Count() == searchModel.Rooms.Count)
                        {
                            propertyResults.ResultsCollection.Add(propertyResult);
                        }
                    });

            results.Add(flightResults);
            results.Add(propertyResults);

            return results;
        }
    }
}