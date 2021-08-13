namespace Web.Template.Application.Results.Adaptors
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using AutoMapper;

    using iVectorConnectInterface.Interfaces;
    using iVectorConnectInterface.Transfer;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Results;
    using Web.Template.Application.Results.ResultModels;

    /// <summary>
    /// A class that takes in Connect flight results and coverts them to our result model.
    /// </summary>
    /// <seealso cref="IConnectResultsAdaptor" />
    public class ConnectTransferResultAdaptor : IConnectResultsAdaptor
    {
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectTransferResultAdaptor" /> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        public ConnectTransferResultAdaptor(IMapper mapper)
        {
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets the type of the response.
        /// </summary>
        /// <value>
        /// The type of the response.
        /// </value>
        public Type ResponseType => typeof(SearchResponse);

        /// <summary>
        /// Takes in an connect flight result and outputs our web results model.
        /// </summary>
        /// <param name="ivcTransferResult">The connect transfer result.</param>
        /// <param name="searchMode">The search mode.</param>
        /// <returns>A single result</returns>
        public IResult Create(SearchResponse.Transfer ivcTransferResult, SearchMode searchMode)
        {
            var transferResult = new TransferResult(this.mapper)
            {
                SearchMode = searchMode,
                ArrivalParentType = ivcTransferResult.ArrivalParentType,
                ArrivalParentId = ivcTransferResult.ArrivalParentID,
                BookingToken = ivcTransferResult.BookingToken,
                DepartureParentType = ivcTransferResult.DepartureParentType,
                DepartureId = ivcTransferResult.DepartureParentID,
                MaximumCapacity = ivcTransferResult.MaximumCapacity,
                MinimumCapacity = ivcTransferResult.MinimumCapacity,
                OutboundJourneyTime = ivcTransferResult.OutboundJourneyTime,
                ReturnJourneyTime = ivcTransferResult.ReturnJourneyTime,
                Vehicle = ivcTransferResult.Vehicle,
                VehicleQuantity = ivcTransferResult.VehicleQuantity,
                TotalPrice = ivcTransferResult.Price,
                SupplierId = ivcTransferResult.SupplierDetails.SupplierID
            };

            transferResult.ComponentToken = transferResult.GetHashCode();

            return transferResult;
        }

        /// <summary>
        /// Takes in a connect search response and returns a list of results
        /// </summary>
        /// <param name="connectResponse">The connect response.</param>
        /// <param name="searchModel">The search model.</param>
        /// <param name="context">The context.</param>
        /// <returns>A single property result</returns>
        public List<IResultsModel> Create(iVectorConnectResponse connectResponse, ISearchModel searchModel, HttpContext context)
        {
            var ivcTransferResponse = (SearchResponse)connectResponse;
            var results = new List<IResultsModel>();

            var resultModel = new Results();
            foreach (SearchResponse.Transfer transfer in ivcTransferResponse.Transfers)
            {
                resultModel.ResultsCollection.Add(this.Create(transfer, searchModel.SearchMode));
            }

            results.Add(resultModel);

            return results;
        }
    }
}