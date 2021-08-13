namespace Web.Template.Application.Tests.Adaptors.IVectorConnect.Search
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using iVectorConnectInterface.Property;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Search;
    using Web.Template.Application.IVectorConnect.Requests;
    using Web.Template.Application.Repositories.Domain.Geography;
    using Web.Template.Application.Search.Adaptor;
    using Web.Template.Application.Search.SearchModels;
    using Web.Template.Domain.Entities.Geography;

    /// <summary>
    ///     A Test class used for testing the property search request factory
    /// </summary>
    [TestFixture]
    internal class PropertySearchRequestAdapterTests
    {
        /// <summary>
        ///     Creates the fake search model.
        /// </summary>
        /// <returns>
        ///     A search model
        /// </returns>
        private static ISearchModel CreateFakeSearchModel()
        {
            ISearchModel searchModel = new SearchModel
                                           {
                                               SearchMode = SearchMode.Hotel, 
                                               ArrivalID = 5, 
                                               ArrivalType = LocationType.Region, 
                                               DepartureDate = DateTime.Now.Date, 
                                               Duration = 7, 
                                               MinRating = 2, 
                                               MealBasisID = 9, 
                                               Rooms =
                                                   new List<Room>
                                                       {
                                                           new Room
                                                               {
                                                                   Adults = 2, 
                                                                   Children = 0, 
                                                                   Infants = 0, 
                                                                   ChildAges = new List<int>()
                                                               }
                                                       }
                                           };

            return searchModel;
        }

        /// <summary>
        ///     Fakes the geography group.
        /// </summary>
        /// <returns>A fake geography group</returns>
        private static List<GeographyGrouping> FakeGeographyGroup()
        {
            return new List<GeographyGrouping>
                       {
                           ////new GeographyGrouping
                           ////    {
                           ////        Geographies =
                           ////            new List<Geography>
                           ////                {
                           ////                    new Geography
                           ////                        {
                           ////                            Level1Name = "level1NameA", 
                           ////                            Level1Id = 1, 
                           ////                            Level2Name = "level2NameA", 
                           ////                            Level2Id = 2, 
                           ////                            Level3Name = "level3NameA", 
                           ////                            Level3Id = 3
                           ////                        }, 
                           ////                    new Geography
                           ////                        {
                           ////                            Level1Name = "level1NameB", 
                           ////                            Level1Id = 11, 
                           ////                            Level2Name = "level2NameB", 
                           ////                            Level2Id = 22, 
                           ////                            Level3Name = "level3NameB", 
                           ////                            Level3Id = 33
                           ////                        }
                           ////                }, 
                           ////        Id = 1, 
                           ////        Name = "TestGroupA", 
                           ////        ShowInSearch = true
                           ////    }, 
                           ////new GeographyGrouping
                           ////    {
                           ////        Geographies =
                           ////            new List<Geography>
                           ////                {
                           ////                    new Geography
                           ////                        {
                           ////                            Level1Name = "level1NameC", 
                           ////                            Level1Id = 4, 
                           ////                            Level2Name = "level2NameC", 
                           ////                            Level2Id = 5, 
                           ////                            Level3Name = "level3NameC", 
                           ////                            Level3Id = 6
                           ////                        }, 
                           ////                    new Geography
                           ////                        {
                           ////                            Level1Name = "level1NameD", 
                           ////                            Level1Id = 44, 
                           ////                            Level2Name = "level2NameD", 
                           ////                            Level2Id = 55, 
                           ////                            Level3Name = "level3NameD", 
                           ////                            Level3Id = 66
                           ////                        }
                           ////                }, 
                           ////        Id = 2, 
                           ////        Name = "TestGroupB", 
                           ////        ShowInSearch = true
                           ////    }
                       };
        }

        /// <summary>
        ///     Creates the fake flight plus hotel search model.
        /// </summary>
        /// <returns>A search Model</returns>
        private ISearchModel CreateFakeFlightPlusHotelSearchModel()
        {
            ISearchModel searchModel = new SearchModel
                                           {
                                               SearchMode = SearchMode.FlightPlusHotel, 
                                               DepartureType = LocationType.Airport, 
                                               DepartureID = 1, 
                                               ArrivalID = 5, 
                                               ArrivalType = LocationType.Resort, 
                                               DepartureDate = DateTime.Now.Date, 
                                               Duration = 7, 
                                               MinRating = 2, 
                                               MealBasisID = 9, 
                                               Rooms =
                                                   new List<Room>
                                                       {
                                                           new Room
                                                               {
                                                                   Adults = 2, 
                                                                   Children = 0, 
                                                                   Infants = 0, 
                                                                   ChildAges = new List<int>()
                                                               }
                                                       }
                                           };

            return searchModel;
        }

        /// <summary>
        ///     Creates the fake flight plus hotel search model with airport group.
        /// </summary>
        /// <returns>A search Model</returns>
        private ISearchModel CreateFakeFlightPlusHotelSearchModelWithAirportGroup()
        {
            ISearchModel searchModel = new SearchModel
                                           {
                                               SearchMode = SearchMode.FlightPlusHotel, 
                                               DepartureType = LocationType.AirportGroup, 
                                               DepartureID = 1, 
                                               ArrivalID = 5, 
                                               ArrivalType = LocationType.Resort, 
                                               DepartureDate = DateTime.Now.Date, 
                                               Duration = 7, 
                                               MinRating = 2, 
                                               MealBasisID = 9, 
                                               Rooms =
                                                   new List<Room>
                                                       {
                                                           new Room
                                                               {
                                                                   Adults = 2, 
                                                                   Children = 0, 
                                                                   Infants = 0, 
                                                                   ChildAges = new List<int>()
                                                               }
                                                       }
                                           };

            return searchModel;
        }

        /// <summary>
        ///     Creates the a fake model with longitude and latitude values populated.
        /// </summary>
        /// <returns>A Search Model</returns>
        private ISearchModel CreateFakeLongLatModel()
        {
            ISearchModel searchModel = new SearchModel
                                           {
                                               SearchMode = SearchMode.Hotel, 
                                               ArrivalLongitude = 5, 
                                               ArrivalLatitude = 10, 
                                               ArrivalRadius = 2, 
                                               ArrivalType = LocationType.GeoCode, 
                                               DepartureDate = DateTime.Now.Date, 
                                               Duration = 7, 
                                               MinRating = 2, 
                                               MealBasisID = 9, 
                                               Rooms =
                                                   new List<Room>
                                                       {
                                                           new Room
                                                               {
                                                                   Adults = 2, 
                                                                   Children = 0, 
                                                                   Infants = 0, 
                                                                   ChildAges = new List<int>()
                                                               }
                                                       }
                                           };

            return searchModel;
        }

        /// <summary>
        ///     Create should build a flight details object when the search mode is flight plus hotel
        /// </summary>
        [Test]
        public void Create_Should_BuildFlightDetails_When_SearchModeIsFlightPlusHotel()
        {
            // Arrange
            ISearchModel searchModel = this.CreateFakeFlightPlusHotelSearchModel();
            ISearchRequestAdapter searchRequestAdapter =
                new PropertySearchRequestAdapter(
                    new Mock<GeographyGroupingRepository>(null).Object, 
                    new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModel, HttpContext.Current);

            Assert.AreEqual(searchRequest.SearchMode, "FlightPlusHotel");
            Assert.AreEqual(searchRequest.AirportID, 0);
            Assert.AreEqual(searchRequest.AirportGroupID, 0);
            Assert.AreEqual(searchRequest.RegionID, 0);
            Assert.AreEqual(searchRequest.Resorts.Count, 1);
            Assert.AreEqual(searchRequest.Resorts[0], 5);
            Assert.AreEqual(searchRequest.PropertyReferenceID, 0);

            Assert.IsNotNull(searchRequest.FlightDetails);
            Assert.AreEqual(searchRequest.FlightDetails.DepartureAirportID, 1);
            Assert.AreEqual(searchRequest.FlightDetails.DepartureAirportGroupID, 0);
        }

        /// <summary>
        ///     Create should build a flight details object and populate the airport group when the
        ///     search mode is flight plus hotel and the departure type is group
        /// </summary>
        [Test]
        public void Create_Should_BuildFlightDetailsWithAirportGroup_When_DepartureTypeIsAirportGroup()
        {
            // Arrange
            ISearchModel searchModel = this.CreateFakeFlightPlusHotelSearchModelWithAirportGroup();
            ISearchRequestAdapter searchRequestAdapter =
                new PropertySearchRequestAdapter(
                    new Mock<GeographyGroupingRepository>(null).Object, 
                    new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModel, HttpContext.Current);

            Assert.IsNotNull(searchRequest.FlightDetails);
            Assert.AreEqual(searchRequest.FlightDetails.DepartureAirportID, 0);
            Assert.AreEqual(searchRequest.FlightDetails.DepartureAirportGroupID, 1);
        }

        /// <summary>
        ///     create should populate the geo location values when the arrival type is geocode.
        /// </summary>
        [Test]
        public void Create_Should_PopulateGeoValues_When_ArrivalTypeIsGeocode()
        {
            // Arrange
            ISearchModel searchModel = this.CreateFakeLongLatModel();
            ISearchRequestAdapter searchRequestAdapter =
                new PropertySearchRequestAdapter(
                    new Mock<GeographyGroupingRepository>(null).Object, 
                    new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModel, HttpContext.Current);

            Assert.AreEqual(searchRequest.Longitude, 5);
            Assert.AreEqual(searchRequest.Latitude, 10);
            Assert.AreEqual(searchRequest.Radius, 2);
        }

        /// <summary>
        /// Login details should be populated correctly from config.
        /// </summary>
        [Test]
        public void Create_Should_PopulateLoginFromConfig_When_ValuesInConfig()
        {
            // Arrange
            ISearchModel searchModel = CreateFakeSearchModel();
            var loginDetailsFactoryMock = new Mock<IConnectLoginDetailsFactory>();

            ISearchRequestAdapter searchRequestAdapter =
                new PropertySearchRequestAdapter(
                    new Mock<GeographyGroupingRepository>(null).Object, 
                    loginDetailsFactoryMock.Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModel, HttpContext.Current);

            loginDetailsFactoryMock.Verify(ldf => ldf.Create(HttpContext.Current, false), Times.AtLeastOnce);
        }

        /// <summary>
        ///     Create should populate values with data provided
        /// </summary>
        [Test]
        public void Create_Should_PopulateValues_WhenProvidedObjectWithValuesSet()
        {
            // Arrange
            ISearchModel searchModel = CreateFakeSearchModel();
            ISearchRequestAdapter searchRequestAdapter =
                new PropertySearchRequestAdapter(
                    new Mock<GeographyGroupingRepository>(null).Object, 
                    new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModel, HttpContext.Current);

            // Assert
            Assert.AreEqual(searchRequest.SearchMode, "Hotel");
            Assert.AreEqual(searchRequest.AirportID, 0);
            Assert.AreEqual(searchRequest.AirportGroupID, 0);
            Assert.AreEqual(searchRequest.PropertyReferenceID, 0);
            Assert.AreEqual(searchRequest.ArrivalDate, DateTime.Now.Date);
            Assert.AreEqual(searchRequest.Duration, 7);
            Assert.AreEqual(searchRequest.MinStarRating, 2);
            Assert.AreEqual(searchRequest.RoomRequests.Count, 1);
            Assert.AreEqual(searchRequest.MealBasisID, 9);

            Assert.AreEqual(searchRequest.RoomRequests[0].GuestConfiguration.Adults, 2);
            Assert.AreEqual(searchRequest.RoomRequests[0].GuestConfiguration.Children, 0);
            Assert.AreEqual(searchRequest.RoomRequests[0].GuestConfiguration.Infants, 0);
            Assert.AreEqual(searchRequest.RoomRequests[0].GuestConfiguration.ChildAges.Count, 0);
        }

        /// <summary>
        /// Create should populate the airport group ID if the arrival type is airport group
        /// </summary>
        [Test]
        public void Create_Should_SetTheAirportGroupID_When_ArrivalTypeIsAirportGroup()
        {
            // Arrange
            ISearchModel searchModel = new SearchModel { ArrivalType = LocationType.AirportGroup, ArrivalID = 690 };
            ISearchRequestAdapter searchRequestAdapter =
                new PropertySearchRequestAdapter(
                    new Mock<GeographyGroupingRepository>(null).Object, 
                    new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModel, HttpContext.Current);

            Assert.AreEqual(searchRequest.AirportID, 0);
            Assert.AreEqual(searchRequest.RegionID, 0);
            Assert.AreEqual(searchRequest.Resorts.Count, 0);
            Assert.AreEqual(searchRequest.AirportGroupID, 690);
            Assert.AreEqual(searchRequest.PropertyReferenceID, 0);
        }

        /// <summary>
        ///     Create should populate the airport ID if the arrival type is airport.
        /// </summary>
        [Test]
        public void Create_Should_SetTheAirportID_When_ArrivalTypeIsAirport()
        {
            // Arrange
            ISearchModel searchModel = new SearchModel { ArrivalType = LocationType.Airport, ArrivalID = 500 };
            ISearchRequestAdapter searchRequestAdapter =
                new PropertySearchRequestAdapter(
                    new Mock<GeographyGroupingRepository>(null).Object, 
                    new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModel, HttpContext.Current);

            Assert.AreEqual(searchRequest.AirportID, 500);
            Assert.AreEqual(searchRequest.RegionID, 0);
            Assert.AreEqual(searchRequest.Resorts.Count, 0);
            Assert.AreEqual(searchRequest.AirportGroupID, 0);
            Assert.AreEqual(searchRequest.PropertyReferenceID, 0);
        }

        /// <summary>
        ///     Create should populate the Property Reference ID if the arrival type is property
        /// </summary>
        [Test]
        public void Create_Should_SetThePropertyReferenceID_When_ArrivalTypeIsProperty()
        {
            // Arrange
            ISearchModel searchModel = new SearchModel { ArrivalType = LocationType.Property, ArrivalID = 123 };
            ISearchRequestAdapter searchRequestAdapter =
                new PropertySearchRequestAdapter(
                    new Mock<GeographyGroupingRepository>(null).Object, 
                    new Mock<IConnectLoginDetailsFactory>().Object);

            // Act
            var searchRequest = (SearchRequest)searchRequestAdapter.Create(searchModel, HttpContext.Current);

            Assert.AreEqual(searchRequest.AirportGroupID, 0);
            Assert.AreEqual(searchRequest.RegionID, 0);
            Assert.AreEqual(searchRequest.Resorts.Count, 0);
            Assert.AreEqual(searchRequest.AirportGroupID, 0);
            Assert.AreEqual(searchRequest.PropertyReferenceID, 123);
        }
    }
}