namespace Web.Template.Application.Tests.Book
{
    using System.Collections.Generic;

    using iVectorConnectInterface.Property;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Book;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Prebook;

    using ivci = iVectorConnectInterface;
    using PreBookResponse = iVectorConnectInterface.Basket.PreBookResponse;

    /// <summary>
    /// Test class used to test connect trade login adaptor.
    /// </summary>
    [TestFixture]
    public class BookResponseProcessorTests
    {
        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Process_Should_SetPaymentsDue_When_PaymentsReturnFromConnect()
        {
            ////Arrange
            var responseProcessor = new BookResponseProcessor();

            var subcomponentMock = new Mock<ISubComponent>();
            subcomponentMock.SetupAllProperties();

            var componentMock = new Mock<IBasketComponent>();
            componentMock.SetupAllProperties().SetupGet(c => c.ComponentType).Returns(ComponentType.Hotel);
            componentMock.SetupGet(c => c.SubComponents).Returns(new List<ISubComponent> { subcomponentMock.Object, subcomponentMock.Object });

            var componentMockB = new Mock<IBasketComponent>();
            componentMockB.SetupAllProperties().SetupGet(c => c.ComponentType).Returns(ComponentType.Flight);

            var basketMock = new Mock<IBasket>();
            basketMock.SetupAllProperties();
            basketMock.Setup(b => b.Components).Returns(new List<IBasketComponent>() { componentMock.Object, componentMockB.Object });

            ////Act
            var response = new ivci.Basket.BookResponse()
                               {
                                   PropertyBookings = new List<BookResponse>() { new BookResponse() { ReturnStatus = new ivci.ReturnStatus() { Success = false } } }, 
                                   FlightBookings = new List<iVectorConnectInterface.Flight.BookResponse>() { new ivci.Flight.BookResponse() { ReturnStatus = new ivci.ReturnStatus() { Success = true } } }
                               };

            responseProcessor.Process(response, basketMock.Object);

            ////Assert 
            Assert.AreEqual(basketMock.Object.ComponentFailedToBook, true);
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Process_Should_SetTheAllComponentsPrebookedFlagToTrue()
        {
            ////Arrange
            var responseProcessor = new BookResponseProcessor();
            var basketMock = new Mock<IBasket>();
            basketMock.SetupAllProperties();

            ////Act
            var response = new ivci.Basket.BookResponse();
            responseProcessor.Process(response, basketMock.Object);

            ////Assert 
            Assert.IsTrue(basketMock.Object.AllComponentsBooked);
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Process_Should_SetUpAdjustments_When_AdjustmentsReturnFromConnect()
        {
            ////Arrange
            var responseProcessor = new PrebookResponseProcessor();

            var componentMock = new Mock<IBasketComponent>();
            componentMock.SetupAllProperties().SetupGet(c => c.ComponentType).Returns(ComponentType.Transfer);

            var basketMock = new Mock<IBasket>();
            basketMock.SetupAllProperties();
            basketMock.Setup(b => b.Components).Returns(new List<IBasketComponent>() { componentMock.Object });

            ////Act
            var response = new PreBookResponse()
                               {
                                   BookingAdjustments =
                                       new List<PreBookResponse.BookingAdjustment>()
                                           {
                                               new PreBookResponse.BookingAdjustment()
                                                   {
                                                       AdjustmentType = "testType", 
                                                       AdjustmentAmount = 123, 
                                                       CalculationBasis = "testBasis", 
                                                       ParentType = "testParentType"
                                                   }, 
                                               new PreBookResponse.BookingAdjustment()
                                                   {
                                                       AdjustmentType = "testTypeb", 
                                                       AdjustmentAmount = 122, 
                                                       CalculationBasis = "testBasisb", 
                                                       ParentType = "testParentTypeb"
                                                   }
                                           }
                               };

            responseProcessor.Process(response, basketMock.Object, null);

            ////Assert 
            Assert.AreEqual(basketMock.Object.Adjustments.Count, 2);

            Assert.AreEqual(basketMock.Object.Adjustments[0].AdjustmentType, "testType");
            Assert.AreEqual(basketMock.Object.Adjustments[0].AdjustmentAmount, 123);
            Assert.AreEqual(basketMock.Object.Adjustments[0].CalculationBasis, "testBasis");
            Assert.AreEqual(basketMock.Object.Adjustments[0].ParentType, "testParentType");

            Assert.AreEqual(basketMock.Object.Adjustments[1].AdjustmentType, "testTypeb");
            Assert.AreEqual(basketMock.Object.Adjustments[1].AdjustmentAmount, 122);
            Assert.AreEqual(basketMock.Object.Adjustments[1].CalculationBasis, "testBasisb");
            Assert.AreEqual(basketMock.Object.Adjustments[1].ParentType, "testParentTypeb");
        }
    }
}