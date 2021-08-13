namespace Web.Template.Application.Quote.Models
{
    using System.Collections.Generic;
    using Web.Template.Application.Results.ResultModels;

    /// <summary>
    /// Class QuoteDocumentationFlightResult.
    /// </summary>
    public class QuoteDocumentationFlightResult
    {
        /// <summary>
        /// Gets or sets the Sector Additional Information
        /// </summary>
        private List<QuoteDocumentationSectorAdditionalInformation> quoteDocumentationSectorAdditionalInformation = new List<QuoteDocumentationSectorAdditionalInformation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteDocumentationFlightResult" /> class.
        /// </summary>
        public QuoteDocumentationFlightResult()
        {
        }

        /// <summary>
        /// Gets or sets the Flight Result
        /// </summary>
        public FlightResult FlightResult { get; set; }

        /// <summary>
        /// Gets or sets the Carrier Name
        /// </summary>
        public string CarrierName { get; set; }

        /// <summary>
        /// Gets or sets the Carrier Logo
        /// </summary>
        public string CarrierLogo { get; set; }

        /// <summary>
        /// Gets or sets the Sector Additional Information
        /// </summary>
        public List<QuoteDocumentationSectorAdditionalInformation> QuoteDocumentationSectorAdditionalInformation
        {
            get { return this.quoteDocumentationSectorAdditionalInformation; }
            set { this.quoteDocumentationSectorAdditionalInformation = value; }
        }
    }
}
