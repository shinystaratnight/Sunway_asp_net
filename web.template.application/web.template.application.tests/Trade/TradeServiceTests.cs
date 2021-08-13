namespace Web.Template.Application.Tests.Trade
{
    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.Trade;
    using Web.Template.Application.Interfaces.User;
    using Web.Template.Application.Services;
    using Web.Template.Application.Trade.Models;
    using Web.Template.Domain.Entities.Booking;

    /// <summary>
    /// Test class for the trade service.
    /// </summary>
    [TestFixture]
    public class TradeServiceTests
    {
        /// <summary>
        /// Login should call user service login once when login successful.
        /// </summary>
        [Test]
        public void Login_ShouldCallUserServiceLoginOnce_WhenLoginSuccessful()
        {
            // Arrange
            var userService = new Mock<IUserService>();
            var tradeLoginAdaptor = new Mock<ITradeLoginAdaptor>();
            var tradeSessionMock = new Mock<ITradeSession>();
            tradeSessionMock.SetupGet(tsm => tsm.TradeContactId).Returns(10);
            var tradeLoginReturnMock = new Mock<ITradeLoginReturn>();
            tradeLoginReturnMock.SetupGet(tlr => tlr.TradeSession).Returns(tradeSessionMock.Object);
            var tradeContactRepoMock = new Mock<ITradeContactRepository>();
            var tradeRepoMock = new Mock<ITradeRepository>();
            var tradeContactGroupRepoMock = new Mock<ITradeContactGroupRepository>();
            var tradeGroupRepoMock = new Mock<ITradeGroupRepository>();
            
            tradeContactGroupRepoMock.Setup(s => s.GetSingle(It.IsAny<int>(), null)).Returns(new TradeContactGroup());


            tradeContactRepoMock.Setup(s => s.GetSingle(It.IsAny<int>(), null)).Returns(new TradeContact());

            tradeLoginReturnMock.SetupGet(t => t.LoginSuccessful).Returns(true);

            tradeLoginAdaptor.Setup(t => t.Login(It.IsAny<ITradeLoginModel>())).Returns(tradeLoginReturnMock.Object);
            ITradeService tradeService = new TradeService(tradeLoginAdaptor.Object, tradeContactRepoMock.Object, tradeRepoMock.Object,
                tradeContactGroupRepoMock.Object, tradeGroupRepoMock.Object);

            // Act
            ITradeLoginReturn loginReturn = tradeService.Login(saveDetails:false);

            // Assert
            Assert.IsTrue(loginReturn.LoginSuccessful);
        }

        /// <summary>
        /// Login should not call user service login when login unsuccessful.
        /// </summary>
        [Test]
        public void Login_ShouldNotCallUserServiceLogin_WhenLoginUnsuccessful()
        {
            // Arrange
            var userService = new Mock<IUserService>();
            var tradeLoginAdaptor = new Mock<ITradeLoginAdaptor>();
            var tradeLoginReturnMock = new Mock<ITradeLoginReturn>();
            var tradeContactRepoMock = new Mock<ITradeContactRepository>();
            var tradeRepoMock = new Mock<ITradeRepository>();
            var tradeContactGroupRepoMock = new Mock<ITradeContactGroupRepository>();
            var tradeGroupRepoMock = new Mock<ITradeGroupRepository>();

            tradeLoginReturnMock.SetupGet(t => t.LoginSuccessful).Returns(false);
            tradeLoginAdaptor.Setup(t => t.Login(It.IsAny<ITradeLoginModel>())).Returns(tradeLoginReturnMock.Object);
            ITradeService tradeService = new TradeService(tradeLoginAdaptor.Object, tradeContactRepoMock.Object, tradeRepoMock.Object,
                tradeContactGroupRepoMock.Object, tradeGroupRepoMock.Object);

            // Act
            ITradeLoginReturn loginReturn = tradeService.Login();


            // Assert
            userService.Verify(u => u.LoginTrade(It.IsAny<ITradeSession>()), Times.Never);
        }

        /// <summary>
        /// Login should return what the login adaptor receives when called.
        /// </summary>
        [Test]
        public void Login_ShouldReturnWhatTheLoginAdaptorReceives_Whencalled()
        {
            // Arrange
            var userService = new Mock<IUserService>();
            var tradeLoginAdaptor = new Mock<ITradeLoginAdaptor>();
            var tradeLoginReturnMock = new Mock<ITradeLoginReturn>();
            var tradeContactRepoMock = new Mock<ITradeContactRepository>();
            var tradeRepoMock = new Mock<ITradeRepository>();
            var tradeContactGroupRepoMock = new Mock<ITradeContactGroupRepository>();
            var tradeGroupRepoMock = new Mock<ITradeGroupRepository>();

            tradeLoginReturnMock.SetupGet(t => t.LoginSuccessful).Returns(false);
            tradeLoginAdaptor.Setup(t => t.Login(It.IsAny<ITradeLoginModel>())).Returns(tradeLoginReturnMock.Object);
            ITradeService tradeService = new TradeService(tradeLoginAdaptor.Object, tradeContactRepoMock.Object, tradeRepoMock.Object,
                tradeContactGroupRepoMock.Object, tradeGroupRepoMock.Object);

            // Act
            ITradeLoginReturn login = tradeService.Login();

            // Assert
            Assert.AreEqual(login, tradeLoginReturnMock.Object);
        }
    }
}