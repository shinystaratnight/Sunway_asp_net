namespace Web.Template.Application.Tests.Factories.Search
{
    using System;

    using iVectorConnectInterface.Flight;

    using NUnit.Framework;

    using Web.Template.Application.Interfaces.Search;
    using Web.Template.Application.Search.Factories;
    using Web.Template.IoC;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Tests for the Request Adaptor Factory.
    /// </summary>
    [TestFixture]
    public class RequestAdaptorFactoryTests
    {
        /// <summary>
        /// Create Adaptor By Response Type Should Return Adaptor For Correct Type When Called For Flight
        /// </summary>
        [Test]
        public void CreateAdaptorByResponseType_Should_ReturnAdaptorForCorrectType_When_CalledForFlight()
        {
            //// Arrange
            AutofacRoot.SetupForTests();

            //// Act
            var adaptorFactory = new SearchRequestAdaptorFactory();
            ISearchRequestAdapter searchRequestAdapter = adaptorFactory.CreateAdaptorByResponseType(typeof(SearchResponse));

            //// Assert
            Assert.NotNull(searchRequestAdapter);
            Assert.AreEqual(searchRequestAdapter.ResponseType, typeof(SearchResponse));
        }

        /// <summary>
        /// Create Adaptor By Response Type Should Return Adaptor For Correct Type When Called for Property
        /// </summary>
        [Test]
        public void CreateAdaptorByResponseType_Should_ReturnAdaptorForCorrectType_When_CalledForProperty()
        {
            //// Arrange
            AutofacRoot.SetupForTests();

            //// Act
            var adaptorFactory = new SearchRequestAdaptorFactory();
            ISearchRequestAdapter searchRequestAdapter = adaptorFactory.CreateAdaptorByResponseType(typeof(iVectorConnectInterface.Property.SearchResponse));

            //// Assert
            Assert.NotNull(searchRequestAdapter);
            Assert.AreEqual(searchRequestAdapter.ResponseType, typeof(iVectorConnectInterface.Property.SearchResponse));
        }

        /// <summary>
        /// Create Adaptor By Response Type Should Return Different Adaptors For Flight And Hotel When Called
        /// </summary>
        [Test]
        public void CreateAdaptorByResponseType_Should_ReturnDifferentAdaptorsForFlightAndHotel_When_Called()
        {
            //// Arrange
            AutofacRoot.SetupForTests();

            //// Act
            var adaptorFactory = new SearchRequestAdaptorFactory();
            ISearchRequestAdapter flightSearchRequestAdapter = adaptorFactory.CreateAdaptorByResponseType(typeof(SearchResponse));
            ISearchRequestAdapter propertySearchRequestAdapter = adaptorFactory.CreateAdaptorByResponseType(typeof(iVectorConnectInterface.Property.SearchResponse));

            //// Assert
            Assert.AreNotEqual(flightSearchRequestAdapter.ResponseType, propertySearchRequestAdapter.ResponseType);
        }

        /// <summary>
        /// Create Adaptor By Response Type Should Throw Exception When Type Requested That Does Not Exist
        /// </summary>
        [Test]
        public void CreateAdaptorByResponseType_Should_ThrowException_When_TypeRequestedThatDoesNotExist()
        {
            //// Arrange
            AutofacRoot.SetupForTests();
            var adaptorFactory = new SearchRequestAdaptorFactory();

            //// Act
            //// Assert
            Assert.Throws<NotImplementedException>(() => adaptorFactory.CreateAdaptorByResponseType(typeof(int)));
        }
    }
}