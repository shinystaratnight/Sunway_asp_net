namespace Web.Template.Application.Tests.Adaptors.IVectorConnect.Search
{
    using System;
    using System.Collections.Generic;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Results.ResultModels;
    using Web.Template.Application.Search.Adaptor;
    using Web.Template.Application.Search.SearchModels;

    /// <summary>
    /// Class ExtraSearchModelAdaptorTests.
    /// </summary>
    [TestFixture]
    public class ExtraSearchModelAdaptorTests
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
            var basketMock = new Mock<IBasket>();
            basketMock.SetupGet(x => x.SearchDetails)
                .Returns(
                    new SearchModel
                        {
                            Rooms =
                                new List<Room>
                                    {
                                        new Room
                                            {
                                                Adults = adults, 
                                                Children = children, 
                                                Infants = infants, 
                                                ChildAges = new List<int>()
                                            }
                                    }
                        });

            var extraBasketSearchMock = new Mock<IExtraBasketSearchModel>();

            IExtraSearchModelAdaptor searchModelAdaptor = new ExtraSearchModelAdaptor();

            //// Act
            var searchModel = searchModelAdaptor.Create(basketMock.Object, extraBasketSearchMock.Object);

            //// Assert
            Assert.AreEqual(searchModel.Adults, adults);
            Assert.AreEqual(searchModel.Children, children);
            Assert.AreEqual(searchModel.Infants, infants);
        }

        /// <summary>
        /// Create_s the should_ set extra details_ when_ set in extra basket search model.
        /// </summary>
        [TestCase]
        public void Create_Should_SetExtraDetails_When_SetInExtraBasketSearchModel()
        {
            //// Arrange
            var basketMock = new Mock<IBasket>();

            var extraBasketSearchMock = new Mock<IExtraBasketSearchModel>();
            extraBasketSearchMock.SetupGet(x => x.ExtraId).Returns(1);
            extraBasketSearchMock.SetupGet(x => x.ExtraGroupId).Returns(1);
            extraBasketSearchMock.SetupGet(x => x.ExtraTypes).Returns(new List<int> { 1, 2 });

            IExtraSearchModelAdaptor searchModelAdaptor = new ExtraSearchModelAdaptor();

            //// Act
            var searchModel = searchModelAdaptor.Create(basketMock.Object, extraBasketSearchMock.Object);

            //// Assert
            Assert.AreEqual(searchModel.ExtraId, 1);
            Assert.AreEqual(searchModel.ExtraGroupId, 1);
            Assert.AreEqual(searchModel.ExtraTypes.Count, 2);
        }

        /// <summary>
        /// Create should set flight details when flight component exists.
        /// </summary>
        [Test]
        public void Create_Should_SetFlightDetails_When_FlightComponentExists()
        {
            //// Arrange
            var basketMock = new Mock<IBasket>();
            var flightComponent = new Flight
                                      {
                                          DepartureAirportId = 1, 
                                          ArrivalAirportId = 2, 
                                          OutboundFlightDetails =
                                              new FlightDetails
                                                  {
                                                      DepartureDate = DateTime.Now.Date, 
                                                      DepartureTime = "10:00"
                                                  }, 
                                          ReturnFlightDetails =
                                              new FlightDetails
                                                  {
                                                      ArrivalDate = DateTime.Now.Date.AddDays(7), 
                                                      ArrivalTime = "19:00"
                                                  }, 
                                      };

            basketMock.SetupGet(b => b.Components).Returns(new List<IBasketComponent> { flightComponent });

            var extraBasketSearchMock = new Mock<IExtraBasketSearchModel>();

            IExtraSearchModelAdaptor searchModelAdaptor = new ExtraSearchModelAdaptor();

            //// Act
            var searchModel = searchModelAdaptor.Create(basketMock.Object, extraBasketSearchMock.Object);

            //// Assert
            Assert.AreEqual(searchModel.DepartureAirportId, 1);
            Assert.AreEqual(searchModel.ArrivalAirportId, 2);
            Assert.AreEqual(searchModel.DepartureDate, DateTime.Now.Date);
            Assert.AreEqual(searchModel.DepartureTime, "10:00");
            Assert.AreEqual(searchModel.ReturnDate, DateTime.Now.Date.AddDays(7));
            Assert.AreEqual(searchModel.ReturnTime, "19:00");
        }

        /// <summary>
        /// Create should set hotel details when hotel component exists.
        /// </summary>
        [Test]
        public void Create_Should_SetHotelDetails_When_HotelComponentExists()
        {
            //// Arrange
            var basketMock = new Mock<IBasket>();
            var hotelComponent = new Hotel()
                                     {
                                         PropertyReferenceId = 1, 
                                         GeographyLevel1Id = 1, 
                                         GeographyLevel2Id = 2, 
                                         GeographyLevel3Id = 3, 
                                         ArrivalDate = DateTime.Now.Date, 
                                         Duration = 7
                                     };

            basketMock.SetupGet(b => b.Components).Returns(new List<IBasketComponent> { hotelComponent });

            var extraBasketSearchMock = new Mock<IExtraBasketSearchModel>();

            IExtraSearchModelAdaptor searchModelAdaptor = new ExtraSearchModelAdaptor();

            //// Act
            var searchModel = searchModelAdaptor.Create(basketMock.Object, extraBasketSearchMock.Object);

            //// Assert
            Assert.AreEqual(searchModel.PropertyReferenceId, 1);
            Assert.AreEqual(searchModel.GeographyLevel1Id, 1);
            Assert.AreEqual(searchModel.GeographyLevel2Id, 2);
            Assert.AreEqual(searchModel.GeographyLevel3Id, 3);
            Assert.AreEqual(searchModel.DepartureDate, DateTime.Now.Date);
            Assert.AreEqual(searchModel.ReturnDate, DateTime.Now.Date.AddDays(7));
        }
    }
}