namespace Web.Template.Application.Tests.Adaptors.IVectorConnect.Search
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    using iVectorConnectInterface.Interfaces;
    using iVectorConnectInterface.Property;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Results;
    using Web.Template.Application.Interfaces.Search;
    using Web.Template.Application.Net.IVectorConnect;
    using Web.Template.Application.Search.Adaptor;
    using Web.Template.Application.Search.SearchModels;

    /// <summary>
    /// Tests for the Connect Search Adaptor Class
    /// </summary>
    [TestFixture]
    public class ConnectSearchAdaptorTests
    {
        /// <summary>
        ///     Search should return warnings when invalid or incomplete search details provided.
        /// </summary>
        [Test]
        public void Search_Should_ReturnWarnings_When_InvalidOrIncompleteSearchDetailsProvided()
        {
            ////Arrange
            var requestFactoryMock = new Mock<IIVectorConnectRequestFactory>();

            var searchModel = new SearchModel { SearchMode = SearchMode.Hotel };

            var connectResultsAdaptor = new Mock<IIVConnectResultsAdaptorFactory>();
            var requestAdaptor = new Mock<ISearchRequestAdapter>();

            var requestAdaptorFactory = new Mock<ISearchRequestAdaptorFactory>();
            var extraSearchRequestAdaptor = new Mock<IExtraSearchRequestAdaptor>();

            requestAdaptorFactory.Setup(x => x.CreateAdaptorByResponseType(typeof(SearchResponse))).Returns(requestAdaptor.Object);

            requestAdaptor.Setup(x => x.Create(searchModel, HttpContext.Current)).Returns(new SearchRequest());

            ISearchAdaptor searchAdaptor = new ConnectSearchAdaptor(
                requestFactoryMock.Object,
                requestAdaptorFactory.Object,
                connectResultsAdaptor.Object,
                extraSearchRequestAdaptor.Object);

            CancellationToken token = default(CancellationToken);

            ////Act
            Task<IResultsModel> resultsModelTask = searchAdaptor.Search<SearchResponse>(searchModel, token, HttpContext.Current);
            Task.WaitAny(resultsModelTask);

            ////Assert
            Assert.GreaterOrEqual(resultsModelTask.Result.WarningList.Count, 1);
            Assert.IsFalse(resultsModelTask.Result.Success);
        }

        /// <summary>
        /// Search should run to completion when provided correct details
        /// </summary>
        [Test]
        public void Search_Should_RunToCompletion_When_Provided_Correct_Details()
        {
            ////Arrange
            var searchModel = new SearchModel { SearchMode = SearchMode.Hotel };

            var searchRequestMock = new Mock<iVectorConnectRequest>();
            searchRequestMock.Setup(sr => sr.Validate(eValidationType.None)).Returns(new List<string>());

            var requestFactoryMock = new Mock<IIVectorConnectRequestFactory>();
            var dummySearchResponse = new SearchResponse();
            var connectRequestMock = new Mock<IIVectorConnectRequest>();
            connectRequestMock.Setup(cr => cr.GoAsync<SearchResponse>(false)).ReturnsAsync(dummySearchResponse);
            requestFactoryMock.Setup(rfm => rfm.Create(searchRequestMock.Object, HttpContext.Current)).Returns(connectRequestMock.Object);

            var connectResultsAdaptorMock = new Mock<IConnectResultsAdaptor>();

            connectResultsAdaptorMock.Setup(ram => ram.Create(dummySearchResponse, searchModel, HttpContext.Current)).Returns(new List<IResultsModel>());

            var connectResultsAdaptorFactorMock = new Mock<IIVConnectResultsAdaptorFactory>();
            connectResultsAdaptorFactorMock.Setup(ram => ram.CreateAdaptorByResponseType(typeof(SearchResponse))).Returns(connectResultsAdaptorMock.Object);

            var requestAdaptorFactory = new Mock<ISearchRequestAdaptorFactory>();
            var requestAdaptorMock = new Mock<ISearchRequestAdapter>();
            requestAdaptorMock.Setup(x => x.Create(searchModel, HttpContext.Current)).Returns(searchRequestMock.Object);
            requestAdaptorFactory.Setup(x => x.CreateAdaptorByResponseType(typeof(SearchResponse))).Returns(requestAdaptorMock.Object);

            var extraSearchRequestAdaptor = new Mock<IExtraSearchRequestAdaptor>();

            ISearchAdaptor searchAdaptor = new ConnectSearchAdaptor(
                requestFactoryMock.Object,
                requestAdaptorFactory.Object,
                connectResultsAdaptorFactorMock.Object,
                extraSearchRequestAdaptor.Object);

            CancellationToken token = default(CancellationToken);

            ////Act
            Task<IResultsModel> resultsModelTask = searchAdaptor.Search<SearchResponse>(searchModel, token, HttpContext.Current);
            Task.WaitAny(resultsModelTask);

            ////Assert
            Assert.IsEmpty(resultsModelTask.Result.WarningList);
            Assert.IsTrue(resultsModelTask.Result.Success);
        }
    }
}