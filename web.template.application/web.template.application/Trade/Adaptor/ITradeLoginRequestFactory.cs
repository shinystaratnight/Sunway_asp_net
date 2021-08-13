namespace Web.Template.Application.Trade.Adaptor
{
    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Trade.Models;

    /// <summary>
    /// Defines a class Responsible for building trade login requests.
    /// </summary>
    public interface ITradeLoginRequestFactory
    {
        /// <summary>
        /// Builds the connect request from model.
        /// </summary>
        /// <param name="tradeLoginModel">The trade login model.</param>
        /// <returns>A trade login request.</returns>
        iVectorConnectRequest Create(ITradeLoginModel tradeLoginModel);
    }
}