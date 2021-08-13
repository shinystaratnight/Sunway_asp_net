namespace Web.Template.Application.Results.Adaptors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using AutoMapper;

    using iVectorConnectInterface.Interfaces;
    using iVectorConnectInterface.Property;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Results;
    using Web.Template.Application.Results.Factories;
    using Web.Template.Application.Results.ResultModels;

    /// <summary>
    /// Factory for turning iVector Connect property results into IResults
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Results.IConnectResultsAdaptor" />
    /// <seealso cref="IConnectResultsAdaptor" />
    public class ConnectPropertyResultAdaptor : IConnectResultsAdaptor
    {
        /// <summary>
        /// The component adaptor factory
        /// </summary>
        private readonly IIvConnectResultComponentAdaptorFactory componentAdaptorFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectPropertyResultAdaptor" /> class.
        /// </summary>
        /// <param name="componentAdaptorFactory">The component adaptor factory.</param>
        public ConnectPropertyResultAdaptor(IIvConnectResultComponentAdaptorFactory componentAdaptorFactory)
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
        /// Takes a list of property results and calls the create method on each one
        /// </summary>
        /// <param name="connectResponse">The connect response.</param>
        /// <param name="searchModel">The search model.</param>
        /// <param name="context">The context.</param>
        /// <returns>A list of property results</returns>
        public List<IResultsModel> Create(iVectorConnectResponse connectResponse, ISearchModel searchModel, HttpContext context)
        {
            var ivcPropertyResponse = (SearchResponse)connectResponse;
            var results = new List<IResultsModel>();

            var resultModel = new Results();

            IConnectResultComponentAdaptor componentAdaptor = this.componentAdaptorFactory.CreateAdaptorByComponentType(typeof(SearchResponse.PropertyResult));
            componentAdaptor.SetArrivalDate(ivcPropertyResponse.ArrivalDate);
            componentAdaptor.SetDuration(ivcPropertyResponse.Duration);

            foreach (SearchResponse.PropertyResult propertyResult in ivcPropertyResponse.PropertyResults)
            {
                var result = componentAdaptor.Create(propertyResult, searchModel.SearchMode, context);
                if (result.SubResults.Select(subResult => ((RoomOption)subResult).Sequence).Distinct().Count() == searchModel.Rooms.Count)
                {
                    resultModel.ResultsCollection.Add(result);
                }
            }

            results.Add(resultModel);
            return results;
        }
    }
}