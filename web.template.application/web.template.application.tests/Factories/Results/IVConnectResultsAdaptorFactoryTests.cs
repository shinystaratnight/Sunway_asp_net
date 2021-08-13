namespace Web.Template.Application.Tests.Factories.Results
{
    using System;

    using iVectorConnectInterface.Flight;

    using NUnit.Framework;

    using Web.Template.Application.Interfaces.Results;
    using Web.Template.Application.Results.Factories;
    using Web.Template.IoC;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Test Class for the connect result adaptor factory.
    /// </summary>
    [TestFixture]
    public class IVConnectResultsAdaptorFactoryTests
    {
        /// <summary>
        ///     Create Adaptor By Response Type Should Return Adaptor For Correct Type When Called For Flight
        /// </summary>
        [Test]
        public void CreateAdaptorByResponseType_Should_ReturnAdaptorForCorrectType_When_CalledForFlight()
        {
            //// Arrange
            AutofacRoot.SetupForTests();

            //// Act
            var adaptorFactory = new IVConnectResultsAdaptorFactory();
            IConnectResultsAdaptor requestAdapter = adaptorFactory.CreateAdaptorByResponseType(typeof(SearchResponse));

            ////Assert
            Assert.NotNull(requestAdapter);
            Assert.AreEqual(requestAdapter.ResponseType, typeof(SearchResponse));
        }

        /// <summary>
        ///     Create Adaptor By Response Type Should Return Adaptor For Correct Type When Called For Property
        /// </summary>
        [Test]
        public void CreateAdaptorByResponseType_Should_ReturnAdaptorForCorrectType_When_CalledForProperty()
        {
            //// Arrange
            AutofacRoot.SetupForTests();

            //// Act
            var adaptorFactory = new IVConnectResultsAdaptorFactory();
            IConnectResultsAdaptor requestAdapter = adaptorFactory.CreateAdaptorByResponseType(typeof(iVectorConnectInterface.Property.SearchResponse));

            ////Assert
            Assert.NotNull(requestAdapter);
            Assert.AreEqual(requestAdapter.ResponseType, typeof(iVectorConnectInterface.Property.SearchResponse));
        }

        /// <summary>
        ///     Create Adaptor By Response Type Should Return Different Adaptors For Flight And Hotel When Called
        /// </summary>
        [Test]
        public void CreateAdaptorByResponseType_Should_ReturnDifferentAdaptorsForFlightAndHotel_When_Called()
        {
            //// Arrange
            AutofacRoot.SetupForTests();

            //// Act
            var adaptorFactory = new IVConnectResultsAdaptorFactory();
            IConnectResultsAdaptor flightRequestAdapter = adaptorFactory.CreateAdaptorByResponseType(typeof(SearchResponse));
            IConnectResultsAdaptor propertyRequestAdapter = adaptorFactory.CreateAdaptorByResponseType(typeof(iVectorConnectInterface.Property.SearchResponse));

            //// Assert
            Assert.AreNotEqual(flightRequestAdapter.ResponseType, propertyRequestAdapter.ResponseType);
        }

        /// <summary>
        ///     Create Adaptor By Response Type Should Throw Exception When Type Requested That Does Not Exist
        /// </summary>
        [Test]
        public void CreateAdaptorByResponseType_Should_ThrowException_When_TypeRequestedThatDoesNotExist()
        {
            //// Arrange
            AutofacRoot.SetupForTests();
            var adaptorFactory = new IVConnectResultsAdaptorFactory();

            //// Act
            //// Assert
            Assert.Throws<NotImplementedException>(() => adaptorFactory.CreateAdaptorByResponseType(typeof(int)));
        }
    }
}