namespace Web.Template.Application.Lookup.Services
{
    using System.Collections.Generic;

    using Web.Template.Domain.Entities.Booking;

    /// <summary>
    /// Trade lookup service
    /// </summary>
    public interface ITradeLookupService
    {
        /// <summary>
        /// Gets the trade contact identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>a TradeContactGroup</returns>
        TradeContactGroup GetTradeContactGroupByID(int id);

        /// <summary>
        /// Gets the trade contacts.
        /// </summary>
        /// <returns>a list of TradeContactGroup</returns>
        List<TradeContactGroup> GetTradeContactGroups();

        /// <summary>
        /// Gets the trade contact identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>a TradeContact</returns>
        TradeContact GetTradeContactID(int id);

        /// <summary>
        /// Gets the trade contacts.
        /// </summary>
        /// <returns>a list of TradeContact</returns>
        List<TradeContact> GetTradeContacts();

        /// <summary>
        /// Gets the trade group by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>a TradeGroup that matches the id provided</returns>
        TradeGroup GetTradeGroupByID(int id);

        /// <summary>
        /// Gets the trade groups.
        /// </summary>
        /// <returns>a list of TradeGroups</returns>
        List<TradeGroup> GetTradeGroups();

        /// <summary>
        /// Gets the trade parent group by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>a TradeParentGroup that matches the id provided</returns>
        TradeParentGroup GetTradeParentGroupByID(int id);

        /// <summary>
        /// Gets the trade parent groups.
        /// </summary>
        /// <returns>a list of all TradeParentGroup</returns>
        List<TradeParentGroup> GetTradeParentGroups();

        /// <summary>
        /// Gets the trades.
        /// </summary>
        /// <returns>A list of all the Trades</returns>
        List<Trade> GetTrades();
    }
}