namespace Web.Template.Application.Tests.Adaptors.IVectorConnect.Results
{
    using System.Collections.Generic;
    using System.Web;

    using AutoMapper;

    using ivci = iVectorConnectInterface;
    using iVectorConnectInterface.Transfer;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Results.Adaptors;
    using Web.Template.Application.Results.ResultModels;

    /// <summary>
    ///     Test class for testing the property results factory
    /// </summary>
    [TestFixture]
    public class ConnectTransferResultAdaptorTests
    {
        /// <summary>
        /// Builds the connect result, used for the tests
        /// </summary>
        /// <returns>
        /// A transfer search response used for faking
        /// </returns>
        private static SearchResponse.Transfer BuildConnectResult()
        {
            var ivcSearchResult = new SearchResponse.Transfer
                                      {
                                          BookingToken = "test Booking Token", 
                                          Vehicle = "test", 
                                          ArrivalParentID = 1, 
                                          DepartureParentType = "Airport", 
                                          ArrivalParentType = "Resort", 
                                          Price = 100, 
                                          DepartureParentID = 9, 
                                          OutboundJourneyTime = "50", 
                                          MinimumCapacity = 1, 
                                          MaximumCapacity = 4, 
                                          ReturnJourneyTime = "110", 
                                          TotalCommission = 10, 
                                          TransferContractID = 10, 
                                          VehicleQuantity = 2, 
                                          SupplierDetails = new ivci.Support.SupplierDetails() { SupplierID = 5 }
                                      };

            return ivcSearchResult;
        }

        /// <summary>
        /// Create should return results with matching number when handed a full search response.
        /// </summary>
        [Test]
        public void Create_Should_ReturnResultsWithMatchingNumber_When_HandedAFullSearchResponse()
        {
            // Arrange
            SearchResponse ivcResponse = new SearchResponse() { Transfers = new List<SearchResponse.Transfer>() { BuildConnectResult(), BuildConnectResult() } };

            var mapperMock = new Mock<IMapper>();
            var resultAdaptor = new ConnectTransferResultAdaptor(mapperMock.Object);

            var searchModel = new Mock<ISearchModel>();
            searchModel.SetupGet(sm => sm.SearchMode).Returns(SearchMode.FlightPlusHotel);

            // Act
            List<IResultsModel> results = resultAdaptor.Create(ivcResponse, searchModel.Object, HttpContext.Current);

            // Assert
            Assert.AreEqual(results[0].ResultsCollection.Count, 2);
        }

        /// <summary>
        /// Create should set supplier id when set.
        /// </summary>
        [Test]
        public void Create_Should_SetSupplierID_When_Set()
        {
            // Arrange
            SearchResponse.Transfer ivcSearchResult = BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var resultAdaptor = new ConnectTransferResultAdaptor(mapperMock.Object);

            // Act
            IResult result = resultAdaptor.Create(ivcSearchResult, SearchMode.FlightPlusHotel);
            var transferResult = (TransferResult)result;

            // Assert
            Assert.AreEqual(transferResult.SupplierId, 5);
        }

        /// <summary>
        /// Create should set value of departure identifier to that of the search response when set.
        /// </summary>
        [Test]
        public void Create_Should_SetValueOfDepartureIDToThatOfTheSearchResponse_When_Set()
        {
            // Arrange
            SearchResponse.Transfer ivcSearchResult = BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var resultAdaptor = new ConnectTransferResultAdaptor(mapperMock.Object);

            // Act
            IResult result = resultAdaptor.Create(ivcSearchResult, SearchMode.FlightPlusHotel);
            var transferResult = (TransferResult)result;

            // Assert
            Assert.AreEqual(transferResult.DepartureId, 9);
        }

        /// <summary>
        /// Create should set value of departure parent type to that of the search response when_ set.
        /// </summary>
        [Test]
        public void Create_Should_SetValueOfDepartureParentTypeToThatOfTheSearchResponse_When_Set()
        {
            // Arrange
            SearchResponse.Transfer ivcSearchResult = BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var resultAdaptor = new ConnectTransferResultAdaptor(mapperMock.Object);

            // Act
            IResult result = resultAdaptor.Create(ivcSearchResult, SearchMode.FlightPlusHotel);
            var transferResult = (TransferResult)result;

            // Assert
            Assert.AreEqual(transferResult.DepartureParentType, "Airport");
        }

        /// <summary>
        /// Create should set value of maximum capacity to that of the search response when set.
        /// </summary>
        [Test]
        public void Create_Should_SetValueOfMaximumCapacityToThatOfTheSearchResponse_When_Set()
        {
            // Arrange
            SearchResponse.Transfer ivcSearchResult = BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var resultAdaptor = new ConnectTransferResultAdaptor(mapperMock.Object);

            // Act
            IResult result = resultAdaptor.Create(ivcSearchResult, SearchMode.FlightPlusHotel);
            var transferResult = (TransferResult)result;

            // Assert
            Assert.AreEqual(transferResult.MaximumCapacity, 4);
        }

        /// <summary>
        /// Create should set value of minimum capacity to that of the search response when_ set.
        /// </summary>
        [Test]
        public void Create_Should_SetValueOfMinimumCapacityToThatOfTheSearchResponse_When_Set()
        {
            // Arrange
            SearchResponse.Transfer ivcSearchResult = BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var resultAdaptor = new ConnectTransferResultAdaptor(mapperMock.Object); ;

            // Act
            IResult result = resultAdaptor.Create(ivcSearchResult, SearchMode.FlightPlusHotel);
            var transferResult = (TransferResult)result;

            // Assert
            Assert.AreEqual(transferResult.MinimumCapacity, 1);
        }

