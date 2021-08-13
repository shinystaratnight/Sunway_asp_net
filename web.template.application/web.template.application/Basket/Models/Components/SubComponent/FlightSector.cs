namespace Web.Template.Application.Basket.Models.Components.SubComponent
{
    using System;

    /// <summary>
    /// Class representing a single flight sector
    /// </summary>
    public class FlightSector
    {
        /// <summary>
        ///     Gets or sets the arrival airport.
        /// </summary>
        /// <value>
        ///     The arrival airport.
        /// </value>
        public string ArrivalAirport { get; set; }

        /// <summary>
        ///     Gets or sets the arrival airport code.
        /// </summary>
        /// <value>
        ///     The arrival airport code.
        /// </value>
        public string ArrivalAirportCode { get; set; }

        /// <summary>
        ///     Gets or sets the arrival airport identifier.
        /// </summary>
        /// <value>
        ///     The arrival airport identifier.
        /// </value>
        public int ArrivalAirportID { get; set; }

        /// <summary>
        ///     Gets or sets the arrival date.
        /// </summary>
        /// <value>
        ///     The arrival date.
        /// </value>
        public DateTime ArrivalDate { get; set; }

        /// <summary>
        ///     Gets or sets the arrival time.
        /// </summary>
        /// <value>
        ///     The arrival time.
        /// </value>
        public string ArrivalTime { get; set; }

        /// <summary>
        ///     Gets or sets the departure airport.
        /// </summary>
        /// <value>
        ///     The departure airport.
        /// </value>
        public string DepartureAirport { get; set; }

        /// <summary>
        ///     Gets or sets the departure airport code.
        /// </summary>
        /// <value>
        ///     The departure airport code.
        /// </value>
        public string DepartureAirportCode { get; set; }

        /// <summary>
        ///     Gets or sets the departure airport identifier.
        /// </summary>
        /// <value>
        ///     The departure airport identifier.
        /// </value>
        public int DepartureAirportID { get; set; }

        /// <summary>
        ///     Gets or sets the departure date.
        /// </summary>
        /// <value>
        ///     The departure date.
        /// </value>
        public DateTime DepartureDate { get; set; }

        /// <summary>
        ///     Gets or sets the departure time.
        /// </summary>
        /// <value>
        ///     The departure time.
        /// </value>
        public string DepartureTime { get; set; }

        /// <summary>
        ///     Gets or sets the direction.
        /// </summary>
        /// <value>
        ///     The direction.
        /// </value>
        public string Direction { get; set; }

        /// <summary>
        ///     Gets or sets the flight carrier.
        /// </summary>
        /// <value>
        ///     The flight carrier.
        /// </value>
        public string FlightCarrier { get; set; }

        /// <summary>
        ///     Gets or sets the flight carrier identifier.
        /// </summary>
        /// <value>
        ///     The flight carrier identifier.
        /// </value>
        public int FlightCarrierID { get; set; }

        /// <summary>
        ///     Gets or sets the flight code.
        /// </summary>
        /// <value>
        ///     The flight code.
        /// </value>
        public string FlightCode { get; set; }

        /// <summary>
        ///     Gets or sets the flight time.
        /// </summary>
        /// <value>
        ///     The flight time.
        /// </value>
        public int FlightTime { get; set; }

        /// <summary>
        ///     Gets or sets the number of stops.
        /// </summary>
        /// <value>
        ///     The number of stops.
        /// </value>
        public int NumberOfStops { get; set; }

        /// <summary>
        ///     Gets or sets the sequence.
        /// </summary>
        /// <value>
        ///     The sequence.
        /// </value>
        public int Sequence { get; set; }

        /// <summary>
        ///     Gets or sets the travel time.
        /// </summary>
        /// <value>
        ///     The travel time.
        /// </value>
        public int TravelTime { get; set; }

        /// <summary>
        /// Gets or sets the vehicle name.
        /// </summary>
        /// <value>
        /// The vehicle name.
        /// </value>
        public string VehicleName { get; set; }
    }
}