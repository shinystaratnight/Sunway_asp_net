namespace Web.Template.Application.Tests.Prebook
{
    using System;

    using NUnit.Framework;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Prebook;
    using Web.Template.Application.Prebook.Factories;
    using Web.Template.IoC;

    /// <summary>
    /// Test for the prebook request adaptor factory
    /// </summary>
    [TestFixture]
    public class PrebookRequestAdaptorFactoryTests
    {
        /// <summary>
        /// Create Adaptor By Response Type Should Return Adaptor For Correct Type When Called For Property
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        [TestCase(ComponentType.Hotel)]
        [TestCase(ComponentType.Flight)]
        [TestCase(ComponentType.Transfer)]
        public void CreateAdaptorByResponseType_Should_ReturnAdaptorForCorrectType_When_Called(ComponentType componentType)
        {
            //// Arrange
            AutofacRoot.SetupForTests();

            //// Act
            var adaptorFactory = new PrebookAdaptorFactory();
            IPrebookRequestAdaptor searchRequestAdaptor = adaptorFactory.CreateAdaptorByComponentType(componentType);

            //// Assert
            Assert.NotNull(searchRequestAdaptor);
            Assert.AreEqual(searchRequestAdaptor.ComponentType, componentType);
        }

        /// <summary>
        /// We dont want random adaptors coming back for component types that are incorrectly set, make sure that it throws
        /// the expected exception here.
        /// </summary>
        public void CreateAdaptorByResponseType_Should_ThrowException_When_CalledForUnknownType()
        {
            //// Arrange
            AutofacRoot.SetupForTests();

            //// Act, Assert
            var adaptorFactory = new PrebookAdaptorFactory();
            Assert.Throws<NotImplementedException>(() => adaptorFactory.CreateAdaptorByComponentType(ComponentType.Unset));
        }
    }
}