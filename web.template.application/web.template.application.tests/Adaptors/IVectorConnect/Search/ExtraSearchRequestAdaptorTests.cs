namespace Web.Template.Application.Tests.Adaptors.IVectorConnect.Search
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using iVectorConnectInterface.Extra;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.IVectorConnect.Requests;
    using Web.Template.Application.Search.Adaptor;

    /// <summary>
    /// Class ExtraSearchRequestAdaptorTests.
    /// </summary>
    [TestFixture]
    public class ExtraSearchRequestAdaptorTests
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
            var searchModelMock = new Mock<IExtraSearchModel>();

            searchModelMock.SetupGet(x => x.Adults).Returns(adults);
            searchModelMock.SetupGet(x => x.Children).Returns(children);
            searchModelMock.SetupGet(x => x.Infants).Returns(infants);

            IExtraSearchRequestAdaptor searchRequestAdapter =
                new ExtraSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            //// Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            //// Assert
            Assert.AreEqual(searchRequest.GuestConfiguration.Adults, adults);
            Assert.AreEqual(searchRequest.GuestConfiguration.Children, children);
            Assert.AreEqual(searchRequest.GuestConfiguration.Infants, infants);
        }

        /// <summary>
        /// Create should populate aiport values when set.
        /// </summary>
        [Test]
        public void Create_Should_PopulateAiportValues_When_Set()
        {
            //// Arrange
            var searchModelMock = new Mock<IExtraSearchModel>();

            searchModelMock.SetupGet(x => x.DepartureAirportId).Returns(1);
            searchModelMock.SetupGet(x => x.ArrivalAirportId).Returns(2);

            IExtraSearchRequestAdaptor searchRequestAdapter =
                new ExtraSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            //// Assert
            Assert.AreEqual(searchRequest.DepartureAirportID, 1);
            Assert.AreEqual(searchRequest.ArrivalAirportID, 2);
        }

        /// <summary>
        /// Create should populate extra values when set.
        /// </summary>
        [Test]
        public void Create_Should_PopulateBookingValues_When_Set()
        {
            //// Arrange
            var searchModelMock = new Mock<IExtraSearchModel>();

            searchModelMock.SetupGet(x => x.BookingType).Returns("Hotel");
            searchModelMock.SetupGet(x => x.BookingPrice).Returns(300);

            IExtraSearchRequestAdaptor searchRequestAdapter =
                new ExtraSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            //// Assert
            Assert.AreEqual(searchRequest.BookingType, "Hotel");
            Assert.AreEqual(searchRequest.BookingPrice, 300);
        }

        /// <summary>
        /// Create should populate extra values when set.
        /// </summary>
        [Test]
        public void Create_Should_PopulateExtraValues_When_Set()
        {
            //// Arrange
            var searchModelMock = new Mock<IExtraSearchModel>();

            searchModelMock.SetupGet(x => x.ExtraId).Returns(1);
            searchModelMock.SetupGet(x => x.ExtraGroupId).Returns(2);

            var extraTypeIds = new List<int>() { 1, 2 };
            searchModelMock.SetupGet(x => x.ExtraTypes).Returns(extraTypeIds);

            IExtraSearchRequestAdaptor searchRequestAdapter =
                new ExtraSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            //// Assert
            Assert.AreEqual(searchRequest.ExtraID, 1);
            Assert.AreEqual(searchRequest.ExtraGroupID, 2);
            Assert.AreEqual(searchRequest.ExtraTypes.Count, extraTypeIds.Count);
        }

        /// <summary>
        /// Create should populate geography values when set.
        /// </summary>
        [Test]
        public void Create_Should_PopulateGeographyValues_When_Set()
        {
            //// Arrange
            var searchModelMock = new Mock<IExtraSearchModel>();

            searchModelMock.SetupGet(x => x.GeographyLevel1Id).Returns(1);
            searchModelMock.SetupGet(x => x.GeographyLevel2Id).Returns(2);
            searchModelMock.SetupGet(x => x.GeographyLevel3Id).Returns(3);

            IExtraSearchRequestAdaptor searchRequestAdapter =
                new ExtraSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            //// Assert
            Assert.AreEqual(searchRequest.GeographyLevel1ID, 1);
            Assert.AreEqual(searchRequest.GeographyLevel2ID, 2);
            Assert.AreEqual(searchRequest.GeographyLevel3ID, 3);
        }

        /// <summary>
        /// Create should populate values.
        /// </summary>
        [Test]
        public void Create_Should_PopulateMiscValues_When_Set()
        {
            //// Arrange
            var searchModelMock = new Mock<IExtraSearchModel>();

            searchModelMock.SetupGet(x => x.DepartureDate).Returns(DateTime.Now.Date);
            searchModelMock.SetupGet(x => x.ReturnDate).Returns(DateTime.Now.Date.AddDays(7));
            searchModelMock.SetupGet(x => x.ReturnTime).Returns("11:00");
            searchModelMock.SetupGet(x => x.DepartureTime).Returns("13:00");

            IExtraSearchRequestAdaptor searchRequestAdapter =
                new ExtraSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            //// Assert
            Assert.AreEqual(DateTime.Now.Date, searchRequest.DepartureDate);
            Assert.AreEqual(DateTime.Now.Date.AddDays(7), searchRequest.ReturnDate);
            Assert.AreEqual("11:00", searchRequest.ReturnTime);
            Assert.AreEqual("13:00", searchRequest.DepartureTime);
        }

        /// <summary>
        /// Create should populate property value when set.
        /// </summary>
        [Test]
        public void Create_Should_PopulatePropertyValue_When_Set()
        {
            //// Arrange
            var searchModelMock = new Mock<IExtraSearchModel>();

            searchModelMock.SetupGet(x => x.PropertyReferenceId).Returns(1);

            IExtraSearchRequestAdaptor searchRequestAdapter =
                new ExtraSearchRequestAdaptor(new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModelMock.Object, HttpContext.Current);

            //// Assert
            Assert.AreEqual(searchRequest.PropertyReferenceID, 1);
        }
    }
}