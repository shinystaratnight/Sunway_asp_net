namespace Web.Template.Application.Results.ResultModels
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using AutoMapper;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Class representing a single flight result
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Models.IResult" />
    public class FlightResult : IResult
    {
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightResult"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        public FlightResult(IMapper mapper)
        {
            this.mapper = mapper;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightResult"/> class.
        /// </summary>
        public FlightResult()
        {
        }

        /// <summary>
        /// Gets or sets the arrival airport identifier.
        /// </summary>
        /// <value>
        /// The arrival airport identifier.
        /// </value>
        public int ArrivalAirportId { get; set; }

        /// <summary>
        /// Gets or sets the baggage description.
        /// </summary>
        /// <value>
        /// The baggage description.
        /// </value>
        public string BaggageDescription { get; set; }

        /// <summary>
        /// Gets or sets the baggage price.
        /// </summary>
        /// <value>
        /// The baggage price.
        /// </value>
        public decimal BaggagePrice { get; set; }

        /// <summary>
        /// Gets or sets the booking token.
        /// </summary>
        /// <value>
        /// The booking token, the unique identifier connect users for the result
        /// </value>
        public string BookingToken { get; set; }

        /// <summary>
        /// Gets or sets the component token.
        /// </summary>
        /// <value>
        /// The component token.
        /// </value>
        public int ComponentToken { get; set; }

        /// <summary>
        /// Gets or sets the departure airport identifier.
        /// </summary>
        /// <value>
        /// The departure airport identifier.
        /// </value>
        public int DepartureAirportId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [exact match].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [exact match]; otherwise, <c>false</c>.
        /// </value>
        public bool ExactMatch { get; set; }

        /// <summary>
        /// Gets or sets the flight carrier identifier.
        /// </summary>
        /// <value>
        /// The flight carrier identifier.
        /// </value>
        public int FlightCarrierId { get; set; }

        /// <summary>
        /// Gets or sets the flight sectors.
        /// </summary>
        /// <value>
        /// The flight sectors.
        /// </value>
        public List<FlightSector> FlightSectors { get; set; }

        /// <summary>
        /// Gets or sets the flight supplier identifier.
        /// </summary>
        /// <value>The flight supplier identifier.</value>
        public int FlightSupplierId { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the included baggage allowance.
        /// </summary>
        /// <value>
        /// The included baggage allowance.
        /// </value>
        public int IncludedBaggageAllowance { get; set; }

        /// <summary>
        /// Gets or sets the included baggage weight.
        /// </summary>
        /// <value>
        /// The included baggage weight.
        /// </value>
        public decimal IncludedBaggageWeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [includes supplier baggage].
        /// </summary>
        /// <value>
        /// <c>true</c> if [includes supplier baggage]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludesSupplierBaggage { get; set; }

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
        /// Gets or sets the price.
        /// </summary>
        /// <value>
        /// The price.
        /// </value>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the return flight details.
        /// </summary>
        /// <value>The return flight details.</value>
        public FlightDetails ReturnFlightDetails { get; set; }

        /// <summary>
        /// Gets or sets the search mode.
        /// </summary>
        /// <value>The search mode.</value>
        public SearchMode SearchMode { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the sub results.
        /// </summary>
        /// <value>
        /// The sub results.
        /// </value>
        [XmlIgnore]
        public List<ISubResult> SubResults { get; set; }

        /// <summary>
        /// Gets or sets the supplier identifier.
        /// </summary>
        /// <value>
        /// The supplier identifier.
        /// </value>
        public int SupplierId { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        /// <value>
        /// The total price.
        /// </value>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the MultiCarrier Details.
        /// </summary>
        public MultiCarrierDetails ReturnMultiCarrierDetails { get; set; }

        /// <summary>
        /// Gets or sets the Included Baggage Text
        /// </summary>
        public string IncludedBaggageText { get; set; }

        /// <summary>
        /// Creates the basket component.
        /// </summary>
        /// <returns>The Transfer component.</returns>
        public IBasketComponent CreateBasketComponent()
        {
            return this.mapper.Map<IResult, Flight>(this);
        }
    }

    public class MultiCarrierDetails
    {
        public string BookingToken { get; set; }
        public decimal Price { get; set; }
        public Decimal TotalCommission { get; set; }
        public int FlightBookingId { get; set; }
        public string SearchBookingToken { get; set; }
        public int SupplierId { get; set; }

        public string TermsAndConditions { get; set; }

        public string TermsAndConditionsUrl { get; set; }
    }
}