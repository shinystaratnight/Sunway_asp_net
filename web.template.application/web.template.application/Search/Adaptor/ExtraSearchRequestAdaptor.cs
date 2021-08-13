namespace Web.Template.Application.Search.Adaptor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using iVectorConnectInterface.Extra;
    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Search;
    using Web.Template.Application.IVectorConnect.Requests;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class ExtraSearchRequestAdaptor.
    /// </summary>
    public class ExtraSearchRequestAdaptor : IExtraSearchRequestAdaptor
    {
        /// <summary>
        /// The connect login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory connectLoginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtraSearchRequestAdaptor"/> class.
        /// </summary>
        /// <param name="connectLoginDetailsFactory">The connect login details factory.</param>
        public ExtraSearchRequestAdaptor(IConnectLoginDetailsFactory connectLoginDetailsFactory)
        {
            this.connectLoginDetailsFactory = connectLoginDetailsFactory;
        }

        /// <summary>
        /// Creates the specified search model.
        /// </summary>
        /// <param name="searchmodel">The search model.</param>
        /// <param name="context">The context.</param>
        /// <returns>The iVectorConnectRequest.</returns>
        public iVectorConnectRequest Create(IExtraSearchModel searchmodel, HttpContext context)
        {
            SearchRequest connectRequest = this.SetupConnectRequest(searchmodel, context);
            return connectRequest;
        }

        /// <summary>
        /// Setups the connect request.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <param name="context">The context.</param>
        /// <returns>The extra search request</returns>
        private SearchRequest SetupConnectRequest(IExtraSearchModel searchModel, HttpContext context)
        {
            var connectRequest = new SearchRequest
                                     {
                                         LoginDetails = this.connectLoginDetailsFactory.Create(context),

                                         ExtraID = searchModel.ExtraId,
                                         ExtraGroupID = searchModel.ExtraGroupId,
                                         ExtraTypes = new List<SearchRequest.ExtraType>(),

                                         BookingType = searchModel.BookingType,
                                         BookingPrice = searchModel.BookingPrice,

                                         DepartureAirportID = searchModel.DepartureAirportId,
                                         ArrivalAirportID = searchModel.ArrivalAirportId,
                                         GeographyLevel1ID = searchModel.GeographyLevel1Id,
                                         GeographyLevel2ID = searchModel.GeographyLevel2Id,
                                         GeographyLevel3ID = searchModel.GeographyLevel3Id,
                                         PropertyReferenceID = searchModel.PropertyReferenceId,

                                         DepartureDate = searchModel.DepartureDate,
                                         ReturnDate = searchModel.ReturnDate,
                                         DepartureTime = searchModel.DepartureTime,
                                         ReturnTime = searchModel.ReturnTime,

                                         GuestConfiguration = new ivci.Support.GuestConfiguration
                                                                 {
                                                                     Adults = searchModel.Adults,
                                                                     Children = searchModel.Children,
                                                                     Infants = searchModel.Infants,
                                                                     AdultAges = searchModel.AdultAges,
                                                                     ChildAges = searchModel.ChildAges
                                                                 }
                                     };

            if (searchModel.ExtraTypes != null)
            {
                foreach (int extraTypeId in searchModel.ExtraTypes)
                {
                    var extraType = new SearchRequest.ExtraType() { ExtraTypeID = extraTypeId };
                    connectRequest.ExtraTypes.Add(extraType);
                }
            }

            return connectRequest;
        }
    }
}
