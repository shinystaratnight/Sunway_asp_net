namespace Web.Template.Application.Tests.Book
{
    using System.Collections.Generic;
    using System.Web;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Book.Adaptors;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Book;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.User;
    using Web.Template.Application.IVectorConnect.Requests;

    /// <summary>
    /// Test class used to test connect trade login adaptor.
    /// </summary>
    [TestFixture]
    public class BasketBookAdaptorTests
    {
        /// <summary>
        /// Book should attempt to create a request for each component in the basket.
        /// </summary>
        /// <param name="numberOfComponents">The number of components.</param>
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Book_Should_AttemptToCreateARequestForEveryComponent_When_Called(int numberOfComponents)
        {
            ////Arrange
            var connectRequestFactoryMock = new Mock<IIVectorConnectRequestFactory>();
            var bookRequestAdaptorFactoryMock = new Mock<IBookRequestAdaptorFactory>();
            var bookReturnBuilderMock = new Mock<IBookReturnBuilder>();
            bookReturnBuilderMock.SetupGet(prb => prb.CurrentlySuccessful).Returns(true);
            var responseProcessMock = new Mock<IBookResponseProcessor>();
            var logindetailsMock = new Mock<IConnectLoginDetailsFactory>();
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(us => us.GetUser(HttpContext.Current)).Returns(new Mock<IUserSession>().Object);
            var basketBookAdaptor = new ConnectBasketBookService(
                connectRequestFactoryMock.Object, 
                logindetailsMock.Object, 
                bookRequestAdaptorFactoryMock.Object, 
                bookReturnBuilderMock.Object, 
                responseProcessMock.Object, 
                userServiceMock.Object);
            var bookRequestAdaptorMock = new Mock<IBookRequestAdaptor>();

            bookRequestAdaptorFactoryMock.Setup(pra => pra.CreateAdaptorByComponentType(It.IsAny<ComponentType>())).Returns(bookRequestAdaptorMock.Object);

            var componentList = new List<IBasketComponent>();

            for (int i = 0; i < numberOfComponents; i++)
            {
                var componentMock = new Mock<IBasketComponent>();
                componentList.Add(componentMock.Object);
            }

            var basketMock = new Mock<IBasket>();
            basketMock.Setup(b => b.Components).Returns(componentList);
            basketMock.SetupGet(b => b.LeadGuest).Returns(new LeadGuestDetails());
            basketMock.SetupGet(b => b.Rooms).Returns(new Mock<List<BasketRoom>>().Object);

            ////Act
            IBookReturn bookReturn = basketBookAdaptor.Book(basketMock.Object);

            ////Assert 
            bookRequestAdaptorFactoryMock.Verify(prf => prf.CreateAdaptorByComponentType(It.IsAny<ComponentType>()), Times.Exactly(numberOfComponents));
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Book_Should_ReturnWarnings_When_ThereAreNoComponentsOnTheBasket()
        {
            ////Arrange
            var connectRequestFactoryMock = new Mock<IIVectorConnectRequestFactory>();
            var bookRequestAdaptorFactoryMock = new Mock<IBookRequestAdaptorFactory>();
            var bookReturnBuilderMock = new Mock<IBookReturnBuilder>();
            var responseProcessMock = new Mock<IBookResponseProcessor>();
            var logindetailsMock = new Mock<IConnectLoginDetailsFactory>();
            var userServiceMock = new Mock<IUserService>();
            var basketBookAdaptor = new ConnectBasketBookService(
                connectRequestFactoryMock.Object, 
                logindetailsMock.Object, 
                bookRequestAdaptorFactoryMock.Object, 
                bookReturnBuilderMock.Object, 
                responseProcessMock.Object, 
                userServiceMock.Object);

            ////Act
            IBookReturn bookReturn = basketBookAdaptor.Book(It.IsAny<IBasket>());

            ////Assert 
            bookReturnBuilderMock.Verify(prb => prb.AddWarnings(It.IsAny<List<string>>()), Times.AtLeastOnce);
        }
    }
}