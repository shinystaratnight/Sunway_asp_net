namespace Web.Template.Application.Results.Adaptors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using iVectorConnectInterface.Flight;
    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Results;
    using Web.Template.Application.Results.Factories;
    using Web.Template.Application.Results.ResultModels;

    /// <summary>
    /// A class that takes in Connect flight results and coverts them to our result model.
    /// </summary>
    /// <seealso cref="IConnectResultsAdaptor" />
    public class ConnectFlightResultAdaptor : IConnectResultsAdaptor
    {
        /// <summary>
        /// The component adaptor factory
        /// </summary>
        private readonly IIvConnectResultComponentAdaptorFactory componentAdaptorFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectFlightResultAdaptor" /> class.
        /// </summary>
        /// <param name="componentAdaptorFactory">The component adaptor factory.</param>
        public ConnectFlightResultAdaptor(IIvConnectResultComponentAdaptorFactory componentAdaptorFactory)
        {
            this.componentAdaptorFactory = componentAdaptorFactory;
        }

        /// <summary>
        /// Gets the type of the response.
        /// </summary>
        /// <value>
        /// The type of the response.
        /// </value>
        public Type ResponseType => typeof(SearchResponse);

        /// <summary>
        /// Takes in a connect search response and returns a list of results
        /// </summary>
        /// <param name="connectResponse">The connect response.</param>
        /// <param name="searchModel">The search model.</param>
        /// <param name="context">The context.</param>
        /// <returns>A single property result</returns>
        public List<IResultsModel> Create(iVectorConnectResponse connectResponse, ISearchModel searchModel, HttpContext context)
        {
            var ivcFlightResponse = (SearchResponse)connectResponse;

            var results = new List<IResultsModel>();

            var resultModel = new Results();

            IConnectResultComponentAdaptor componentAdaptor = this.componentAdaptorFactory.CreateAdaptorByComponentType(typeof(SearchResponse.Flight));

            foreach (SearchResponse.Flight flight in ivcFlightResponse.Flights.Where(f => f.ExactMatch))
            {
                resultModel.ResultsCollection.Add(componentAdaptor.Create(flight, searchModel.SearchMode, context));
            }

            results.Add(resultModel);

            return results;
        }
    }
}