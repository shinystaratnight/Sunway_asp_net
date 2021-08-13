namespace Web.Template.Application.Tests.Adaptors.IVectorConnect.Search
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using iVectorConnectInterface.Flight;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Search;
    using Web.Template.Application.IVectorConnect.Requests;
    using Web.Template.Application.Search.Adaptor;
    using Web.Template.Application.Search.SearchModels;

    /// <summary>
    /// Tests for the Flight Search Request Adaptor Class
    /// </summary>
    [TestFixture]
    public class FlightSearchRequestAdaptorTests
    {
        /// <summary>
        /// Create should build guest details when provided guests.
        /// </summary>
        /// <param name="adults">The adults.</param>
        /// <param name="children">The children.</param>
        /// <param name="infants">The infants.</param>
        [TestCase(2, 0, 0)]
        [TestCase(4, 0, 0)]
        [TestCase(2, 1, 0)]
        [TestCase(2, 1, 1)]
        [TestCase(2, 0, 2)]
        public void Create_Should_BuildGuestDetails_When_ProvidedGuests(int adults, int children, int infants)
        {
            //// Arrange
            var searchModelMock = new Mock<ISearchModel>();
            searchModelMock.SetupGet(x => x.Rooms).Returns(new List<Room> { new Room { Adults = adults, Children = children, Infants = infants, ChildAges = new List<int>() } });

            ISearchRequestAdapter searchRequestAdapter = new FlightSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            //// Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            ////Assert
            Assert.AreEqual(searchRequest.GuestConfiguration.Adults, adults);
            Assert.AreEqual(searchRequest.GuestConfiguration.Children, children);
            Assert.AreEqual(searchRequest.GuestConfiguration.Infants, infants);
        }

        /// <summary>
        /// Create the should build guest details when more than one room on search model.
        /// </summary>
        /// <param name="room1Adults">The room1 adults.</param>
        /// <param name="room1Children">The room1 children.</param>
        /// <param name="room1Infants">The room1 infants.</param>
        /// <param name="room2Adults">The room2 adults.</param>
        /// <param name="room2Children">The room2 children.</param>
        /// <param name="room2Infants">The room2 infants.</param>
        [TestCase(2, 0, 0, 2, 0, 0)]
        [TestCase(4, 0, 0, 6, 0, 0)]
        [TestCase(2, 1, 0, 3, 1, 1)]
        [TestCase(2, 1, 1, 1, 0, 1)]
        [TestCase(2, 0, 2, 1, 1, 1)]
        [TestCase(3, 3, 3, 3, 3, 3)]
        public void Create_Should_BuildGuestDetails_When_MoreThanOneRoomOnSearchModel(int room1Adults, int room1Children, int room1Infants, int room2Adults, int room2Children, int room2Infants)
        {
            //// Arrange
            var searchModelMock = new Mock<ISearchModel>();
            searchModelMock.SetupGet(x => x.Rooms)
                .Returns(
                    new List<Room>
                        {
                            new Room { Adults = room1Adults, Children = room1Children, Infants = room1Infants, ChildAges = new List<int>() }, 
                            new Room { Adults = room2Adults, Children = room2Children, Infants = room2Infants, ChildAges = new List<int>() }
                        });

            ISearchRequestAdapter searchRequestAdapter = new FlightSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            //// Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            ////Assert
            Assert.AreEqual(searchRequest.GuestConfiguration.Adults, room1Adults + room2Adults);
            Assert.AreEqual(searchRequest.GuestConfiguration.Children, room1Children + room2Children);
            Assert.AreEqual(searchRequest.GuestConfiguration.Infants, room1Infants + room2Infants);
        }

        /// <summary>
        /// Create should populate values when search mode is flight plus hotel.
        /// </summary>
        [Test]
        public void Create_Should_PopulateMiscValues_When_SearchModeIsFlightPlusHotel()
        {
            //// Arrange
            var searchModelMock = new Mock<ISearchModel>();

            searchModelMock.SetupGet(x => x.SearchMode).Returns(SearchMode.Flight);
            searchModelMock.SetupGet(x => x.DepartureDate).Returns(DateTime.Now.Date);
            searchModelMock.SetupGet(x => x.Duration).Returns(7);
            searchModelMock.SetupGet(x => x.MinRating).Returns(2);
            searchModelMock.SetupGet(x => x.MealBasisID).Returns(9);

            ISearchRequestAdapter searchRequestAdapter = new FlightSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            Assert.AreEqual(searchRequest.DepartureDate, DateTime.Now.Date);

            Assert.AreEqual(searchRequest.Duration, 7);
            Assert.AreEqual(searchRequest.OneWay, false);
            Assert.AreEqual(searchRequest.AllowMultisectorFlights, true);
            Assert.AreEqual(searchRequest.FlightAndHotel, false);
        }

        /// <summary>
        /// Create should set departure airport group not airport when departure type is airport group.
        /// </summary>
        [Test]
        public void Create_Should_SetDepartureAirportGroupNotAirport_When_DepartureTypeIsAirportGroup()
        {
            // Arrange
            var searchModelMock = new Mock<ISearchModel>();
            searchModelMock.SetupGet(x => x.DepartureType).Returns(LocationType.AirportGroup);
            searchModelMock.SetupGet(x => x.DepartureID).Returns(1);

            ISearchRequestAdapter searchRequestAdapter = new FlightSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            // Assert
            Assert.AreEqual(searchRequest.DepartureAirportID, 0);
            Assert.AreEqual(searchRequest.DepartureAirportGroupID, 1);
        }

        /// <summary>
        /// Create should set departure airport not departure airport group when departure type is airport.
        /// </summary>
        [Test]
        public void Create_Should_SetDepartureAirportNotDepartureAirportGroup_When_DepartureTypeIsAirport()
        {
            // Arrange
            var searchModelMock = new Mock<ISearchModel>();
            searchModelMock.SetupGet(x => x.DepartureType).Returns(LocationType.Airport);
            searchModelMock.SetupGet(x => x.DepartureID).Returns(1);

            ISearchRequestAdapter searchRequestAdapter = new FlightSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            // Assert
            Assert.AreEqual(searchRequest.DepartureAirportID, 1);
            Assert.AreEqual(searchRequest.DepartureAirportGroupID, 0);
        }

        /// <summary>
        /// Create should set only arrival airport when arrival type is airport.
        /// </summary>
        [Test]
        public void Create_Should_SetOnlyArrivalAirport_When_ArrivalTypeIsAirport()
        {
            // Arrange
            var searchModelMock = new Mock<ISearchModel>();
            searchModelMock.SetupGet(x => x.ArrivalID).Returns(4);
            searchModelMock.SetupGet(x => x.ArrivalType).Returns(LocationType.Airport);

            ISearchRequestAdapter searchRequestAdapter = new FlightSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            // Assert
            Assert.AreEqual(searchRequest.ArrivalAirportID, 4);
            Assert.AreEqual(searchRequest.ArrivalAirportGroupID, 0);
            Assert.AreEqual(searchRequest.Resorts.Count, 0);
            Assert.AreEqual(searchRequest.RegionID, 0);
        }

        /// <summary>
        /// Create should set only arrival airport group when arrival type is airport group.
        /// </summary>
        [Test]
        public void Create_Should_SetOnlyArrivalAirportGroup_When_ArrivalTypeIsAirportGroup()
        {
            // Arrange
            var searchModelMock = new Mock<ISearchModel>();
            searchModelMock.SetupGet(x => x.ArrivalID).Returns(5);
            searchModelMock.SetupGet(x => x.ArrivalType).Returns(LocationType.AirportGroup);

            ISearchRequestAdapter searchRequestAdapter = new FlightSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            // Assert
            Assert.AreEqual(searchRequest.ArrivalAirportID, 0);
            Assert.AreEqual(searchRequest.ArrivalAirportGroupID, 5);
            Assert.AreEqual(searchRequest.Resorts.Count, 0);
            Assert.AreEqual(searchRequest.RegionID, 0);
        }

        /// <summary>
        /// Create should set only region id when arrival type is region.
        /// </summary>
        [Test]
        public void Create_Should_SetOnlyRegionID_When_ArrivalTypeIsRegion()
        {
            // Arrange
            var searchModelMock = new Mock<ISearchModel>();
            searchModelMock.SetupGet(x => x.ArrivalID).Returns(3);
            searchModelMock.SetupGet(x => x.ArrivalType).Returns(LocationType.Region);

            ISearchRequestAdapter searchRequestAdapter = new FlightSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            // Assert
            Assert.AreEqual(searchRequest.ArrivalAirportID, 0);
            Assert.AreEqual(searchRequest.ArrivalAirportGroupID, 0);
            Assert.AreEqual(searchRequest.Resorts.Count, 0);
            Assert.AreEqual(searchRequest.RegionID, 3);
        }

        /// <summary>
        /// Create should set only resort id when arrival type is resort.
        /// </summary>
        [Test]
        public void Create_Should_SetOnlyResortID_When_ArrivalTypeIsResort()
        {
            // Arrange
            var searchModelMock = new Mock<ISearchModel>();
            searchModelMock.SetupGet(x => x.ArrivalID).Returns(50);
            searchModelMock.SetupGet(x => x.ArrivalType).Returns(LocationType.Resort);

            ISearchRequestAdapter searchRequestAdapter = new FlightSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            // Assert
            Assert.AreEqual(searchRequest.ArrivalAirportID, 0);
            Assert.AreEqual(searchRequest.ArrivalAirportGroupID, 0);
            Assert.AreEqual(searchRequest.Resorts.Count, 1);
            Assert.AreEqual(searchRequest.Resorts[0], 50);
            Assert.AreEqual(searchRequest.RegionID, 0);
        }

        /// <summary>
        /// Responses the type should be flight when evaluated.
        /// </summary>
        [Test]
        public void ResponseType_Should_BeFlight_When_Evaluated()
        {
            //// Arrange
            ISearchRequestAdapter searchRequestAdapter = new FlightSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            ////Assert
            Assert.AreEqual(searchRequestAdapter.ResponseType, typeof(SearchResponse));
        }
    }
}