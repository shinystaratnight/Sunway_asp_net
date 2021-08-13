namespace Web.Template.Application.Tests.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    using Intuitive;

    using iVectorConnectInterface.Property;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Logging;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Search.Services;
    using Web.Template.Application.Services;

    /// <summary>
    /// Tests for the Search Service Class.
    /// </summary>
    [TestFixture]
    internal class SearchServiceTests
    {
        /// <summary>
        ///     Finds the by url_ should_ find page_ when_ existing page is requested.
        /// </summary>
        [Test]
        public void SearchService_Should_CarryOutBothFlightAndHotelSearch_When_SearchModeIsFlightAndHotel()
        {
            //// Arrange
            var searchmodelMock = new Mock<ISearchModel>();
            searchmodelMock.SetupGet(model => model.SearchMode).Returns(SearchMode.FlightPlusHotel);

            var resultsModelMock = new Mock<IResultsModel>();

            var searchAdaptorMock = new Mock<ISearchAdaptor>();
            searchAdaptorMock.Setup(
                adaptor =>
                adaptor.Search<SearchResponse>(
                    searchmodelMock.Object, 
                    It.IsAny<CancellationToken>(), 
                    HttpContext.Current)).ReturnsAsync(resultsModelMock.Object);
            ISearchService searchService = new SearchService(
                searchAdaptorMock.Object, 
                new Mock<IResultService>().Object, 
                new Mock<ILogWriter>().Object);

            //// Act
            Task<List<IResultsModel>> results = searchService.Search(searchmodelMock.Object);

            //// Assert
            Assert.AreEqual(results.Result.Count, 2);
        }

        /// <summary>
        ///     Finds the by url_ should_ find page_ when_ existing page is requested.
        /// </summary>
        [Test]
        public void SearchService_Should_CarryOutFlightOnlySearch_When_SearchModeIsFlight()
        {
            //// Arrange
            var searchmodelMock = new Mock<ISearchModel>();
            searchmodelMock.SetupGet(model => model.SearchMode).Returns(SearchMode.Flight);

            var resultsModelMock = new Mock<IResultsModel>();

            var propertySearchAdaptorMock = new Mock<ISearchAdaptor>();
            propertySearchAdaptorMock.Setup(
                adaptor =>
                adaptor.Search<SearchResponse>(
                    searchmodelMock.Object, 
                    It.IsAny<CancellationToken>(), 
                    HttpContext.Current)).ReturnsAsync(resultsModelMock.Object);
            ISearchService searchService = new SearchService(
                propertySearchAdaptorMock.Object, 
                new Mock<IResultService>().Object, 
                new Mock<ILogWriter>().Object);

            //// Act
            Task<List<IResultsModel>> results = searchService.Search(searchmodelMock.Object);

            //// Assert
            Assert.AreEqual(results.Result.Count, 1);
        }

        /// <summary>
        ///     Finds the by url_ should_ find page_ when_ existing page is requested.
        /// </summary>
        [Test]
        public void SearchService_Should_CarryOutHotelSearch_When_SearchModeIsHotel()
        {
            //// arrange
            var searchmodelMock = new Mock<ISearchModel>();
            searchmodelMock.SetupGet(model => model.SearchMode).Returns(SearchMode.Hotel);

            var resultsModelMock = new Mock<IResultsModel>();

            var propertySearchAdaptorMock = new Mock<ISearchAdaptor>();
            propertySearchAdaptorMock.Setup(
                adaptor =>
                adaptor.Search<SearchResponse>(
                    searchmodelMock.Object, 
                    It.IsAny<CancellationToken>(), 
                    HttpContext.Current)).ReturnsAsync(resultsModelMock.Object);
            ISearchService searchService = new SearchService(
                propertySearchAdaptorMock.Object, 
                new Mock<IResultService>().Object, 
                new Mock<ILogWriter>().Object);

            //// act
            Task<List<IResultsModel>> results = searchService.Search(searchmodelMock.Object);

            //// Assert
            Assert.AreEqual(results.Result.Count, 1);
            Assert.AreEqual(results.Result[0], resultsModelMock.Object);
        }
    }
}