namespace Web.Template.Application.Tests.Trade
{
    using System.Collections.Generic;

    using iVectorConnectInterface.Interfaces;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Trade;
    using Web.Template.Application.Trade.Adaptor;
    using Web.Template.Application.Trade.Models;

    /// <summary>
    /// Test class used to test connect trade login adaptor.
    /// </summary>
    [TestFixture]
    public class ConnectTradeLoginAdaptorTests
    {
        /// <summary>
        /// Login should return success false when warnings raised.
        /// </summary>
        [Test]
        public void Login_ShouldReturnSuccessFalse_WhenWarningsRaised()
        {
            ////Arrange
            var loginModel = new Mock<ITradeLoginModel>().Object;

            var mockConnectRequest = new Mock<iVectorConnectRequest>();
            mockConnectRequest.Setup(r => r.Validate(eValidationType.None)).Returns(new List<string>() { "warning1", "warning2" });

            var vectorConnectRequestFactory = new Mock<ITradeLoginRequestFactory>();
            vectorConnectRequestFactory.Setup(f => f.Create(It.IsAny<ITradeLoginModel>())).Returns(mockConnectRequest.Object);

            var connectTradeLoginAdaptor = new ConnectTradeLoginAdaptor(new Mock<IIVectorConnectRequestFactory>().Object, new Mock<ITradeSessionFactory>().Object, vectorConnectRequestFactory.Object);

            ////Act
            ITradeLoginReturn tradeLoginReturn = connectTradeLoginAdaptor.Login(loginModel);

            ////Assert
            Assert.IsFalse(tradeLoginReturn.LoginSuccessful);
        }
    }
}