namespace Web.Template.Application.Tests.Prebook
{
    using System;
    using System.Collections.Generic;
    using iVectorConnectInterface.Property;
    using Moq;
    using NUnit.Framework;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Prebook;
    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Test class used to test connect trade login adaptor.
    /// </summary>
    [TestFixture]
    public class PrebookResponseProcessorTests
    {
        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Process_Should_SetPaymentsDue_When_PaymentsReturnFromConnect()
        {
            ////Arrange
            var responseProcessor = new PrebookResponseProcessor();

            var subcomponentMock = new Mock<ISubComponent>();
            subcomponentMock.SetupAllProperties();

            var componentMock = new Mock<IBasketComponent>();
            componentMock.SetupAllProperties().SetupGet(c => c.ComponentType).Returns(ComponentType.Hotel);
            componentMock.SetupGet(c => c.SubComponents).Returns(new List<ISubComponent> { subcomponentMock.Object, subcomponentMock.Object });

            var componentMockB = new Mock<Flight>();
            componentMockB.SetupAllProperties().SetupGet(c => c.ComponentType).Returns(ComponentType.Flight);

            var basketMock = new Mock<IBasket>();
            basketMock.SetupAllProperties();
            basketMock.Setup(b => b.Components).Returns(new List<IBasketComponent>() { componentMock.Object, componentMockB.Object });

            ////Act
            var response = new ivci.Basket.PreBookResponse()
            {
                PropertyBookings =
                                       new List<PreBookResponse>()
                                       {
                                           new PreBookResponse()
                                           {
                                               PaymentsDue =
                                                           new List<ivci.Support.PaymentDue>()
                                                           {
                                                               new ivci.Support.PaymentDue()
                                                               {
                                                                   Amount = 105,
                                                                   DateDue = DateTime.Now.AddDays(5)
                                                               },
                                                               new ivci.Support.PaymentDue()
                                                               {
                                                                   Amount = 80,
                                                                   DateDue = DateTime.Now.AddDays(-5)
                                                               }
                                                           }
                                           }
                                       },
                FlightBookings =
                                       new List<iVectorConnectInterface.Flight.PreBookResponse>()
                                       {
                                           new ivci.Flight.PreBookResponse()
                                           {
                                               PaymentsDue =
                                                           new List<ivci.Support.PaymentDue>()
                                                           {
                                                               new ivci.Support.PaymentDue()
                                                               {
                                                                   Amount
                                                                               =
                                                                               210,
                                                                   DateDue
                                                                               =
                                                                               DateTime
                                                                               .Now
                                                                               .AddDays(10)
                                                               },
                                                               new ivci.Support.PaymentDue()
                                                               {
                                                                   Amount
                                                                               =
                                                                               10,
                                                                   DateDue
                                                                               =
                                                                               DateTime
                                                                               .Now
                                                                               .AddDays(-15)
                                                               },
                                                               new ivci.Support.PaymentDue()
                                                               {
                                                                   Amount
                                                                               =
                                                                               20,
                                                                   DateDue
                                                                               =
                                                                               DateTime
                                                                               .Now
                                                                               .AddDays(5)
                                                               }
                                                           }
                                           }
                                       },
                PaymentAmountDetail = new ivci.Support.PaymentAmountDetail
                {
                    DueNowPaymentAmount = 90,
                    TotalPaymentAmount = 425
                }
            };

            responseProcessor.Process(response, basketMock.Object, null);

            ////Assert
            Assert.AreEqual(4, basketMock.Object.Payments.Count);
            Assert.AreEqual(90, basketMock.Object.AmountDueToday);
            Assert.AreEqual(335, basketMock.Object.OutstandingAmount);
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Process_Should_SetTheAllComponentsPrebookedFlagToTrue()
        {
            ////Arrange
            var responseProcessor = new PrebookResponseProcessor();
            var basketMock = new Mock<IBasket>();
            basketMock.SetupAllProperties();

            var componentMock = new Mock<IBasketComponent>();
            componentMock.SetupAllProperties().SetupGet(c => c.ComponentType).Returns(ComponentType.Hotel);
            basketMock.Setup(b => b.Components).Returns(new List<IBasketComponent>() { componentMock.Object });

            ////Act
            var response = new ivci.Basket.PreBookResponse();
            responseProcessor.Process(response, basketMock.Object, null);

            ////Assert
            Assert.IsTrue(basketMock.Object.AllComponentsPreBooked);
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Process_Should_SetTheCorrectFlightComponentValues_When_CalledWithAResponseWithTheValuesSet()
        {
            ////Arrange
            var responseProcessor = new PrebookResponseProcessor();

            var componentMock = new Mock<Flight>();
            componentMock.SetupAllProperties().SetupGet(c => c.ComponentType).Returns(ComponentType.Flight);

            var basketMock = new Mock<IBasket>();
            basketMock.SetupAllProperties();
            basketMock.Setup(b => b.Components).Returns(new List<IBasketComponent>() { componentMock.Object });

            ////Act
            var response = new ivci.Basket.PreBookResponse()
            {
                FlightBookings =
                                       new List<iVectorConnectInterface.Flight.PreBookResponse>()
                                           {
                                               new ivci.Flight.PreBookResponse()
                                                   {
                                                       BookingToken = "FlightPrebookToken",
                                                       TotalPrice = 1245,
                                                       TotalCommission = 10,
                                                       TermsAndConditions = "This is some Terms",
                                                       TermsAndConditionsURL = "This Is a URL"
                                                   }
                                           }
            };

            responseProcessor.Process(response, basketMock.Object, null);

            ////Assert
            Assert.AreEqual(basketMock.Object.Components[0].BookingToken, "FlightPrebookToken");
            Assert.AreEqual(basketMock.Object.Components[0].Price, 1245);
            Assert.AreEqual(basketMock.Object.Components[0].TotalCommission, 10);
            Assert.AreEqual(basketMock.Object.Components[0].TermsAndConditions, "This is some Terms");
            Assert.AreEqual(basketMock.Object.Components[0].TermsAndConditionsUrl, "This Is a URL");
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Process_Should_SetTheCorrectPropertyComponentValues_When_CalledWithAResponseWithTheValuesSet()
        {
            ////Arrange
            var responseProcessor = new PrebookResponseProcessor();

            var subcomponentMock = new Mock<ISubComponent>();
            subcomponentMock.SetupAllProperties();

            var componentMock = new Mock<IBasketComponent>();
            componentMock.SetupAllProperties().SetupGet(c => c.ComponentType).Returns(ComponentType.Hotel);
            componentMock.SetupGet(c => c.SubComponents).Returns(new List<ISubComponent> { subcomponentMock.Object, subcomponentMock.Object });

            var basketMock = new Mock<IBasket>();
            basketMock.SetupAllProperties();
            basketMock.Setup(b => b.Components).Returns(new List<IBasketComponent>() { componentMock.Object });

            ////Act
            var response = new ivci.Basket.PreBookResponse()
            {
                PropertyBookings =
                                       new List<PreBookResponse>()
                                           {
                                               new PreBookResponse()
                                                   {
                                                       BookingToken = "PropertyToken",
                                                       TotalPrice = 4800,
                                                       TotalCommission = 10,
                                                       TermsAndConditions = "This is some Terms",
                                                       TermsAndConditionsURL = "This Is a URL"
                                                   }
                                           }
            };

            responseProcessor.Process(response, basketMock.Object, null);

            ////Assert
            Assert.AreEqual(basketMock.Object.Components[0].BookingToken, "PropertyToken");
            Assert.AreEqual(basketMock.Object.Components[0].TotalPrice, 4800);
            Assert.AreEqual(basketMock.Object.Components[0].SubComponents[0].TotalPrice, 2400);
            Assert.AreEqual(basketMock.Object.Components[0].TermsAndConditions, "This is some Terms");
            Assert.AreEqual(basketMock.Object.Components[0].TermsAndConditionsUrl, "This Is a URL");
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Process_Should_SetTheCorrectTransferComponentValues_When_CalledWithAResponseWithTheValuesSet()
        {
            ////Arrange
            var responseProcessor = new PrebookResponseProcessor();

            var componentMock = new Mock<IBasketComponent>();
            componentMock.SetupAllProperties().SetupGet(c => c.ComponentType).Returns(ComponentType.Transfer);

            var basketMock = new Mock<IBasket>();
            basketMock.SetupAllProperties();
            basketMock.Setup(b => b.Components).Returns(new List<IBasketComponent>() { componentMock.Object });

            ////Act
            var response = new ivci.Basket.PreBookResponse()
            {
                TransferBookings =
                                       new List<iVectorConnectInterface.Transfer.PreBookResponse>()
                                           {
                                               new ivci.Transfer.PreBookResponse()
                                                   {
                                                       BookingToken = "TransferToken",
                                                       TotalPrice = 7777,
                                                       TotalCommission = 96
                                                   }
                                           }
            };

            responseProcessor.Process(response, basketMock.Object, null);

            ////Assert
            Assert.AreEqual(basketMock.Object.Components[0].BookingToken, "TransferToken");
            Assert.AreEqual(basketMock.Object.Components[0].TotalPrice, 7777);
            Assert.AreEqual(basketMock.Object.Components[0].TotalCommission, 96);
        }

        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Process_Should_SetThePrebookTotalPriceOnTheBasket_When_TheResponseHasATotalPrice()
        {
            ////Arrange
            var responseProcessor = new PrebookResponseProcessor();
            var basketMock = new Mock<IBasket>();
            basketMock.SetupAllProperties();

            var componentMock = new Mock<IBasketComponent>();
            componentMock.SetupAllProperties().SetupGet(c => c.ComponentType).Returns(ComponentType.Hotel);
            basketMock.Setup(b => b.Components).Returns(new List<IBasketComponent>() { componentMock.Object });

            ////Act
            var response = new ivci.Basket.PreBookResponse() { TotalPrice = 84569 };
            responseProcessor.Process(response, basketMock.Object, null);

            ////Assert
            Assert.AreEqual(basketMock.Object.PrebookTotalPrice, 84569);
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
            var response = new ivci.Basket.PreBookResponse()
            {
                BookingAdjustments =
                                       new List<iVectorConnectInterface.Basket.PreBookResponse.BookingAdjustment>()
                                           {
                                               new ivci.Basket.PreBookResponse.BookingAdjustment()
                                                   {
                                                       AdjustmentType = "testType",
                                                       AdjustmentAmount = 123,
                                                       CalculationBasis = "testBasis",
                                                       ParentType = "testParentType"
                                                   },
                                               new ivci.Basket.PreBookResponse.BookingAdjustment()
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