        /// <summary>
        /// Create should set value of outbound journey time to that of the search response when set.
        /// </summary>
        [Test]
        public void Create_Should_SetValueOfOutboundJourneyTimeToThatOfTheSearchResponse_When_Set()
        {
            // Arrange
            SearchResponse.Transfer ivcSearchResult = BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var resultAdaptor = new ConnectTransferResultAdaptor(mapperMock.Object);

            // Act
            IResult result = resultAdaptor.Create(ivcSearchResult, SearchMode.FlightPlusHotel);
            var transferResult = (TransferResult)result;

            // Assert
            Assert.AreEqual(transferResult.OutboundJourneyTime, "50");
        }

        /// <summary>
        /// Create should set value of price to that of the search response when set.
        /// </summary>
        [Test]
        public void Create_Should_SetValueOfPriceToThatOfTheSearchResponse_When_Set()
        {
            // Arrange
            SearchResponse.Transfer ivcSearchResult = BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var resultAdaptor = new ConnectTransferResultAdaptor(mapperMock.Object);

            // Act
            IResult result = resultAdaptor.Create(ivcSearchResult, SearchMode.FlightPlusHotel);
            var transferResult = (TransferResult)result;

            // Assert
            Assert.AreEqual(transferResult.TotalPrice, 100);
        }

        /// <summary>
        /// Create should set value of result arrival parent identifier to that of the search response when set.
        /// </summary>
        [Test]
        public void Create_Should_SetValueOfResultArrivalParentIDToThatOfTheSearchResponse_When_Set()
        {
            // Arrange
            SearchResponse.Transfer ivcSearchResult = BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var resultAdaptor = new ConnectTransferResultAdaptor(mapperMock.Object);

            // Act
            IResult result = resultAdaptor.Create(ivcSearchResult, SearchMode.FlightPlusHotel);
            var transferResult = (TransferResult)result;

            // Assert
            Assert.AreEqual(transferResult.ArrivalParentId, 1);
        }

        /// <summary>
        /// Create should set value of result arrival parent type to that of the search response when set.
        /// </summary>
        [Test]
        public void Create_Should_SetValueOfResultArrivalParentTypeToThatOfTheSearchResponse_When_Set()
        {
            // Arrange
            SearchResponse.Transfer ivcSearchResult = BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var resultAdaptor = new ConnectTransferResultAdaptor(mapperMock.Object);

            // Act
            IResult result = resultAdaptor.Create(ivcSearchResult, SearchMode.FlightPlusHotel);
            var transferResult = (TransferResult)result;

            // Assert
            Assert.AreEqual(transferResult.ArrivalParentType, "Resort");
        }

        /// <summary>
        /// Create should set value of result booking token to that of the search response when set.
        /// </summary>
        [Test]
        public void Create_Should_SetValueOfResultBookingTokenToThatOfTheSearchResponse_When_Set()
        {
            // Arrange
            SearchResponse.Transfer ivcSearchResult = BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var resultAdaptor = new ConnectTransferResultAdaptor(mapperMock.Object);
            // Act
            IResult result = resultAdaptor.Create(ivcSearchResult, SearchMode.FlightPlusHotel);
            var transferResult = (TransferResult)result;

            // Assert
            Assert.AreEqual(transferResult.BookingToken, "test Booking Token");
        }

        /// <summary>
        /// Create should set value of result vehicle to that of the search response when set.
        /// </summary>
        [Test]
        public void Create_Should_SetValueOfResultVehicleToThatOfTheSearchResponse_When_Set()
        {
            // Arrange
            SearchResponse.Transfer ivcSearchResult = BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var resultAdaptor = new ConnectTransferResultAdaptor(mapperMock.Object);

            // Act
            IResult result = resultAdaptor.Create(ivcSearchResult, SearchMode.FlightPlusHotel);
            var transferResult = (TransferResult)result;

            // Assert
            Assert.AreEqual(transferResult.Vehicle, "test");
        }

        /// <summary>
        /// Create should set value of return journey time to that of the search response when set.
        /// </summary>
        [Test]
        public void Create_Should_SetValueOfReturnJourneyTimeToThatOfTheSearchResponse_When_Set()
        {
            // Arrange
            SearchResponse.Transfer ivcSearchResult = BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var resultAdaptor = new ConnectTransferResultAdaptor(mapperMock.Object);

            // Act
            IResult result = resultAdaptor.Create(ivcSearchResult, SearchMode.FlightPlusHotel);
            var transferResult = (TransferResult)result;

            // Assert
            Assert.AreEqual(transferResult.ReturnJourneyTime, "110");
        }

        /// <summary>
        /// Create the should set value of vehicle quantity to that of the search response when set.
        /// </summary>
        [Test]
        public void Create_Should_SetValueOfVehicleQuantityToThatOfTheSearchResponse_When_Set()
        {
            // Arrange
            SearchResponse.Transfer ivcSearchResult = BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var resultAdaptor = new ConnectTransferResultAdaptor(mapperMock.Object);

            // Act
            IResult result = resultAdaptor.Create(ivcSearchResult, SearchMode.FlightPlusHotel);
            var transferResult = (TransferResult)result;

            // Assert
            Assert.AreEqual(transferResult.VehicleQuantity, 2);
        }
    }
}