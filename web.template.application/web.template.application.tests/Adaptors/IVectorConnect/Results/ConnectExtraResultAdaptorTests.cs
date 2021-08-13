namespace Web.Template.Application.Tests.Adaptors.IVectorConnect.Results
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using AutoMapper;

    using iVectorConnectInterface.Extra;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Application.Results.Adaptors;
    using Web.Template.Application.Results.ResultModels;

    /// <summary>
    /// Class ConnectExtraResultAdaptorTests.
    /// </summary>
    [TestFixture]
    public class ConnectExtraResultAdaptorTests
    {
        /// <summary>
        /// Create should set dates times and duration when set in option.
        /// </summary>
        [Test]
        public void Create_ShouldSetDatesTimesAndDuration_When_SetInOption()
        {
            //// Arrange
            SearchResponse ivcSearchResult = this.BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var extraServiceMock = new Mock<IExtraService>();
            var resultAdaptor = new ConnectExtraResultAdaptor(mapperMock.Object, extraServiceMock.Object);

            var searchModel = new Mock<ISearchModel>();

            //// Act
            List<IResultsModel> result = resultAdaptor.Create(ivcSearchResult, searchModel.Object, HttpContext.Current);
            var extraOption = (ExtraOption)result[0].ResultsCollection[0].SubResults[0];

            //// Assert
            Assert.AreEqual(extraOption.Duration, 7);
            Assert.AreEqual(extraOption.StartDate, DateTime.Now.Date);
            Assert.AreEqual(extraOption.StartTime, "10:00");
            Assert.AreEqual(extraOption.EndDate, DateTime.Now.Date.AddDays(7));
            Assert.AreEqual(extraOption.EndTime, "17:00");
        }

        /// <summary>
        /// Create should set extra information when set in option.
        /// </summary>
        [Test]
        public void Create_ShouldSetExtraInformation_When_SetInOption()
        {
            //// Arrange
            SearchResponse ivcSearchResult = this.BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var extraServiceMock = new Mock<IExtraService>();
            var resultAdaptor = new ConnectExtraResultAdaptor(mapperMock.Object, extraServiceMock.Object);

            var searchModel = new Mock<ISearchModel>();

            extraServiceMock.Setup(x => x.GetExtraTypeById(1)).Returns(new Domain.Entities.Extras.ExtraType() { Name = "Test Type"});

            //// Act
            List<IResultsModel> result = resultAdaptor.Create(ivcSearchResult, searchModel.Object, HttpContext.Current);
            var extraResult = (ExtraResult)result[0].ResultsCollection[0];
            var extraOption = (ExtraOption)extraResult.SubResults[0];

            //// Assert
            Assert.AreEqual(1, extraResult.ExtraId);
            Assert.AreEqual("Test Extra", extraResult.ExtraName);
            Assert.AreEqual("Test Type", extraResult.ExtraType);
            Assert.AreEqual(1, extraOption.ExtraCategoryId);
            Assert.AreEqual("Test Category", extraOption.ExtraCategory);
            Assert.AreEqual("Test Description", extraOption.Description);
        }

        /// <summary>
        /// Create should set minimum maximum ages when set in option.
        /// </summary>
        [Test]
        public void Create_ShouldSetMinMaxAges_When_SetInOption()
        {
            //// Arrange
            SearchResponse ivcSearchResult = this.BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var extraServiceMock = new Mock<IExtraService>();
            var resultAdaptor = new ConnectExtraResultAdaptor(mapperMock.Object, extraServiceMock.Object);

            var searchModel = new Mock<ISearchModel>();

            //// Act
            List<IResultsModel> result = resultAdaptor.Create(ivcSearchResult, searchModel.Object, HttpContext.Current);
            var extraOption = (ExtraOption)result[0].ResultsCollection[0].SubResults[0];

            //// Assert
            Assert.AreEqual(extraOption.MinimumAge, 5);
            Assert.AreEqual(extraOption.MaximumAge, 100);
            Assert.AreEqual(extraOption.MinChildAge, 5);
            Assert.AreEqual(extraOption.MaxChildAge, 17);
            Assert.AreEqual(extraOption.SeniorAge, 60);
        }

        /// <summary>
        /// Create should set occupancy rules when set in option.
        /// </summary>
        [Test]
        public void Create_ShouldSetOccupancyRules_When_SetInOption()
        {
            //// Arrange
            SearchResponse ivcSearchResult = this.BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var extraServiceMock = new Mock<IExtraService>();
            var resultAdaptor = new ConnectExtraResultAdaptor(mapperMock.Object, extraServiceMock.Object);

            var searchModel = new Mock<ISearchModel>();

            //// Act
            List<IResultsModel> result = resultAdaptor.Create(ivcSearchResult, searchModel.Object, HttpContext.Current);
            var extraOption = (ExtraOption)result[0].ResultsCollection[0].SubResults[0];

            //// Assert
            Assert.AreEqual(extraOption.OccupancyRules, true);
            Assert.AreEqual(extraOption.MinPassengers, 1);
            Assert.AreEqual(extraOption.MaxPassengers, 4);
            Assert.AreEqual(extraOption.MinAdults, 1);
            Assert.AreEqual(extraOption.MaxAdults, 3);
            Assert.AreEqual(extraOption.MinChildren, 1);
            Assert.AreEqual(extraOption.MaxChildren, 3);
        }

        /// <summary>
        /// Create should set minimum maximum ages when set in option.
        /// </summary>
        [Test]
        public void Create_ShouldSetPricing_When_SetInOption()
        {
            //// Arrange
            SearchResponse ivcSearchResult = this.BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var extraServiceMock = new Mock<IExtraService>();
            var resultAdaptor = new ConnectExtraResultAdaptor(mapperMock.Object, extraServiceMock.Object);

            var searchModel = new Mock<ISearchModel>();

            //// Act
            List<IResultsModel> result = resultAdaptor.Create(ivcSearchResult, searchModel.Object, HttpContext.Current);
            var extraOption = (ExtraOption)result[0].ResultsCollection[0].SubResults[0];

            //// Assert
            Assert.AreEqual(extraOption.PricingType, "Per Extra");
            Assert.AreEqual(extraOption.ExtraPrice, 100);
            Assert.AreEqual(extraOption.TotalPrice, 100);
            Assert.AreEqual(extraOption.AdultPrice, 100);
            Assert.AreEqual(extraOption.ChildPrice, 100);
            Assert.AreEqual(extraOption.InfantPrice, 100);
            Assert.AreEqual(extraOption.SeniorPrice, 100);
            Assert.AreEqual(extraOption.TotalCommission, 10);
        }

        /// <summary>
        /// Create should set required information when set in option.
        /// </summary>
        [Test]
        public void Create_ShouldSetRequiredInformation_When_SetInOption()
        {
            //// Arrange
            SearchResponse ivcSearchResult = this.BuildConnectResult();
            var mapperMock = new Mock<IMapper>();
            var extraServiceMock = new Mock<IExtraService>();
            var resultAdaptor = new ConnectExtraResultAdaptor(mapperMock.Object, extraServiceMock.Object);

            var searchModel = new Mock<ISearchModel>();

            //// Act
            List<IResultsModel> result = resultAdaptor.Create(ivcSearchResult, searchModel.Object, HttpContext.Current);
            var extraOption = (ExtraOption)result[0].ResultsCollection[0].SubResults[0];

            //// Assert
            Assert.AreEqual(extraOption.AgeRequired, true);
            Assert.AreEqual(extraOption.DateRequired, true);
            Assert.AreEqual(extraOption.TimeRequired, true);
        }
        
        /// <summary>
        /// Builds the connect result.
        /// </summary>
        /// <returns>The SearchResponse.ExtraType.</returns>
        private SearchResponse BuildConnectResult()
        {
            var ivcSearchResult = new SearchResponse
            {
                ExtraTypes = new List<SearchResponse.ExtraType>
                {
                    new SearchResponse.ExtraType
                        {
                            ExtraTypeID = 1,
                            ExtraSubTypes = new List<SearchResponse.ExtraSubType>
                            {
                                new SearchResponse.ExtraSubType
                                    {
                                        ExtraSubTypeID = 1,
                                        Extras = new List<SearchResponse.Extra>
                                            {
                                                new SearchResponse.Extra
                                                    {
                                                        ExtraID = 1,
                                                        ExtraName = "Test Extra",
                                                        ExtraType = "Test Type",
                                                        Options = new List<SearchResponse.Option>
                                                            {
                                                                new SearchResponse.Option
                                                                    {
                                                                        BookingToken = "test booking token",
                                                                        ExtraCategoryID = 1,
                                                                        ExtraCategory = "Test Category",

                                                                        Duration = 7,
                                                                        StartDate = DateTime.Now.Date,
                                                                        StartTime = "10:00",
                                                                        EndDate = DateTime.Now.Date.AddDays(7),
                                                                        EndTime = "17:00",

                                                                        AgeRequired = true,
                                                                        DateRequired = true,
                                                                        TimeRequired = true,

                                                                        OccupancyRules = true,
                                                                        MinPassengers = 1,
                                                                        MaxPassengers = 4,
                                                                        MinAdults = 1,
                                                                        MaxAdults = 3,
                                                                        MinChildren = 1,
                                                                        MaxChildren = 3,

                                                                        MinimumAge = 5,
                                                                        MaximumAge = 100,
                                                                        MinChildAge = 5,
                                                                        MaxChildAge = 17,
                                                                        SeniorAge = 60,

                                                                        PricingType = "Per Extra",
                                                                        ExtraPrice = 100,
                                                                        AdultPrice = 100,
                                                                        ChildPrice = 100,
                                                                        InfantPrice = 100,
                                                                        SeniorPrice = 100,
                                                                        TotalPrice = 100,
                                                                        TotalCommission = 10,

                                                                        Description = "Test Description",
                                                                        MultiBook = true,
                                                                        MaximumQuantity = 5
                                                                    }
                                                            }
                                                    }
                                            }
                                    }
                            }
                    }
                }
            };
            return ivcSearchResult;
        }

    }
}
