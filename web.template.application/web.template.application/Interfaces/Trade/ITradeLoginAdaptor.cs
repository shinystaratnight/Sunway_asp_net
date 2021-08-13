namespace Web.Template.Application.Interfaces.Trade
{
    using Web.Template.Application.Trade.Models;

    /// <summary>
    /// interface for a class responsible for talking to the service that handles the login and returns a session object.
    /// </summary>
    public interface ITradeLoginAdaptor
    {
        /// <summary>
        /// Builds the trade login request.
        /// </summary>
        /// <param name="tradeLoginModel">The trade login model.</param>
        /// <returns>A trade session.</returns>
        ITradeLoginReturn Login(ITradeLoginModel tradeLoginModel);
    }
}