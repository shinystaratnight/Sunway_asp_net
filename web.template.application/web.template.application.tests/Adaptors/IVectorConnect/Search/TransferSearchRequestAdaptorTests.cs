namespace Web.Template.Application.Tests.Adaptors.IVectorConnect.Search
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using iVectorConnectInterface.Transfer;

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
    public class TransferSearchRequestAdaptorTests
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

            ISearchRequestAdapter searchRequestAdapter = new TransferSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

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

            ISearchRequestAdapter searchRequestAdapter = new TransferSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            //// Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            ////Assert
            Assert.AreEqual(searchRequest.GuestConfiguration.Adults, room1Adults + room2Adults);
            Assert.AreEqual(searchRequest.GuestConfiguration.Children, room1Children + room2Children);
            Assert.AreEqual(searchRequest.GuestConfiguration.Infants, room1Infants + room2Infants);
        }

        /// <summary>
        /// Create should set departure airport group not airport when departure type is airport group.
        /// </summary>
        /// <param name="departureParentType">Type of the departure parent.</param>
        /// <param name="departureParentId">The departure parent identifier.</param>
        [TestCase(LocationType.Airport, 3)]
        [TestCase(LocationType.Resort, 1)]
        [TestCase(LocationType.Region, 7)]
        [TestCase(LocationType.Property, 5)]
        public void Create_Should_SetCorrectDepartureParentTypeAndID_When_Set(LocationType departureParentType, int departureParentId)
        {
            // Arrange
            var searchModelMock = new Mock<ISearchModel>();
            searchModelMock.SetupGet(x => x.DepartureType).Returns(departureParentType);
            searchModelMock.SetupGet(x => x.DepartureID).Returns(departureParentId);

            ISearchRequestAdapter searchRequestAdapter = new TransferSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            // Assert
            Assert.AreEqual(searchRequest.DepartureParentID, departureParentId);
            Assert.AreEqual(searchRequest.DepartureParentType, departureParentType.ToString());
        }

        /// <summary>
        /// Create should set departure airport group not airport when departure type is airport group.
        /// </summary>
        /// <param name="arrivalParentType">Type of the arrival parent.</param>
        /// <param name="arrivalParentId">The arrival parent identifier.</param>
        [TestCase(LocationType.Airport, 7)]
        [TestCase(LocationType.Resort, 100)]
        [TestCase(LocationType.Region, 40)]
        [TestCase(LocationType.Property, 8)]
        public void Create_Should_SetCorrectArrivalParentTypeAndID_When_Set(LocationType arrivalParentType, int arrivalParentId)
        {
            // Arrange
            var searchModelMock = new Mock<ISearchModel>();
            searchModelMock.SetupGet(x => x.ArrivalType).Returns(arrivalParentType);
            searchModelMock.SetupGet(x => x.ArrivalID).Returns(arrivalParentId);

            ISearchRequestAdapter searchRequestAdapter = new TransferSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            // Assert
            Assert.AreEqual(searchRequest.ArrivalParentID, arrivalParentId);
            Assert.AreEqual(searchRequest.ArrivalParentType, arrivalParentType.ToString());
        }

        /// <summary>
        /// Create should populate values when search mode is flight plus hotel.
        /// </summary>
        [Test]
        public void Create_Should_PopulateMiscValues_When_Set()
        {
            //// Arrange
            var searchModelMock = new Mock<ISearchModel>();

            searchModelMock.SetupGet(x => x.SearchMode).Returns(SearchMode.Transfer);
            searchModelMock.SetupGet(x => x.DepartureDate).Returns(DateTime.Now.Date);
            searchModelMock.SetupGet(x => x.Duration).Returns(7);
            searchModelMock.SetupGet(x => x.ReturnTime).Returns("11:00");
            searchModelMock.SetupGet(x => x.DepartureTime).Returns("13:00");

            ISearchRequestAdapter searchRequestAdapter = new TransferSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            Assert.AreEqual(searchRequest.DepartureDate, DateTime.Now.Date);
            Assert.AreEqual(searchRequest.ReturnDate, DateTime.Now.Date.AddDays(7));
            Assert.AreEqual(searchRequest.OneWay, false);
            Assert.AreEqual(searchRequest.ReturnTime, "11:00");
            Assert.AreEqual(searchRequest.DepartureTime, "13:00");
        }

        /// <summary>
        /// Responses the type should be flight when evaluated.
        /// </summary>
        [Test]
        public void ResponseType_Should_BeTransfer_When_Evaluated()
        {
            //// Arrange
            ISearchRequestAdapter searchRequestAdapter = new TransferSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            ////Assert
            Assert.AreEqual(searchRequestAdapter.ResponseType, typeof(SearchResponse));
        }
    }
}