namespace Web.Template.Application.Quote.Adaptors
{
    using System.Collections.Generic;
    using System.Linq;

    using iVectorConnectInterface;
    using iVectorConnectInterface.Flight;

    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Basket.Models.Components.SubComponent;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Quote.Adaptors;
    using Web.Template.Application.Results.ResultModels;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class QuoteRetrieveBasketAdaptor.
    /// </summary>
    public class QuoteRetrieveBasketAdaptor : IQuoteRetrieveBasketAdaptor
    {
        /// <summary>
        /// Creates the property component.
        /// </summary>
        /// <param name="quoteProperty">The quote property.</param>
        /// <param name="guestDetails">The guest details.</param>
        /// <returns>The hotel component.</returns>
        public Hotel CreatePropertyComponent(QuoteProperty quoteProperty, List<ivci.Support.GuestDetail> guestDetails)
        {
            var hotel = new Hotel()
                            {
                                BookingToken = quoteProperty.BookingToken,
                                ComponentPreBooked = true,
                                ArrivalDate = quoteProperty.ArrivalDate,
                                Duration = quoteProperty.Duration,
                                GeographyLevel1Id = quoteProperty.GeographyLevel1ID,
                                GeographyLevel2Id = quoteProperty.GeographyLevel2ID,
                                GeographyLevel3Id = quoteProperty.GeographyLevel3ID,
                                PropertyReferenceId = quoteProperty.PropertyReferenceID,
                                TotalPrice = quoteProperty.TotalPrice,
                                TotalCommission = quoteProperty.TotalCommission,
                                VATOnCommission = quoteProperty.VATOnCommission,
                                TermsAndConditions = quoteProperty.TermsAndConditions,
                                SubComponents = new List<ISubComponent>()
                            };

            foreach (var quoteRoom in quoteProperty.Rooms)
            {
                var room = new Room()
                               {
                                   Adults = quoteRoom.Adults,
                                   Children = quoteRoom.Children,
                                   Infants = quoteRoom.Infants,
                                   ChildAges = new List<int>(),
                                   GuestIDs = quoteRoom.GuestIDs,
                                   BookingToken = quoteRoom.RoomBookingToken,
                                   MealBasisId = quoteRoom.MealBasisID,
                                   RoomType = quoteRoom.RoomType,
                                   Source = quoteProperty.Source,
                                   TotalPrice = quoteRoom.TotalPrice
                               };

                foreach (var guestId in quoteRoom.GuestIDs)
                {
                    var guest = guestDetails.FirstOrDefault(guestDetail => guestDetail.GuestID == guestId);
                    if (guest != null && guest.Type == "Child")
                    {
                        room.ChildAges.Add(guest.Age);
                    }
                }
   
                hotel.SubComponents.Add(room);
            }

            hotel.CancellationCharges = new List<ICancellationCharge>();
            foreach (ivci.Support.Cancellation cancellation in quoteProperty.Cancellations)
            {
                hotel.CancellationCharges.Add(new CancellationCharge()
                                                  {
                                                      Amount = cancellation.Amount,
                                                      EndDate = cancellation.EndDate,
                                                      StartDate = cancellation.StartDate
                                                  });
            }

            hotel.Payments = new List<IPayment>();
            foreach (ivci.Support.PaymentDue paymentDue in quoteProperty.PaymentsDue)
            {
                hotel.Payments.Add(new Payment()
                                       {
                                           Amount = paymentDue.Amount,
                                           DateDue = paymentDue.DateDue
                                       });
            }

            return hotel;
        }

