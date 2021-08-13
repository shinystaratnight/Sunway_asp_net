namespace Web.Template.Application.Basket.Models.Components
{
    using System.Collections.Generic;
    using System.Linq;

    using Web.Template.Application.Basket.Models.Components.SubComponent;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Results.ResultModels;

    using FlightSector = Web.Template.Application.Basket.Models.Components.SubComponent.FlightSector;

    /// <summary>
    /// A class representing a flight
    /// </summary>
    /// <seealso cref="BasketCompontentBase" />
    public class Flight : BasketCompontentBase
    {
        /// <summary>
        /// Gets or sets the arrival airport identifier.
        /// </summary>
        /// <value>
        /// The arrival airport identifier.
        /// </value>
        public int ArrivalAirportId { get; set; }

        /// <summary>
        /// Gets or sets the baggage price.
        /// </summary>
        /// <value>
        /// The baggage price.
        /// </value>
        public decimal BaggagePrice { get; set; }

        /// <summary>
        /// Gets or sets the type of the component.
        /// </summary>
        /// <value>
        /// The type of the component.
        /// </value>
        public override ComponentType ComponentType => ComponentType.Flight;

        /// <summary>
        /// Gets or sets the departure airport identifier.
        /// </summary>
        /// <value>
        /// The departure airport identifier.
        /// </value>
        public int DepartureAirportId { get; set; }

        /// <summary>
        /// Gets or sets the flight carrier identifier.
        /// </summary>
        /// <value>
        /// The flight carrier identifier.
        /// </value>
        public int FlightCarrierId { get; set; }

        /// <summary>
        /// Gets or sets the flight supplier identifier.
        /// </summary>
        /// <value>
        /// The flight supplier identifier.
        /// </value>
        public int FlightSupplierId { get; set; }
        
        /// <summary>
        /// Gets the flight extras price.
        /// </summary>
        /// <value>
        /// The flight extras price.
        /// </value>
        public decimal FlightExtrasPrice
        {
            get
            {
                decimal totalPrice = 0;

                totalPrice += this.SubComponents.Sum(
                    sc =>
                        {
                            var flightExtra = (FlightExtra)sc;
                            return flightExtra.TotalPrice * flightExtra.QuantitySelected;
                        });

                return totalPrice;
            }
        }

        /// <summary>
        /// Gets or sets the flight sectors.
        /// </summary>
        /// <value>
        /// The flight sectors.
        /// </value>
        public List<FlightSector> FlightSectors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [includes supplier baggage].
        /// </summary>
        /// <value>
        /// <c>true</c> if [includes supplier baggage]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludesSupplierBaggage { get; set; }

        /// <summary>
        /// Gets or sets the included baggage text.
        /// </summary>
        /// <value>
        /// The included baggage text
        /// </value>
        public string IncludedBaggageText { get; set; }

        /// <summary>
        /// Gets or sets the maximum stops.
        /// </summary>
        /// <value>
        /// The maximum stops.
        /// </value>
        public int MaxStops { get; set; }

        /// <summary>
        /// Gets or sets the outbound flight details.
        /// </summary>
        /// <value>The outbound flight details.</value>
        public FlightDetails OutboundFlightDetails { get; set; }

        /// <summary>
        /// Gets or sets the return flight details.
        /// </summary>
        /// <value>The return flight details.</value>
        public FlightDetails ReturnFlightDetails { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the supplier identifier.
        /// </summary>
        /// <value>
        /// The supplier identifier.
        /// </value>
        public int SupplierId { get; set; }

        public MultiCarrierDetails ReturnMultiCarrierDetails { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>
        /// The price.
        /// </value>
        public override decimal TotalPrice
        {
            get
            {
                return this.Price + this.FlightExtrasPrice;
            }

            set
            {
            }
        }
    }
}