namespace Web.Booking.API.Lookup
{
    using System.Collections.Generic;
    using System.Web.Http;

    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Application.Lookup.Services;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Entities.Payment;

    /// <summary>
    /// Payment controller responsible for exposing payment lookup to the front end.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class TradeController : ApiController
    {
        /// <summary>
        /// The payment service
        /// </summary>
        private readonly ITradeLookupService tradeLookupService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeController"/> class.
        /// </summary>
        /// <param name="tradeLookupService">The trade lookup service.</param>
        public TradeController(ITradeLookupService tradeLookupService)
        {
            this.tradeLookupService = tradeLookupService;
        }

        /// <summary>
        /// Gets all trades.
        /// </summary>
        /// <returns>A list of the trades</returns>
        [Route("api/trade")]
        [HttpGet]
        public List<Trade> GetAllTrades()
        {
            return this.tradeLookupService.GetTrades();
        }

        /// <summary>
        /// Gets all trade groups.
        /// </summary>
        /// <returns></returns>
        [Route("api/tradegroup")]
        [HttpGet]
        public List<TradeGroup> GetAllTradeGroups()
        {
            return this.tradeLookupService.GetTradeGroups();
        }

        /// <summary>
        /// Gets the trade groups by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [Route("api/tradegroup/{id}")]
        [HttpGet]
        public TradeGroup GetTradeGroupsById(int id)
        {
            return this.tradeLookupService.GetTradeGroupByID(id);
        }

        /// <summary>
        /// Gets all trade groups.
        /// </summary>
        /// <returns></returns>
        [Route("api/tradeparentgroup")]
        [HttpGet]
        public List<TradeParentGroup> GetAllTradeParentGroups()
        {
            return this.tradeLookupService.GetTradeParentGroups();
        }

        /// <summary>
        /// Gets the trade groups by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [Route("api/tradeparentgroup/{id}")]
        [HttpGet]
        public TradeParentGroup GetTradeParentGroupsById(int id)
        {
            return this.tradeLookupService.GetTradeParentGroupByID(id);
        }

        /// <summary>
        /// Gets all trade groups.
        /// </summary>
        /// <returns></returns>
        [Route("api/tradecontactgroup")]
        [HttpGet]
        public List<TradeContactGroup> GetAllTradeContactGroups()
        {
            return this.tradeLookupService.GetTradeContactGroups();
        }

        /// <summary>
        /// Gets the trade groups by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [Route("api/tradecontactgroups/{id}")]
        [HttpGet]
        public TradeContactGroup GetTradeContactGroupById(int id)
        {
            return this.tradeLookupService.GetTradeContactGroupByID(id);
        }
    }
}