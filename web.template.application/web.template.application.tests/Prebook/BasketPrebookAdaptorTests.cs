namespace Web.Template.Application.Tests.Prebook
{
    using System.Collections.Generic;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Prebook;
    using Web.Template.Application.IVectorConnect.Requests;
    using Web.Template.Application.Prebook.Adaptor;

    /// <summary>
    /// Test class used to test connect trade login adaptor.
    /// </summary>
    [TestFixture]
    public class BasketPrebookAdaptorTests
    {
        /// <summary>
        /// Prebook should attempt to create a request for each component in the basket.
        /// </summary>
        /// <param name="numberOfComponents">The number of components.</param>
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Prebook_Should_AttemptToCreateARequestForEveryComponent_When_Called(int numberOfComponents)
        {
            ////Arrange
            var connectRequestFactoryMock = new Mock<IIVectorConnectRequestFactory>();
            var prebookRequestAdaptorFactoryMock = new Mock<IPrebookRequestAdaptorFactory>();
            var prebookReturnBuilderMock = new Mock<IPrebookReturnBuilder>();
            prebookReturnBuilderMock.SetupGet(prb => prb.CurrentlySuccessful).Returns(true);
            var responseProcessMock = new Mock<IPrebookResponseProcessor>();
            var logindetailsMock = new Mock<IConnectLoginDetailsFactory>();
            var basketPrebookAdaptor = new ConnectBasketPrebookService(connectRequestFactoryMock.Object, prebookRequestAdaptorFactoryMock.Object, prebookReturnBuilderMock.Object, responseProcessMock.Object, logindetailsMock.Object);
            var prebookRequestAdaptorMock = new Mock<IPrebookRequestAdaptor>();

            prebookRequestAdaptorFactoryMock.Setup(pra => pra.CreateAdaptorByComponentType(It.IsAny<ComponentType>())).Returns(prebookRequestAdaptorMock.Object);

            var componentList = new List<IBasketComponent>();

            for (int i = 0; i < numberOfComponents; i++)
            {
                var componentMock = new Mock<IBasketComponent>();
                componentList.Add(componentMock.Object);
            }

            var basketMock = new Mock<IBasket>();
            basketMock.Setup(b => b.Components).Returns(componentList);

            ////Act
            IPrebookReturn prebookReturn = basketPrebookAdaptor.Prebook(basketMock.Object);

            ////Assert 
            prebookRequestAdaptorFactoryMock.Verify(prf => prf.CreateAdaptorByComponentType(It.IsAny<ComponentType>()), Times.Exactly(numberOfComponents));
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Prebook_Should_ReturnWarnings_When_ThereAreNoComponentsOnTheBasket()
        {
            ////Arrange
            var connectRequestFactoryMock = new Mock<IIVectorConnectRequestFactory>();
            var prebookRequestAdaptorFactoryMock = new Mock<IPrebookRequestAdaptorFactory>();
            var prebookReturnBuilderMock = new Mock<IPrebookReturnBuilder>();
            var responseProcessMock = new Mock<IPrebookResponseProcessor>();
            var logindetailsMock = new Mock<IConnectLoginDetailsFactory>();
            var basketPrebookAdaptor = new ConnectBasketPrebookService(connectRequestFactoryMock.Object, prebookRequestAdaptorFactoryMock.Object, prebookReturnBuilderMock.Object, responseProcessMock.Object, logindetailsMock.Object);

            ////Act
            IPrebookReturn prebookReturn = basketPrebookAdaptor.Prebook(It.IsAny<IBasket>());

            ////Assert 
            prebookReturnBuilderMock.Verify(prb => prb.AddWarnings(It.IsAny<List<string>>()), Times.AtLeastOnce);
        }
    }
}