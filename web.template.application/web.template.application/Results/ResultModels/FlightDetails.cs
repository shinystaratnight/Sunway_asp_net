namespace Web.Template.Application.Results.ResultModels
{
    using System;

    /// <summary>
    /// Class FlightDetails.
    /// </summary>
    public class FlightDetails
    {
        /// <summary>
        /// Gets or sets the arrival date.
        /// </summary>
        /// <value>The arrival date.</value>
        public DateTime ArrivalDate { get; set; }

        /// <summary>
        /// Gets or sets the arrival time.
        /// </summary>
        /// <value>The arrival time.</value>
        public string ArrivalTime { get; set; }

        /// <summary>
        /// Gets or sets the departure date.
        /// </summary>
        /// <value>The departure date.</value>
        public DateTime DepartureDate { get; set; }

        /// <summary>
        /// Gets or sets the departure time.
        /// </summary>
        /// <value>The departure time.</value>
        public string DepartureTime { get; set; }

        /// <summary>
        /// Gets or sets the fare code.
        /// </summary>
        /// <value>The fare code.</value>
        public string FareCode { get; set; }

        /// <summary>
        /// Gets or sets the flight class identifier.
        /// </summary>
        /// <value>The flight class identifier.</value>
        public int FlightClassId { get; set; }

        /// <summary>
        /// Gets or sets the flight code.
        /// </summary>
        /// <value>The flight code.</value>
        public string FlightCode { get; set; }

        /// <summary>
        /// Gets or sets the number of stops.
        /// </summary>
        /// <value>The number of stops.</value>
        public int NumberOfStops { get; set; }

        /// <summary>
        /// Gets or sets the operating flight carrier identifier.
        /// </summary>
        /// <value>The operating flight carrier identifier.</value>
        public int OperatingFlightCarrierId { get; set; }
    }
}