namespace Web.Template.Application.Tests.Adaptors.IVectorConnect.Results
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using AutoMapper;

    using ivci = iVectorConnectInterface;
    using iVectorConnectInterface.Flight;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Results.Adaptors;
    using Web.Template.Application.Results.Factories;
    using Web.Template.Application.Results.ResultModels;
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;

    /// <summary>
    ///     Test class for testing the property results factory
    /// </summary>
    [TestFixture]
    public class ConnectFlightResultAdaptorTests
    {
        /// <summary>
        ///     Create should Map values from connect request
        /// </summary>
        [Test]
        public void Create_Should_CreateAflightWithMatchingInformation_When_ConnectReturnsResult()
        {
            // Arrange
            SearchResponse ivcSearchResult = BuildConnectResult(3);

            var carrierRepoMock = new Mock<IFlightCarrierRepository>();
            carrierRepoMock.Setup(crm => crm.GetSingle(3, null)).Returns(new FlightCarrier { Name = "TestCarrierA" });
            carrierRepoMock.Setup(crm => crm.GetSingle(2, null)).Returns(new FlightCarrier { Name = "TestCarrierB" });

            var airportRepoMock = new Mock<IAirportRepository>();
            airportRepoMock.Setup(arm => arm.GetSingle(1, null)).Returns(new Airport { Name = "AirportA", IATACode = "APA" });
            airportRepoMock.Setup(arm => arm.GetSingle(4, null)).Returns(new Airport { Name = "AirportB", IATACode = "APB" });

            var vehicleRepoMock = new Mock<IVehicleRepository>();
            vehicleRepoMock.Setup(vrm => vrm.GetSingle(5, null)).Returns(new Vehicle { Name = "Plane" });


            var componentAdaptorFactoryMock = new Mock<IIvConnectResultComponentAdaptorFactory>();

            var mapperMock = new Mock<IMapper>();

            componentAdaptorFactoryMock.Setup(car => car.CreateAdaptorByComponentType(typeof(iVectorConnectInterface.Flight.SearchResponse.Flight))).Returns(new ConnectFlightAdaptor(mapperMock.Object));

            var resultAdaptor = new ConnectFlightResultAdaptor(componentAdaptorFactoryMock.Object);

            var searchModel = new Mock<ISearchModel>();
            searchModel.SetupGet(sm => sm.SearchMode).Returns(SearchMode.FlightPlusHotel);

            //// Act
            List<IResultsModel> result = resultAdaptor.Create(ivcSearchResult, searchModel.Object, HttpContext.Current);

            var flightResult = (FlightResult)result[0].ResultsCollection[0];

            //// Assert
            Assert.AreEqual(flightResult.BookingToken, "test Booking Token");
            Assert.AreEqual(flightResult.DepartureAirportId, 2);
            Assert.AreEqual(flightResult.ArrivalAirportId, 1);
            Assert.AreEqual(flightResult.BaggagePrice, 23.00);
            Assert.AreEqual(flightResult.ExactMatch, true);
            Assert.AreEqual(flightResult.FlightCarrierId, 3);
            Assert.AreEqual(flightResult.MaxStops, 2);

            Assert.AreEqual(flightResult.ReturnFlightDetails.NumberOfStops, 2);
            Assert.AreEqual(flightResult.OutboundFlightDetails.NumberOfStops, 1);
            Assert.AreEqual(flightResult.OutboundFlightDetails.DepartureTime, "11:00");
            Assert.AreEqual(flightResult.OutboundFlightDetails.ArrivalDate, DateTime.Now.AddDays(33).Date);
            Assert.AreEqual(flightResult.OutboundFlightDetails.DepartureDate, DateTime.Now.AddDays(33).Date);
            Assert.AreEqual(flightResult.OutboundFlightDetails.FlightClassId, 4);
            Assert.AreEqual(flightResult.OutboundFlightDetails.FlightCode, "CRO");
            Assert.AreEqual(flightResult.TotalPrice, 100.00);
            Assert.AreEqual(flightResult.ReturnFlightDetails.DepartureTime, "13:00");

            Assert.AreEqual(flightResult.FlightSectors.Count, 2);

            Assert.AreEqual(flightResult.FlightSectors[0].ArrivalAirportID, 4);
            Assert.AreEqual(flightResult.FlightSectors[0].DepartureAirportID, 1);
            Assert.AreEqual(flightResult.FlightSectors[0].DepartureTime, "13:00");
            Assert.AreEqual(flightResult.FlightSectors[0].ArrivalDate, DateTime.Now.AddDays(33).Date);
            Assert.AreEqual(flightResult.FlightSectors[0].ArrivalTime, "10:00");
            Assert.AreEqual(flightResult.FlightSectors[0].DepartureDate, DateTime.Now.AddDays(33).Date);
            Assert.AreEqual(flightResult.FlightSectors[0].FlightCarrierID, 2);
            Assert.AreEqual(flightResult.FlightSectors[0].Sequence, 1);
            Assert.AreEqual(flightResult.FlightSectors[0].FlightCode, "CRD");
            Assert.AreEqual(flightResult.FlightSectors[0].FlightTime, 1200);
            Assert.AreEqual(flightResult.FlightSectors[0].NumberOfStops, 1);
            Assert.AreEqual(flightResult.FlightSectors[0].TravelTime, 1200);
            Assert.AreEqual(flightResult.FlightSectors[0].Direction, "outbound");
        }

        /// <summary>
        ///     When evaluated the response type the connect flight results adaptor is mapped to should be flight
        /// </summary>
        [Test]
        public void ResponseType_Should_BeFlight_When_Evaluated()
        {

            var componentAdaptorFactoryMock = new Mock<IIvConnectResultComponentAdaptorFactory>();

            var resultAdaptor = new ConnectFlightResultAdaptor(componentAdaptorFactoryMock.Object);

            Assert.AreEqual(resultAdaptor.ResponseType, typeof(SearchResponse));
        }

        /// <summary>
        /// Builds the connect result.
        /// </summary>
        /// <param name="numberOfRooms">The number of rooms.</param>
        /// <returns>
        /// A property search response used for faking
        /// </returns>
        private static SearchResponse BuildConnectResult(int numberOfRooms = 1)
        {
            var ivcSearchResult = new SearchResponse.Flight
                                      {
                                          BookingToken = "test Booking Token", 
                                          AltReturnAirportID = 0, 
                                          ArrivalAirportID = 1, 
                                          BookingClass = "test", 
                                          DepartureAirportID = 2, 
                                          ExactMatch = true, 
                                          FlightCarrierID = 3, 
                                          FlightErrata = new List<ivci.Support.FlightErratum>(), 
                                          FlightSectors =
                                              new List<ivci.Support.FlightSector>
                                                  {
                                                      new ivci.Support.FlightSector
                                                          {
                                                              ArrivalDate = DateTime.Now.AddDays(33).Date, 
                                                              ArrivalAirportID = 4, 
                                                              ArrivalTime = "10:00", 
                                                              ArrivalTerminalID = 3, 
                                                              BookingClass = "test", 
                                                              DepartureDate = DateTime.Now.AddDays(33).Date, 
                                                              DepartureAirportID = 1, 
                                                              DepartureTime = "13:00", 
                                                              DepartureTerminalID = 9, 
                                                              Direction = "outbound", 
                                                              FlightCarrierID = 2, 
                                                              Seq = 1, 
                                                              NumberOfStops = 1, 
                                                              TravelTime = 1200, 
                                                              FlightTime = 1200, 
                                                              FlightCode = "CRD", 
                                                              FlightClassID = 1, 
                                                              VehicleID = 5
                                                          }, 
                                                      new ivci.Support.FlightSector
                                                          {
                                                              ArrivalDate = DateTime.Now.AddDays(40).Date, 
                                                              ArrivalAirportID = 1, 
                                                              ArrivalTime = "12:00", 
                                                              ArrivalTerminalID = 3, 
                                                              BookingClass = "test", 
                                                              DepartureDate = DateTime.Now.AddDays(40).Date, 
                                                              DepartureAirportID = 4, 
                                                              DepartureTime = "15:00", 
                                                              DepartureTerminalID = 9, 
                                                              Direction = "return", 
                                                              FlightCarrierID = 2, 
                                                              Seq = 2, 
                                                              NumberOfStops = 2, 
                                                              TravelTime = 1200, 
                                                              FlightTime = 1200, 
                                                              FlightCode = "CRD", 
                                                              FlightClassID = 1, 
                                                              VehicleID = 5
                                                          }
                                                  }, 
                                          Source = "Amadeus", 
                                          ReturnDepartureTime = "13:00", 
                                          NumberOfOutboundStops = 1, 
                                          NumberOfReturnStops = 2, 
                                          OutboundDepartureTime = "11:00", 
                                          TotalBaggagePrice = new decimal(23.0), 
                                          OutboundDepartureDate = DateTime.Now.AddDays(33).Date, 
                                          TotalPrice = new decimal(100.00), 
                                          OutboundFlightClassID = 4, 
                                          OutboundArrivalDate = DateTime.Now.AddDays(33).Date, 
                                          OutboundFlightCode = "CRO", 
                                          FlightSupplierID = 9, 
                                          HotelArrivalDate = DateTime.Now.AddDays(33).Date, 
                                          HotelDuration = 7, 
                                          IncludesSupplierBaggage = false, 
                                          MultiCarrierDetails = null, 
                                          OutboundArrivalTime = "15:00", 
                                          OutboundFareCode = "test", 
                                          OutboundOperatingFlightCarrierID = 3, 
                                          ProductAttributes = null, 
                                          ReturnArrivalDate = DateTime.Now.AddDays(40).Date, 
                                          ReturnArrivalTime = "17:00", 
                                          ReturnDepartureDate = DateTime.Now.AddDays(40).Date, 
                                          ReturnFareCode = "test2", 
                                          ReturnFlightClassID = 5, 
                                          ReturnFlightCode = "CRD", 
                                          ReturnOperatingFlightCarrierID = 9, 
                                          Saving = 5, 
                                          SupplierDetails = new ivci.Support.SupplierDetails(), 
                                          TPSessionID = "123", 
                                          TotalCommission = 5, 
                                          TotalSeatCost = 100, 
                                          TotalSeatPrice = 120
                                      };

            var searchResponse = new SearchResponse { Flights = new List<SearchResponse.Flight> { ivcSearchResult } };

            return searchResponse;
        }
    }
}