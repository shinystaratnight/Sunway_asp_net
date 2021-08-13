namespace Web.Template.Application.Tests.Adaptors.IVectorConnect.Results
{
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Xml;

    using AutoMapper;

    using iVectorConnectInterface.Property;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Configuration;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Results.Adaptors;
    using Web.Template.Application.Results.Factories;
    using Web.Template.Application.Results.ResultModels;
    using Web.Template.Application.Search.SearchModels;

    /// <summary>
    ///     Test class for testing the property results factory
    /// </summary>
    [TestFixture]
    public class ConnectPropertyResultAdaptorTests
    {
        /// <summary>
        ///     Create should return three rooms if connect returns three rooms
        /// </summary>
        [Test]
        public void Create_Should_HaveThreeRooms_When_ConnectReturnsThreeRooms()
        {
            // Arrange
            SearchResponse.PropertyResult ivcSearchResult = BuildConnectResult(3);

            var mapperMock = new Mock<IMapper>();
            var siteServiceMock = new Mock<ISiteService>();
            var site = new Mock<ISite>();
            site.SetupGet(s => s.SiteConfiguration).Returns(new SiteConfiguration()
                                                                {
                                                                    BookingJourneyConfiguration = new BookingJourneyConfiguration()
                                                                                                      {
                                                                                                          OnRequestDisplay = OnRequestDisplay.None
                                                                                                      }
                                                                });
            siteServiceMock.Setup(ss => ss.GetSite(HttpContext.Current)).Returns(site.Object);
            var resultAdaptor = new ConnectPropertyAdaptor(mapperMock.Object, siteServiceMock.Object);

            // Act
            IResult result = resultAdaptor.Create(ivcSearchResult, SearchMode.FlightPlusHotel, HttpContext.Current);
            var propertyResult = (PropertyResult)result;

            // Assert
            Assert.AreEqual(propertyResult.SubResults.Count, 3);
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).BookingToken, "RoomBookingToken");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).TotalPrice, 100.00);
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).RoomType, "Test Room Type");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).RoomView, "Test Room View");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).Sequence, 1);
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).Source, "Test Third Party");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[1]).BookingToken, "RoomBookingToken");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[1]).TotalPrice, 100.00);
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[1]).RoomType, "Test Room Type");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[1]).RoomView, "Test Room View");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[1]).Sequence, 1);
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[1]).Source, "Test Third Party");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[2]).BookingToken, "RoomBookingToken");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[2]).TotalPrice, 100.00);
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[2]).RoomType, "Test Room Type");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[2]).RoomView, "Test Room View");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[2]).Sequence, 1);
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[2]).Source, "Test Third Party");
        }

        /// <summary>
        ///     Create should be a result with two rooms if connect returns two rooms
        /// </summary>
        [Test]
        public void Create_Should_HaveTwoRooms_When_ConnectReturnsTwoRoom()
        {
            // Arrange
            SearchResponse.PropertyResult ivcSearchResult = BuildConnectResult(2);

            var mapperMock = new Mock<IMapper>();
            var siteServiceMock = new Mock<ISiteService>();
            var site = new Mock<ISite>();
            site.SetupGet(s => s.SiteConfiguration).Returns(new SiteConfiguration()
            {
                BookingJourneyConfiguration = new BookingJourneyConfiguration()
                {
                    OnRequestDisplay = OnRequestDisplay.None
                }
            });
            siteServiceMock.Setup(ss => ss.GetSite(HttpContext.Current)).Returns(site.Object);
            var resultAdaptor = new ConnectPropertyAdaptor(mapperMock.Object, siteServiceMock.Object);

            // Act
            IResult result = resultAdaptor.Create(ivcSearchResult, SearchMode.FlightPlusHotel, HttpContext.Current);
            var propertyResult = (PropertyResult)result;

            // Assert
            Assert.AreEqual(propertyResult.SubResults.Count, 2);
            Assert.AreEqual(propertyResult.SubResults[0].BookingToken, "RoomBookingToken");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).TotalPrice, 100.00);
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).RoomType, "Test Room Type");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).RoomView, "Test Room View");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).Sequence, 1);
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).Source, "Test Third Party");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[1]).BookingToken, "RoomBookingToken");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[1]).TotalPrice, 100.00);
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[1]).RoomType, "Test Room Type");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[1]).RoomView, "Test Room View");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[1]).Sequence, 1);
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[1]).Source, "Test Third Party");
        }

        /// <summary>
        ///     Create should populate values when provided values
        /// </summary>
        [Test]
        public void Create_Should_PopulateValues_WhenProvidedObjectWithValuesSet()
        {
            // Arrange
            SearchResponse.PropertyResult ivcSearchResult = BuildConnectResult();

            var mapperMock = new Mock<IMapper>();
            var siteServiceMock = new Mock<ISiteService>();
            var site = new Mock<ISite>();
            site.SetupGet(s => s.SiteConfiguration).Returns(new SiteConfiguration()
            {
                BookingJourneyConfiguration = new BookingJourneyConfiguration()
                {
                    OnRequestDisplay = OnRequestDisplay.None
                }
            });
            siteServiceMock.Setup(ss => ss.GetSite(HttpContext.Current)).Returns(site.Object);

            var resultAdaptor = new ConnectPropertyAdaptor(mapperMock.Object, siteServiceMock.Object);

            // Act
            IResult result = resultAdaptor.Create(ivcSearchResult, SearchMode.FlightPlusHotel, HttpContext.Current);
            var propertyResult = (PropertyResult)result;

            // Assert
            Assert.AreEqual(propertyResult.BookingToken, "test Booking Token");
            Assert.AreEqual(propertyResult.GeographyLevel1Id, 5);
            Assert.AreEqual(propertyResult.GeographyLevel2Id, 2);
            Assert.AreEqual(propertyResult.GeographyLevel3Id, 4);
            Assert.AreEqual(propertyResult.Name, "Test Hotel");
            Assert.AreEqual(propertyResult.Rating, "4.0");
            Assert.AreEqual(propertyResult.SubResults.Count, 1);
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).BookingToken, "RoomBookingToken");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).TotalPrice, 100.00);
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).RoomType, "Test Room Type");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).OnRequest, false);
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).RoomView, "Test Room View");
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).Sequence, 1);
            Assert.AreEqual(((RoomOption)propertyResult.SubResults[0]).Source, "Test Third Party");
        }

        /// <summary>
        ///     Create should populate values when provided values
        /// </summary>
        [Test]
        public void Create_Should_ReturnCollection_When_CollectionPassedIn()
        {
            // Arrange
            var ivcSearchResults = new SearchResponse { PropertyResults = new List<SearchResponse.PropertyResult> { BuildConnectResult(), BuildConnectResult() } };

            var componentAdaptorFactoryMock = new Mock<IIvConnectResultComponentAdaptorFactory>();

            var mapperMock = new Mock<IMapper>();
            var siteServiceMock = new Mock<ISiteService>();
            var site = new Mock<ISite>();
            site.SetupGet(s => s.SiteConfiguration).Returns(new SiteConfiguration()
            {
                BookingJourneyConfiguration = new BookingJourneyConfiguration()
                {
                    OnRequestDisplay = OnRequestDisplay.None
                }
            });
            siteServiceMock.Setup(ss => ss.GetSite(HttpContext.Current)).Returns(site.Object);

            componentAdaptorFactoryMock.Setup(car => car.CreateAdaptorByComponentType(typeof(SearchResponse.PropertyResult))).Returns(new ConnectPropertyAdaptor(mapperMock.Object, siteServiceMock.Object));

            var resultAdaptor = new ConnectPropertyResultAdaptor(componentAdaptorFactoryMock.Object);

            var searchModel = new Mock<ISearchModel>();
            searchModel.SetupGet(sm => sm.SearchMode).Returns(SearchMode.FlightPlusHotel);
            searchModel.SetupGet(sm => sm.Rooms).Returns(new List<Room>() { new Room() { Adults = 2 } });

            // Act
            List<IResultsModel> results = resultAdaptor.Create(ivcSearchResults, searchModel.Object, HttpContext.Current);
            Assert.AreEqual(results[0].ResultsCollection.Count, 2);
        }

        /// <summary>
        ///     When evaluated the response type the connect property results adaptor is mapped to should be property
        /// </summary>
        [Test]
        public void ResponseType_Should_BeProperty_When_Evaluated()
        {

            var componentAdaptorFactoryMock = new Mock<IIvConnectResultComponentAdaptorFactory>();

            var mapperMock = new Mock<IMapper>();

            var resultAdaptor = new ConnectPropertyResultAdaptor(componentAdaptorFactoryMock.Object);

            Assert.AreEqual(resultAdaptor.ResponseType, typeof(SearchResponse));
        }

        /// <summary>
        ///     Builds the connect result.
        /// </summary>
        /// <param name="numberOfRooms">The number of rooms.</param>
        /// <returns>A property search response used for faking</returns>
        private static SearchResponse.PropertyResult BuildConnectResult(int numberOfRooms = 1)
        {
            var ivcSearchResult = new SearchResponse.PropertyResult
                                      {
                                          BookingToken = "test Booking Token", 
                                          ContractID = 5, 
                                          GeographyLevel1ID = 5, 
                                          GeographyLevel2ID = 2, 
                                          GeographyLevel3ID = 4, 
                                          PropertyReferenceID = 9, 
                                          Rental = false, 
                                          RoomTypes = new List<SearchResponse.RoomType> { BuildRoomType() }, 
                                          Source = "Test Source", 
                                          SearchResponseXML =
                                              new XmlDocument
                                                  {
                                                      InnerXml =
                                                          "<Property><PropertyName>Test Hotel</PropertyName><Rating>4.0</Rating><FacilityFlag>8</FacilityFlag><Longitude>13.3404900000</Longitude><Latitude>38.1383400000</Latitude><ReviewScores><ReviewScore><CMSWebsiteID>1</CMSWebsiteID><ReviewNumberOfReviews>23</ReviewNumberOfReviews><ReviewAverageScore>7.000000</ReviewAverageScore></ReviewScore><ReviewScore><CMSWebsiteID>3</CMSWebsiteID><ReviewNumberOfReviews>34</ReviewNumberOfReviews><ReviewAverageScore>8.000000</ReviewAverageScore></ReviewScore></ReviewScores><MainImage>http://imperatoretestadmin.ivector.co.uk/Content/DataObjects/PropertyReference/Image/image_95_v3.jpg</MainImage><MainImageTitle/><Summary>**Andr test** una delle vittime dei due attacchi a Copenaghen è Dan Uzan, di 37 anni, di padre israeliano e madre danese, da anni guardiano della sinagoga dove presumibilmente ha tentato di entrare l'attentatore, poi ucciso in un'altra zona dalla polizia danese.</Summary><URL/><BestSeller>0</BestSeller><ProductAttributes><ProductAttribute><ProductAttribute>regional</ProductAttribute><ProductAttributeID>1</ProductAttributeID></ProductAttribute><ProductAttribute><ProductAttribute>luxury</ProductAttribute><ProductAttributeID>2</ProductAttributeID></ProductAttribute><ProductAttribute><ProductAttribute>charming</ProductAttribute><ProductAttributeID>3</ProductAttributeID></ProductAttribute></ProductAttributes><CustomXML><Facilities><Facility><FacilityID>8</FacilityID><Facility>test2</Facility><FacilityType>Hotel</FacilityType></Facility><Facility><FacilityID>9</FacilityID><Facility>test2</Facility><FacilityType>Hotel</FacilityType></Facility><Facility><FacilityID>10</FacilityID><Facility>test2</Facility><FacilityType>Hotel</FacilityType><Icon>http://imperatoretestadmin.ivector.co.uk/Content/DataObjects/Facility/Image/image_40_v1.png</Icon><Icon_ImageTitle/><Icon_AdditionalInfo/></Facility><Facility><FacilityID>11</FacilityID><Facility>test2</Facility><FacilityType>Hotel</FacilityType></Facility><Facility><FacilityID>12</FacilityID><Facility>test2</Facility><FacilityType>Hotel</FacilityType></Facility></Facilities></CustomXML></Property>"
                                                  }
                                      };

            if (numberOfRooms > 1)
            {
                ivcSearchResult.RoomTypes.Add(BuildRoomType());
            }

            if (numberOfRooms > 2)
            {
                ivcSearchResult.RoomTypes.Add(BuildRoomType());
            }

            return ivcSearchResult;
        }

        /// <summary>
        ///     Builds the type of the room.
        /// </summary>
        /// <returns>A fake room type</returns>
        private static SearchResponse.RoomType BuildRoomType()
        {
            var room = new SearchResponse.RoomType { RoomBookingToken = "RoomBookingToken", Total = (decimal)100.00, RoomType = "Test Room Type", Seq = 1, RoomView = "Test Room View", OnRequest = false, Source = "Test Third Party" };
            return room;
        }
    }
}