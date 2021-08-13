namespace Web.Template.Application.Trade
{
    using Web.Template.Application.Interfaces.Trade;
    using Web.Template.Application.Interfaces.User;
    using Web.Template.Application.User.Models;

    /// <summary>
    /// Class responsible for building up a trade session object
    /// </summary>
    /// <seealso cref="ITradeSessionFactory" />
    public class TradeSessionFactory : ITradeSessionFactory
    {
        /// <summary>
        /// Creates the specified abtaatol.
        /// </summary>
        /// <param name="abtaatol">The abtaatol.</param>
        /// <param name="commisionable">if set to <c>true</c> the agent takes commission</param>
        /// <param name="creditCardAgent">if set to <c>true</c> [credit card agent].</param>
        /// <param name="tradeContactId">The trade contact identifier.</param>
        /// <param name="tradeId">The trade identifier.</param>
        /// <returns>A trade session object</returns>
        public ITradeSession Create(string abtaatol, bool commisionable, bool creditCardAgent, int tradeContactId, int tradeId)
        {
            ITradeSession tradeSession = new TradeSession() { ABTAATOL = abtaatol, Commissionable = commisionable, CreditCardAgent = creditCardAgent, TradeContactId = tradeContactId, TradeId = tradeId };

            return tradeSession;
        }
    }
}