        /// <summary>
        /// Creates the flight component.
        /// </summary>
        /// <param name="quoteFlight">The quote flight.</param>
        /// <param name="guestDetails">The guest details.</param>
        /// <returns>The flight component.</returns>
        public Flight CreateFlightComponent(QuoteFlight quoteFlight, List<ivci.Support.GuestDetail> guestDetails)
        {
            var flight = new Flight()
                             {
                                 BookingToken = quoteFlight.BookingToken,
                                 ComponentPreBooked = true,
                                 GuestIDs = quoteFlight.GuestIDs,
                                 DepartureAirportId = quoteFlight.DepartureAirportID,
                                 ArrivalAirportId = quoteFlight.ArrivalAirportID,
                                 FlightCarrierId = quoteFlight.FlightCarrierID,
                                 Source = quoteFlight.Source,
                                 BaggagePrice = quoteFlight.BaggagePrice,
                                 Price = quoteFlight.TotalPrice,
                                 TotalCommission = quoteFlight.TotalCommission,
                                 VATOnCommission = quoteFlight.VATOnCommission,
                                 TermsAndConditions = quoteFlight.TermsAndConditions,
                                 OutboundFlightDetails = new FlightDetails()
                                                             {
                                                                 DepartureDate = quoteFlight.OutboundDepartureDate,
                                                                 DepartureTime = quoteFlight.OutboundDepartureTime,
                                                                 ArrivalDate = quoteFlight.OutboundArrivalDate,
                                                                 ArrivalTime = quoteFlight.OutboundArrivalTime,
                                                                 FlightClassId = quoteFlight.OutboundFlightClassID,
                                                                 FlightCode = quoteFlight.OutboundFlightCode
                                                             },
                                 ReturnFlightDetails = new FlightDetails()
                                                           {
                                                             DepartureDate = quoteFlight.ReturnDepartureDate,
                                                             DepartureTime = quoteFlight.ReturnDepartureTime,
                                                             ArrivalDate = quoteFlight.ReturnArrivalDate,
                                                             ArrivalTime = quoteFlight.ReturnArrivalTime,
                                                             FlightClassId = quoteFlight.ReturnFlightClassID,
                                                             FlightCode = quoteFlight.ReturnFlightCode
                                                         },
                                 ChildAges = new List<int>(),
                                 SubComponents = new List<ISubComponent>(),
                                 CancellationCharges = new List<ICancellationCharge>(),
                                 Payments = new List<IPayment>()
                             };

            foreach (var guestId in quoteFlight.GuestIDs)
            {
                var guest = guestDetails.FirstOrDefault(guestDetail => guestDetail.GuestID == guestId);

                if (guest != null)
                {
                    flight.Adults += guest.Type == "Adult" ? 1 : 0;
                    flight.Children += guest.Type == "Child" ? 1 : 0;
                    flight.Infants += guest.Type == "Infant" ? 1 : 0;

                    if (guest.Type == "Child")
                    {
                        flight.ChildAges.Add(guest.Age);
                    }
                }
            }

            var totalPax = flight.Adults + flight.Children + flight.Infants;

            foreach (PreBookResponse.Extra extra in quoteFlight.Extras)
            {
                var flightExtra = new FlightExtra();
                flightExtra.UpdateDetailsFromConnect(extra, totalPax);
                flight.SubComponents.Add(flightExtra);

                flight.Price -= extra.Price * extra.QuantitySelected;
            }

            foreach (ivci.Support.Cancellation cancellation in quoteFlight.Cancellations)
            {
                flight.CancellationCharges.Add(new CancellationCharge()
                {
                    Amount = cancellation.Amount,
                    EndDate = cancellation.EndDate,
                    StartDate = cancellation.StartDate
                });
            }

            foreach (ivci.Support.PaymentDue paymentDue in quoteFlight.PaymentsDue)
            {
                flight.Payments.Add(new Payment()
                {
                    Amount = paymentDue.Amount,
                    DateDue = paymentDue.DateDue
                });
            }

            return flight;
        }

        /// <summary>
        /// Creates the transfer component.
        /// </summary>
        /// <param name="quoteTransfer">The quote transfer.</param>
        /// <param name="guestDetails">The guest details.</param>
        /// <returns>The transfer component.</returns>
        public Transfer CreateTransferComponent(QuoteTransfer quoteTransfer, List<ivci.Support.GuestDetail> guestDetails)
        {
            var transfer = new Transfer()
                               {
                                   BookingToken = quoteTransfer.BookingToken,
                                   ComponentPreBooked = true,

                                   Adults = quoteTransfer.Adults,
                                   Children = quoteTransfer.Children,
                                   Infants = quoteTransfer.Infants,
                                   ChildAges = new List<int>(),

                                   Vehicle = quoteTransfer.Vehicle,
                                   VehicleQuantity = quoteTransfer.VehicleQuantity,

                                   DepartureParentType = quoteTransfer.DepartureParentType,
                                   DepartureParentName = quoteTransfer.DepartureParentName,
                                   DepartureParentId = quoteTransfer.DepartureParentID,
                                   OneWay = quoteTransfer.OneWay,
                                   OutboundJourneyDetails = new TransferJourneyDetails()
                                                                {
                                                                    Date = quoteTransfer.DepartureDate,
                                                                    Time = quoteTransfer.DepartureTime,
                                                                    FlightCode = quoteTransfer.DepartureFlightCode
                                                                },
                                   ReturnJourneyDetails = new TransferJourneyDetails()
                                                              {
                                                                  Date = quoteTransfer.ReturnDate,
                                                                  Time = quoteTransfer.ReturnTime,
                                                                  FlightCode = quoteTransfer.ReturnFlightCode
                                                              },
                                   TotalPrice = quoteTransfer.TotalPrice,
                                   TotalCommission = quoteTransfer.TotalCommission,
                                   VATOnCommission = quoteTransfer.VATOnCommission
                               };

            transfer.CancellationCharges = new List<ICancellationCharge>();
            foreach (ivci.Support.Cancellation cancellation in quoteTransfer.Cancellations)
            {
                transfer.CancellationCharges.Add(new CancellationCharge()
                {
                    Amount = cancellation.Amount,
                    EndDate = cancellation.EndDate,
                    StartDate = cancellation.StartDate
                });
            }

            transfer.Payments = new List<IPayment>();
            foreach (ivci.Support.PaymentDue paymentDue in quoteTransfer.PaymentsDue)
            {
                transfer.Payments.Add(new Payment()
                {
                    Amount = paymentDue.Amount,
                    DateDue = paymentDue.DateDue
                });
            }

            return transfer;
        }
    }
}
