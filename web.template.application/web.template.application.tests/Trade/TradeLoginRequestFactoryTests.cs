namespace Web.Template.Application.Tests.Trade
{
    using System.Web;

    using iVectorConnectInterface;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Trade.Adaptor;
    using Web.Template.Application.Trade.Models;

    /// <summary>
    /// Test class for trade login request factory
    /// </summary>
    [TestFixture]
    public class TradeLoginRequestFactoryTests
    {
        /// <summary>
        /// Builds the connect request from model set connect login details password when provided from configuration.
        /// </summary>
        [Test]
        public void BuildConnectRequestFromModel_SetIvectorConnectLoginDetailsPassword_WhenProvidedFromConfig()
        {
            ////Arrange
            var loginModel = new Mock<ITradeLoginModel>();

            var site = new Mock<ISite>();
            site.SetupGet(c => c.IvectorConnectPassword).Returns("ivcpw1");

            var siteService = new Mock<ISiteService>();
            siteService.Setup(ss => ss.GetSite(HttpContext.Current)).Returns(site.Object);

            ITradeLoginRequestFactory tradeLoginRequestFactory = new TradeLoginRequestFactory(siteService.Object);

            ////Act
            TradeLoginRequest connectRequest = (TradeLoginRequest)tradeLoginRequestFactory.Create(loginModel.Object);

            ////Assert 
            Assert.AreEqual(connectRequest.LoginDetails.Password, "ivcpw1");
        }

        /// <summary>
        /// Builds the connect request from model and set connect login details username when provided from configuration.
        /// </summary>
        [Test]
        public void BuildConnectRequestFromModel_SetIvectorConnectLoginDetailsUsername_WhenProvidedFromConfig()
        {
            ////Arrange
            var loginModel = new Mock<ITradeLoginModel>();

            var site = new Mock<ISite>();
            site.SetupGet(c => c.IvectorConnectUsername).Returns("ivcuser2");

            var siteService = new Mock<ISiteService>();
            siteService.Setup(ss => ss.GetSite(HttpContext.Current)).Returns(site.Object);

            ITradeLoginRequestFactory tradeLoginRequestFactory = new TradeLoginRequestFactory(siteService.Object);

            ////Act
            TradeLoginRequest connectRequest = (TradeLoginRequest)tradeLoginRequestFactory.Create(loginModel.Object);

            ////Assert 
            Assert.AreEqual(connectRequest.LoginDetails.Login, "ivcuser2");
        }

        /// <summary>
        /// Builds the connect request from model should set email when it is set.
        /// </summary>
        [Test]
        public void BuildConnectRequestFromModel_ShouldSetEmail_WhenItIsSet()
        {
            ////Arrange
            var site = new Mock<ISite>().Object;

            var siteService = new Mock<ISiteService>();
            siteService.Setup(ss => ss.GetSite(HttpContext.Current)).Returns(site);

            ITradeLoginRequestFactory tradeLoginRequestFactory = new TradeLoginRequestFactory(siteService.Object);

            var loginModel = new Mock<ITradeLoginModel>();
            loginModel.SetupGet(lm => lm.EmailAddress).Returns("testEmailAddress");

            ////Act
            TradeLoginRequest connectRequest = (TradeLoginRequest)tradeLoginRequestFactory.Create(loginModel.Object);

            ////Assert 
            Assert.AreEqual(connectRequest.Email, "testEmailAddress");
        }

        /// <summary>
        /// Builds the connect request from model should set password when it is set.
        /// </summary>
        [Test]
        public void BuildConnectRequestFromModel_ShouldSetPassword_WhenItIsSet()
        {
            ////Arrange
            var site = new Mock<ISite>().Object;
            var siteService = new Mock<ISiteService>();
            siteService.Setup(ss => ss.GetSite(HttpContext.Current)).Returns(site);

            ITradeLoginRequestFactory tradeLoginRequestFactory = new TradeLoginRequestFactory(siteService.Object);

            var loginModel = new Mock<ITradeLoginModel>();
            loginModel.SetupGet(lm => lm.Password).Returns("testPassword");

            ////Act
            TradeLoginRequest connectRequest = (TradeLoginRequest)tradeLoginRequestFactory.Create(loginModel.Object);

            ////Assert 
            Assert.AreEqual(connectRequest.Password, "testPassword");
        }

        /// <summary>
        /// Builds the connect request from model_ should set username_ when it is set.
        /// </summary>
        [Test]
        public void BuildConnectRequestFromModel_ShouldSetUsername_WhenItIsSet()
        {
            ////Arrange
            var site = new Mock<ISite>().Object;
            var siteService = new Mock<ISiteService>();
            siteService.Setup(ss => ss.GetSite(HttpContext.Current)).Returns(site);

            ITradeLoginRequestFactory tradeLoginRequestFactory = new TradeLoginRequestFactory(siteService.Object);

            var loginModel = new Mock<ITradeLoginModel>();
            loginModel.SetupGet(lm => lm.UserName).Returns("testUserName");

            ////Act
            TradeLoginRequest connectRequest = (TradeLoginRequest)tradeLoginRequestFactory.Create(loginModel.Object);

            ////Assert 
            Assert.AreEqual(connectRequest.UserName, "testUserName");
        }

        /// <summary>
        /// Builds the connect request from model_ should set website password_ when it is set.
        /// </summary>
        [Test]
        public void BuildConnectRequestFromModel_ShouldSetWebsitePassword_WhenItIsSet()
        {
            ////Arrange
            var site = new Mock<ISite>().Object;
            var siteService = new Mock<ISiteService>();
            siteService.Setup(ss => ss.GetSite(HttpContext.Current)).Returns(site);

            ITradeLoginRequestFactory tradeLoginRequestFactory = new TradeLoginRequestFactory(siteService.Object);

            var loginModel = new Mock<ITradeLoginModel>();
            loginModel.SetupGet(lm => lm.WebsitePassword).Returns("testwp");

            ////Act
            TradeLoginRequest connectRequest = (TradeLoginRequest)tradeLoginRequestFactory.Create(loginModel.Object);

            ////Assert 
            Assert.AreEqual(connectRequest.WebsitePassword, "testwp");
        }
    }
}