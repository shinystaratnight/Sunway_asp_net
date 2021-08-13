namespace Web.Template.Application.Quote.Models
{
    using System.Collections.Generic;
    using Web.Template.Application.Results.ResultModels;
    using Web.Template.Application.User.Models;

    /// <summary>
    /// Class QuoteDocumentationModel.
    /// </summary>
    public class QuoteDocumentationModel
    {
        /// <summary>
        /// Gets or sets the room options available on the current property
        /// </summary>
        private List<QuoteDocumentationRoomOption> quoteDocumentationRoomOptions = new List<QuoteDocumentationRoomOption>();

        /// <summary>
        /// Gets or sets the Flight Result merged with additional data required for the xml document
        /// </summary>
        public QuoteDocumentationFlightResult FlightResult { get; set; }

        /// <summary>
        /// Gets or sets the Property Result
        /// </summary>
        public PropertyResult PropertyResult { get; set; }

        /// <summary>
        /// Gets or sets the current trade session
        /// </summary>
        public TradeSession TradeSession { get; set; }

        /// <summary>
        /// Gets or sets the Trade information
        /// </summary>
        public Domain.Entities.Booking.Trade Trade { get; set; }

        /// <summary>
        /// Gets or sets the selling currency symbol, used in the xml document for price displays
        /// </summary>
        public string SellingCurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets the cms base url, used in xml document for image pathing
        /// </summary>
        public string CmsBaseURL { get; set; }

        /// <summary>
        /// Gets or sets the room options available on the current property
        /// </summary>
        public List<QuoteDocumentationRoomOption> QuoteDocumentationRoomOptions
        {
            get { return this.quoteDocumentationRoomOptions; }
            set { this.quoteDocumentationRoomOptions = value; }
        }
    }
}
