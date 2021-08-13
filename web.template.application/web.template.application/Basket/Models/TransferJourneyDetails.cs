namespace Web.Template.Application.Basket.Models
{
    using System;

    /// <summary>
    /// Class TransferDetails.
    /// </summary>
    public class TransferJourneyDetails
    {
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the flight code.
        /// </summary>
        /// <value>The flight code.</value>
        public string FlightCode { get; set; }

        /// <summary>
        /// Gets or sets the journey time.
        /// </summary>
        /// <value>The journey time.</value>
        public string JourneyTime { get; set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>The time.</value>
        public string Time { get; set; }
    }
}