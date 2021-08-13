namespace Web.Template.Application.Tests.Prebook
{
    using System;

    using NUnit.Framework;

    using Web.Template.Application.Book.Factories;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Book;
    using Web.Template.IoC;

    /// <summary>
    /// Test for the prebook request adaptor factory
    /// </summary>
    [TestFixture]
    public class BookRequestAdaptorFactoryTests
    {
        /// <summary>
        /// Create Adaptor By Response Type Should Return Adaptor For Correct Type When Called For Property
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        [TestCase(ComponentType.Hotel)]
        [TestCase(ComponentType.Flight)]
        [TestCase(ComponentType.Transfer)]
        [TestCase(ComponentType.Extra)]
        public void CreateAdaptorByResponseType_Should_ReturnAdaptorForCorrectType_When_Called(ComponentType componentType)
        {
            //// Arrange
            AutofacRoot.SetupForTests();

            //// Act
            var adaptorFactory = new BookAdaptorFactory();
            IBookRequestAdaptor searchRequestAdaptor = adaptorFactory.CreateAdaptorByComponentType(componentType);

            //// Assert
            Assert.NotNull(searchRequestAdaptor);
            Assert.AreEqual(searchRequestAdaptor.ComponentType, componentType);
        }

        /// <summary>
        /// We do not want random adaptors coming back for component types that are incorrectly set, make sure that it throws
        /// the expected exception here.
        /// </summary>
        public void CreateAdaptorByResponseType_Should_ThrowException_When_CalledForUnknownType()
        {
            //// Arrange
            AutofacRoot.SetupForTests();

            //// Act, Assert
            var adaptorFactory = new BookAdaptorFactory();
            Assert.Throws<NotImplementedException>(() => adaptorFactory.CreateAdaptorByComponentType(ComponentType.Unset));
        }
    }
}