namespace Web.Template.Application.Interfaces.Trade
{
    using Web.Template.Application.Interfaces.User;

    /// <summary>
    /// Defines a class responsible for building up a trade session object
    /// </summary>
    public interface ITradeSessionFactory
    {
        /// <summary>
        /// Creates the specified abtaatol.
        /// </summary>
        /// <param name="abtaatol">The abtaatol.</param>
        /// <param name="commisionable">if set to <c>true</c> the agent takes commission.</param>
        /// <param name="creditCardAgent">if set to <c>true</c> [credit card agent].</param>
        /// <param name="tradeContactId">The trade contact identifier.</param>
        /// <param name="tradeId">The trade identifier.</param>
        /// <returns>
        /// a trade session object
        /// </returns>
        ITradeSession Create(string abtaatol, bool commisionable, bool creditCardAgent, int tradeContactId, int tradeId);
    }
}