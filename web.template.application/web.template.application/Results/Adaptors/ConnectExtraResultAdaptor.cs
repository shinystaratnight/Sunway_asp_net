namespace Web.Template.Application.Results.Adaptors
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using AutoMapper;

    using iVectorConnectInterface.Interfaces;
    using iVectorConnectInterface.Extra;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Results;
    using Web.Template.Application.Results.ResultModels;

    /// <summary>
    /// Class ConnectExtraAdaptor.
    /// </summary>
    public class ConnectExtraResultAdaptor : IConnectResultsAdaptor
    {
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// The extra service
        /// </summary>
        private readonly IExtraService extraService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectExtraResultAdaptor" /> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="extraService">The extra service.</param>
        public ConnectExtraResultAdaptor(IMapper mapper, IExtraService extraService)
        {
            this.mapper = mapper;
            this.extraService = extraService;
        }

        /// <summary>
        /// Gets or sets the type of the response.
        /// </summary>
        /// <value>The type of the response.</value>
        public Type ResponseType => typeof(SearchResponse);

        public IResult Create(SearchResponse.Extra extra, int extraTypeId)
        {
            var extraResult = new ExtraResult(this.mapper)
            {
                SearchMode = SearchMode.Extra,
                ExtraId = extra.ExtraID,
                ExtraName = extra.ExtraName,
                ExtraType = this.extraService.GetExtraTypeById(extraTypeId)?.Name,
                ExtraTypeId = extraTypeId,
                SubResults = new List<ISubResult>()
            };

            extraResult.ComponentToken = extraResult.GetHashCode();

            foreach (SearchResponse.Option option in extra.Options)
            {
                var extraOption = new ExtraOption()
                                      {
                                          BookingToken = option.BookingToken,

                                          ExtraCategory = option.ExtraCategory,
                                          ExtraCategoryId = option.ExtraCategoryID,

                                          Duration = option.Duration,
                                          StartDate = option.StartDate,
                                          EndDate = option.EndDate,
                                          StartTime = option.StartTime,
                                          EndTime = option.EndTime,

                                          PricingType = option.PricingType,
                                          ExtraPrice = option.ExtraPrice,
                                          TotalPrice = option.TotalPrice,
                                          AdultPrice = option.AdultPrice,
                                          ChildPrice = option.ChildPrice,
                                          InfantPrice = option.InfantPrice,
                                          SeniorPrice = option.SeniorPrice,
                                          TotalCommission = option.TotalCommission,

                                          MinimumAge = option.MinimumAge,
                                          MaximumAge = option.MaximumAge,
                                          MinChildAge = option.MinChildAge,
                                          MaxChildAge = option.MaxChildAge,
                                          SeniorAge = option.SeniorAge,

                                          OccupancyRules = option.OccupancyRules,
                                          MinPassengers = option.MinPassengers,
                                          MaxPassengers = option.MaxPassengers,
                                          MinAdults = option.MinAdults,
                                          MaxAdults = option.MaxAdults,
                                          MinChildren = option.MinChildren,
                                          MaxChildren = option.MaxChildren,

                                          MultiBook = option.MultiBook,
                                          MaximumQuantity = option.MaximumQuantity,

                                          AgeRequired = option.AgeRequired,
                                          DateRequired = option.DateRequired,
                                          TimeRequired = option.TimeRequired,

                                          Description = option.Description
                                      };
                extraOption.ComponentToken = extraOption.GetHashCode();

                extraResult.SubResults.Add(extraOption);
            }

            return extraResult;
        }

        /// <summary>
        /// Creates the specified connect response.
        /// </summary>
        /// <param name="connectResponse">The connect response.</param>
        /// <param name="searchModel">The search model.</param>
        /// <param name="context">The context.</param>
        /// <returns>System.Collections.Generic.List&lt;Web.Template.Application.Interfaces.Models.IResultsModel&gt;.</returns>
        public List<IResultsModel> Create(iVectorConnectResponse connectResponse, ISearchModel searchModel, HttpContext context)
        {
            var results = new List<IResultsModel>();
            var resultModel = new Results { SearchMode = SearchMode.Extra};
            var ivcPropertyResponse = (SearchResponse)connectResponse;

            foreach (SearchResponse.ExtraType extraType in ivcPropertyResponse.ExtraTypes)
            {
                foreach (SearchResponse.ExtraSubType extraSubType in extraType.ExtraSubTypes)
                {
                    foreach (SearchResponse.Extra extra in extraSubType.Extras)
                    {
                        IResult extraResult = this.Create(extra, extraType.ExtraTypeID);
                        resultModel.ResultsCollection.Add(extraResult);
                    }
                }
            }

            results.Add(resultModel);
            return results;
        }
    }
}
