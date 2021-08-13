namespace Web.Template.Application.Lookup.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Web.Template.Domain.Entities.Booking;

    /// <summary>
    /// trade lookup service responsible for retrieving information concerning trades
    /// </summary>
    /// <seealso cref="Web.Template.Application.Lookup.Services.ITradeLookupService" />
    public class TradeLookupService : ITradeLookupService
    {
        /// <summary>
        /// The trade contact group
        /// </summary>
        private readonly ITradeContactGroupRepository tradeContactGroupRepository;

        /// <summary>
        /// The trade contact repository
        /// </summary>
        private readonly ITradeContactRepository tradeContactRepository;

        /// <summary>
        /// The trade group repository
        /// </summary>
        private readonly ITradeGroupRepository tradeGroupRepository;

        /// <summary>
        /// The trade parent group repository
        /// </summary>
        private readonly ITradeParentGroupRepository tradeParentGroupRepository;

        /// <summary>
        /// The trade repository
        /// </summary>
        private readonly ITradeRepository tradeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeLookupService" /> class.
        /// </summary>
        /// <param name="tradeGroupRepository">The trade group repository.</param>
        /// <param name="tradeContactRepository">The trade contact repository.</param>
        /// <param name="tradeParentGroupRepository">The trade parent group repository.</param>
        /// <param name="tradeContactGroupRepository">The trade contact group repository.</param>
        /// <param name="tradeRepository">The trade repository.</param>
        public TradeLookupService(ITradeGroupRepository tradeGroupRepository, ITradeContactRepository tradeContactRepository, ITradeParentGroupRepository tradeParentGroupRepository, ITradeContactGroupRepository tradeContactGroupRepository, ITradeRepository tradeRepository)
        {
            this.tradeGroupRepository = tradeGroupRepository;
            this.tradeContactRepository = tradeContactRepository;
            this.tradeParentGroupRepository = tradeParentGroupRepository;
            this.tradeContactGroupRepository = tradeContactGroupRepository;
            this.tradeRepository = tradeRepository;
        }

        /// <summary>
        /// Gets the trade contact identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>the trade contact group that matches the given id</returns>
        public TradeContactGroup GetTradeContactGroupByID(int id)
        {
            return this.tradeContactGroupRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the trade contacts.
        /// </summary>
        /// <returns>a list of all trade contact groups</returns>
        public List<TradeContactGroup> GetTradeContactGroups()
        {
            return this.tradeContactGroupRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets the trade contact identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>the trade contact that matches the given id</returns>
        public TradeContact GetTradeContactID(int id)
        {
            return this.tradeContactRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the trade contacts.
        /// </summary>
        /// <returns>a list of trade contacts</returns>
        public List<TradeContact> GetTradeContacts()
        {
            return this.tradeContactRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets the trade group by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>a list of trade groups that match the id</returns>
        public TradeGroup GetTradeGroupByID(int id)
        {
            return this.tradeGroupRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the trade groups.
        /// </summary>
        /// <returns>a list of trade groups</returns>
        public List<TradeGroup> GetTradeGroups()
        {
            return this.tradeGroupRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets the trade group by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>a list of trade parent groups that match the supplied id</returns>
        public TradeParentGroup GetTradeParentGroupByID(int id)
        {
            return this.tradeParentGroupRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the trade groups.
        /// </summary>
        /// <returns>a list of trade parent groups</returns>
        public List<TradeParentGroup> GetTradeParentGroups()
        {
            return this.tradeParentGroupRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets the trades.
        /// </summary>
        /// <returns>A List of the trades.</returns>
        public List<Trade> GetTrades()
        {
            return this.tradeRepository.GetAll().ToList();
        }
    }
}