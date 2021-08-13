namespace Web.Template.Application.Results.Adaptors
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using AutoMapper;

    using ivci = iVectorConnectInterface;
    using iVectorConnectInterface.Flight;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Results;
    using Web.Template.Application.Results.ResultModels;

    /// <summary>
    /// A class used to adapt from the connect flight result into our domain flight.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Results.IConnectResultComponentAdaptor" />
    public class ConnectFlightAdaptor : IConnectResultComponentAdaptor
    {
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectFlightAdaptor"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        public ConnectFlightAdaptor(IMapper mapper)
        {
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets or sets the type of the component.
        /// </summary>
        /// <value>The type of the component.</value>
        public Type ComponentType => typeof(SearchResponse.Flight);

        /// <summary>
        /// Takes in an connect flight result and outputs our web results model.
        /// </summary>
        /// <param name="connectResult">The connect result.</param>
        /// <param name="searchMode">The search mode.</param>
        /// <param name="context">The context.</param>
        /// <returns>A single result</returns>
        public IResult Create(object connectResult, SearchMode searchMode, HttpContext context)
        {
            var ivcFlightResult = (SearchResponse.Flight)connectResult;
            var flightResult = new FlightResult(this.mapper)
            {
                BookingToken = ivcFlightResult.BookingToken,
                SearchMode = searchMode,
                DepartureAirportId = ivcFlightResult.DepartureAirportID,
                ArrivalAirportId = ivcFlightResult.ArrivalAirportID,
                BaggagePrice = ivcFlightResult.TotalBaggagePrice,
                ExactMatch = ivcFlightResult.ExactMatch,
                FlightCarrierId = ivcFlightResult.FlightCarrierID,
                FlightSupplierId = ivcFlightResult.FlightSupplierID,
                Id = ivcFlightResult.FlightID,
                IncludesSupplierBaggage = ivcFlightResult.IncludesSupplierBaggage,
                MaxStops = Math.Max(ivcFlightResult.NumberOfOutboundStops, ivcFlightResult.NumberOfReturnStops),
                BaggageDescription = ivcFlightResult.BaggageDescription,
                IncludedBaggageAllowance = ivcFlightResult.IncludedBaggageAllowance,
                IncludedBaggageWeight = ivcFlightResult.IncludedBaggageWeight,
                IncludedBaggageText = ivcFlightResult.IncludedBaggageText,
                OutboundFlightDetails =
                                           new FlightDetails()
                                           {
                                               NumberOfStops = ivcFlightResult.NumberOfOutboundStops,
                                               DepartureDate = ivcFlightResult.OutboundDepartureDate,
                                               DepartureTime = ivcFlightResult.OutboundDepartureTime,
                                               ArrivalDate = ivcFlightResult.OutboundArrivalDate,
                                               ArrivalTime = ivcFlightResult.OutboundArrivalTime,
                                               FlightCode = ivcFlightResult.OutboundFlightCode,
                                               FlightClassId = ivcFlightResult.OutboundFlightClassID,
                                               OperatingFlightCarrierId = ivcFlightResult.OutboundOperatingFlightCarrierID,
                                               FareCode = ivcFlightResult.OutboundFareCode
                                           },
                ReturnFlightDetails =
                                           new FlightDetails()
                                           {
                                               NumberOfStops = ivcFlightResult.NumberOfReturnStops,
                                               DepartureDate = ivcFlightResult.ReturnDepartureDate,
                                               DepartureTime = ivcFlightResult.ReturnDepartureTime,
                                               ArrivalDate = ivcFlightResult.ReturnArrivalDate,
                                               ArrivalTime = ivcFlightResult.ReturnArrivalTime,
                                               FlightCode = ivcFlightResult.ReturnFlightCode,
                                               FlightClassId = ivcFlightResult.ReturnFlightClassID,
                                               OperatingFlightCarrierId = ivcFlightResult.ReturnOperatingFlightCarrierID,
                                               FareCode = ivcFlightResult.ReturnFareCode
                                           },
                Price = ivcFlightResult.TotalPrice,
                TotalPrice = ivcFlightResult.TotalPrice,
                SupplierId = ivcFlightResult.SupplierDetails?.SupplierID ?? 0,
                Source = ivcFlightResult.Source,
                FlightSectors = new List<FlightSector>()
            };

            if (ivcFlightResult.MultiCarrierDetails != null)
            {
                var returnMulticarrierDetails = new MultiCarrierDetails() { BookingToken = ivcFlightResult.MultiCarrierDetails.BookingToken, Price = 0 };
                flightResult.ReturnMultiCarrierDetails = returnMulticarrierDetails;
            }

            foreach (ivci.Support.FlightSector connectFlightSector in ivcFlightResult.FlightSectors)
            {
                var flightSector = new FlightSector
                {
                    ArrivalAirportID = connectFlightSector.ArrivalAirportID,
                    DepartureAirportID = connectFlightSector.DepartureAirportID,
                    DepartureTime = connectFlightSector.DepartureTime,
                    ArrivalDate = connectFlightSector.ArrivalDate,
                    ArrivalTime = connectFlightSector.ArrivalTime,
                    DepartureDate = connectFlightSector.DepartureDate,
                    FlightCarrierID = connectFlightSector.FlightCarrierID,
                    Sequence = connectFlightSector.Seq,
                    FlightCode = connectFlightSector.FlightCode,
                    FlightTime = connectFlightSector.FlightTime,
                    NumberOfStops = connectFlightSector.NumberOfStops,
                    TravelTime = connectFlightSector.TravelTime,
                    Direction = connectFlightSector.Direction,
                };
                flightResult.FlightSectors.Add(flightSector);
            }

            flightResult.ComponentToken = flightResult.GetHashCode();

            return flightResult;
        }

        /// <summary>
        /// Sets the arrival date.
        /// </summary>
        /// <param name="date">The date.</param>
        public void SetArrivalDate(DateTime date)
        {
        }

        /// <summary>
        /// Sets the duration.
        /// </summary>
        /// <param name="newDuration">The new duration.</param>
        public void SetDuration(int newDuration)
        {
        }
    }
}