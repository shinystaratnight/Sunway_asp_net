namespace Web.Template.Application.Quote.Models
{
    /// <summary>
    /// Class QuoteDocumentationSectorAdditionalInformation
    /// </summary>
    public class QuoteDocumentationSectorAdditionalInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteDocumentationSectorAdditionalInformation" /> class.
        /// </summary>
        public QuoteDocumentationSectorAdditionalInformation()
        {
        }

        /// <summary>
        /// Gets or sets the Flight Code
        /// </summary>
        public string FlightCode { get; set; }

        /// <summary>
        /// Gets or sets the Departure Airport Name
        /// </summary>
        public string DepartureAirportName { get; set; }

        /// <summary>
        /// Gets or sets the Departure Airport Code
        /// </summary>
        public string DepartureAirportCode { get; set; }

        /// <summary>
        /// Gets or sets the Arrival Airport Name
        /// </summary>
        public string ArrivalAirportName { get; set; }

        /// <summary>
        /// Gets or sets the Arrival Airport Code
        /// </summary>
        public string ArrivalAirportCode { get; set; }

        /// <summary>
        /// Gets or sets the Class Name
        /// </summary>
        public string ClassName { get; set; }
    }
}