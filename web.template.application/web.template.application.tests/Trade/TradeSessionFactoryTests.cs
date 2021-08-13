namespace Web.Template.Application.Tests.Trade
{
    using NUnit.Framework;

    using Web.Template.Application.Interfaces.User;
    using Web.Template.Application.Trade;

    /// <summary>
    /// Tests for the Trade session Factory
    /// </summary>
    [TestFixture]
    public class TradeSessionFactoryTests
    {
        /// <summary>
        /// Create should set the field.
        /// </summary>
        [Test]
        public void Create_ABTAATOLShouldHaveProvidedValue_WhenCalled()
        {
            ////Arrange
            var tradeSessionFactory = new TradeSessionFactory();

            ////Act
            ITradeSession tradeSession = tradeSessionFactory.Create("abtatest", true, false, 1, 2);

            ////Assert 
            Assert.AreEqual("abtatest", tradeSession.ABTAATOL);
        }

        /// <summary>
        /// Create should set the commission agent flag.
        /// </summary>
        [Test]
        public void Create_CommissionableShouldHaveProvidedValue_WhenCalled()
        {
            ////Arrange
            var tradeSessionFactory = new TradeSessionFactory();

            ////Act
            ITradeSession tradeSession = tradeSessionFactory.Create("abtatest", true, false, 1, 2);

            ////Assert 
            Assert.IsTrue(tradeSession.Commissionable);
        }

        /// <summary>
        /// Create should set the credit card agent flag.
        /// </summary>
        [Test]
        public void Create_CreditCardAgentShouldHaveProvidedValue_WhenCalled()
        {
            ////Arrange
            var tradeSessionFactory = new TradeSessionFactory();

            ////Act
            ITradeSession tradeSession = tradeSessionFactory.Create("abtatest", true, false, 1, 2);

            ////Assert 
            Assert.IsFalse(tradeSession.CreditCardAgent);
        }

        /// <summary>
        /// Create should not return null when called.
        /// </summary>
        [Test]
        public void Create_ShouldNotReturnNull_WhenCalled()
        {
            ////Arrange
            var tradeSessionFactory = new TradeSessionFactory();

            ////Act
            ITradeSession tradeSession = tradeSessionFactory.Create("abtatest", true, false, 1, 2);

            ////Assert 
            Assert.IsNotNull(tradeSession);
        }

        /// <summary>
        /// Create should set Trade Contact Id.
        /// </summary>
        [Test]
        public void Create_TradeContactIdShouldHaveProvidedValue_WhenCalled()
        {
            ////Arrange
            var tradeSessionFactory = new TradeSessionFactory();

            ////Act
            ITradeSession tradeSession = tradeSessionFactory.Create("abtatest", true, false, 1, 2);

            ////Assert 
            Assert.AreEqual(1, tradeSession.TradeContactId);
        }

        /// <summary>
        /// Create should set the Trade Id.
        /// </summary>
        [Test]
        public void Create_TradeIdShouldHaveProvidedValue_WhenCalled()
        {
            ////Arrange
            var tradeSessionFactory = new TradeSessionFactory();

            ////Act
            ITradeSession tradeSession = tradeSessionFactory.Create("abtatest", true, false, 1, 2);

            ////Assert 
            Assert.AreEqual(2, tradeSession.TradeId);
        }
    }
}