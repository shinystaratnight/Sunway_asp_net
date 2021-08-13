namespace Web.Template.Application.Booking.Adapters
{
    using System.Collections.Generic;
    using System.Linq;

    using Intuitive;

    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Basket.Models.Components.SubComponent;
    using Web.Template.Application.Booking.Models;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Booking.Adapters;
    using Web.Template.Application.Interfaces.Booking.Models;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Prebook.Models;
    using Web.Template.Application.Results.ResultModels;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Payment;

    using FlightSector = Web.Template.Application.Basket.Models.Components.SubComponent.FlightSector;
    using ivci = iVectorConnectInterface;

    /// <summary>
    /// A class for adapting content into a booking
    /// </summary>
    /// <seealso cref="IBookingAdapter" />
    public class BookingAdapter : IBookingAdapter
    {
        /// <summary>
        /// The booking object set up by the class
        /// </summary>
        private readonly IBooking booking;

        /// <summary>
        /// The currency repository
        /// </summary>
        private readonly ICurrencyRepository currencyRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingAdapter" /> class.
        /// </summary>
        /// <param name="booking">The booking.</param>
        /// <param name="currencyRepository">The currency repository.</param>
        public BookingAdapter(IBooking booking, ICurrencyRepository currencyRepository)
        {
            this.booking = booking;
            this.currencyRepository = currencyRepository;
        }

        /// <summary>
        /// Creates the booking from get booking details response.
        /// </summary>
        /// <param name="bookingDetailsResponse">The booking details response.</param>
        /// <returns>a booking</returns>
        public IBooking CreateBookingFromGetBookingDetailsResponse(ivci.GetBookingDetailsResponse bookingDetailsResponse)
        {
            this.booking.Reference = bookingDetailsResponse.BookingReference;
            this.booking.BookingDate = bookingDetailsResponse.BookingDate;
            this.booking.Id = bookingDetailsResponse.BookingID;
            this.booking.Components = new List<IBasketComponent>();
            this.booking.Errata = this.AddErratum(bookingDetailsResponse);
            this.BuildLeadGuest(bookingDetailsResponse.LeadCustomer);
            this.booking.Guests = new List<GuestDetail>();
            this.booking.CustomerPayments = new List<CustomerPayment>();
            this.booking.TotalPrice = bookingDetailsResponse.TotalPrice;
            this.booking.TotalPaid = bookingDetailsResponse.TotalPaid;
            this.booking.Currency = this.currencyRepository.GetSingle(bookingDetailsResponse.CurrencyID);

            foreach (ivci.GetBookingDetailsResponse.Flight flight in bookingDetailsResponse.Flights)
            {
                this.booking.Components.Add(this.BuildFlight(flight));
            }

            foreach (ivci.GetBookingDetailsResponse.Property property in bookingDetailsResponse.Properties)
            {
                this.booking.Components.Add(this.BuildHotel(property));
            }

            foreach (ivci.GetBookingDetailsResponse.Transfer transfer in bookingDetailsResponse.Transfers)
            {
                this.booking.Components.Add(this.BuildTransfer(transfer));
            }

            foreach (ivci.GetBookingDetailsResponse.Extra extra in bookingDetailsResponse.Extras)
            {
                this.booking.Components.Add(this.BuildExtra(extra));
            }

            foreach (ivci.GetBookingDetailsResponse.CustomerPayment customerPayment in bookingDetailsResponse.CustomerPayments)
            {
                this.booking.CustomerPayments.Add(this.BuildCustomerPayment(customerPayment));
            }

            return this.booking;
        }

        /// <summary>
        /// Adds the erratum.
        /// </summary>
        /// <param name="bookingDetailsResponse">The booking details response.</param>
        /// <returns>a List of erratum</returns>
        private List<IErratum> AddErratum(ivci.GetBookingDetailsResponse bookingDetailsResponse)
        {
            var erratum = new List<IErratum>();

            foreach (ivci.GetBookingDetailsResponse.Flight flight in bookingDetailsResponse.Flights)
            {
                List<Erratum> flightErrata = flight.FlightErrata.Select(x => new Erratum() { ComponentType = ComponentType.Flight, Description = x.ErratumDescription }).ToList();
                erratum.AddRange(flightErrata);
            }

            foreach (ivci.GetBookingDetailsResponse.Property property in bookingDetailsResponse.Properties)
            {
                List<Erratum> propertyErrata = property.Errata.GroupBy(x => x.ErratumID).Select(x => new Erratum() { ComponentType = ComponentType.Hotel, Description = x.FirstOrDefault().ErratumDescription }).ToList();
                erratum.AddRange(propertyErrata);
            }

            return erratum;
        }

        /// <summary>
        /// Adds the guest if unique.
        /// </summary>
        /// <param name="guestDetails">The guest details.</param>
        private void AddGuestIfUnique(List<ivci.Support.GuestDetail> guestDetails)
        {
            foreach (ivci.Support.GuestDetail ivcGuestDetail in guestDetails)
            {
                if (!this.booking.Guests.Exists(g => g.BookingPassengerID == ivcGuestDetail.GuestID))
                {
                    var guest = new GuestDetail()
                                    {
                                        Age = ivcGuestDetail.Age, 
                                        BookingPassengerID = ivcGuestDetail.BookingPassengerID, 
                                        DateOfBirth = ivcGuestDetail.DateOfBirth, 
                                        FirstName = ivcGuestDetail.FirstName, 
                                        Gender = ivcGuestDetail.Gender, 
                                        GuestID = ivcGuestDetail.GuestID, 
                                        LastName = ivcGuestDetail.LastName, 
                                        MiddleName = ivcGuestDetail.MiddleName, 
                                        NationalityId = ivcGuestDetail.NationalityID, 
                                        Title = ivcGuestDetail.Title
                                    };
                    this.booking.Guests.Add(guest);
                }
            }
        }

        /// <summary>
        /// Builds the customer payment.
        /// </summary>
        /// <param name="ivcCustomerPayment">The connect customer payment.</param>
        /// <returns>The Customer Payment.</returns>
        private CustomerPayment BuildCustomerPayment(ivci.GetBookingDetailsResponse.CustomerPayment ivcCustomerPayment)
        {
            var customerPayment = new CustomerPayment() { DateDue = ivcCustomerPayment.DateDue, CurrencyId = ivcCustomerPayment.CurrencyID, TotalPayment = ivcCustomerPayment.TotalPayment, Outstanding = ivcCustomerPayment.Outstanding };
            return customerPayment;
        }

        /// <summary>
        /// Builds a basket flight from a connect get booking details response flight
        /// </summary>
        /// <param name="ivcFlight">The connect flight.</param>
        /// <returns>a basket flight</returns>
        private Flight BuildFlight(ivci.GetBookingDetailsResponse.Flight ivcFlight)
        {
            var flight = new Flight()
                             {
                                 Adults = ivcFlight.FlightPassengers.Count(fp => fp.GuestType.ToLower() == "adult"), 
                                 Children = ivcFlight.FlightPassengers.Count(fp => fp.GuestType.ToLower() == "child"), 
                                 Infants = ivcFlight.FlightPassengers.Count(fp => fp.GuestType.ToLower() == "infants"), 
                                 ArrivalAirportId = ivcFlight.ArrivalAirportID, 
                                 ArrivalDate = ivcFlight.OutboundArrivalDate, 
                                 BaggagePrice = ivcFlight.BaggagePrice, 
                                 DepartureAirportId = ivcFlight.DepartureAirportID, 
                                 Price = ivcFlight.Price, 
                                 ComponentBooked = true, 
                                 GuestIDs = ivcFlight.FlightPassengers.Select(fp => fp.FlightBookingPassengerID).ToList(), 
                                 SubComponents = new List<ISubComponent>(), 
                                 FlightSectors = new List<FlightSector>(), 
                                 FlightCarrierId = ivcFlight.FlightCarrierID, 
                                 TotalPrice = ivcFlight.Price, 
                                 OutboundFlightDetails =
                                     new FlightDetails()
                                         {
                                             ArrivalDate = ivcFlight.OutboundArrivalDate, 
                                             ArrivalTime = ivcFlight.OutboundArrivalTime, 
                                             DepartureTime = ivcFlight.OutboundDepartureTime, 
                                             DepartureDate = ivcFlight.OutboundDepartureDate, 
                                             OperatingFlightCarrierId = ivcFlight.FlightCarrierID,
                                             FlightCode = ivcFlight.OutboundFlightCode, 
                                             FlightClassId = ivcFlight.OutboundFlightClassID,
                                             NumberOfStops = ivcFlight.FlightSectors.Count(fs => fs.Direction == "Return") - 1
                                         }, 
                                 ReturnFlightDetails =
                                     new FlightDetails()
                                         {
                                             ArrivalDate = ivcFlight.ReturnArrivalDate, 
                                             ArrivalTime = ivcFlight.ReturnArrivalTime, 
                                             DepartureTime = ivcFlight.ReturnDepartureTime, 
                                             DepartureDate = ivcFlight.ReturnDepartureDate, 
                                             OperatingFlightCarrierId = ivcFlight.FlightCarrierID, 
                                             FlightCode = ivcFlight.ReturnFlightCode,
                                             FlightClassId = ivcFlight.ReturnFlightClassID,
                                             NumberOfStops = ivcFlight.FlightSectors.Count(fs => fs.Direction == "Outbound") - 1
                                         }
                             };

            if (flight.OutboundFlightDetails?.NumberOfStops == -1)
            {
                flight.OutboundFlightDetails.NumberOfStops = 0;
            }

            if (flight.ReturnFlightDetails?.NumberOfStops == -1)
            {
                flight.ReturnFlightDetails.NumberOfStops = 0;
            }

            foreach (ivci.GetBookingDetailsResponse.FlightSector ivcFlightSector in ivcFlight.FlightSectors)
            {
                var flightSector = new FlightSector()
                                       {
                                           FlightCode = ivcFlightSector.FlightCode, 
                                           ArrivalAirport = ivcFlightSector.ArrivalAirport, 
                                           ArrivalAirportCode = ivcFlightSector.ArrivalAirportCode, 
                                           ArrivalAirportID = ivcFlightSector.ArrivalAirportID, 
                                           ArrivalDate = ivcFlightSector.ArrivalDate, 
                                           ArrivalTime = ivcFlightSector.ArrivalTime, 
                                           DepartureAirport = ivcFlightSector.DepartureAirport, 
                                           DepartureAirportCode = ivcFlight.DepartureAirportCode, 
                                           DepartureAirportID = ivcFlightSector.DepartureAirportID, 
                                           DepartureDate = ivcFlightSector.DepartureDate, 
                                           DepartureTime = ivcFlightSector.DepartureTime, 
                                           FlightCarrierID = ivcFlightSector.FlightCarrierID, 
                                           FlightCarrier = ivcFlightSector.FlightCarrier, 
                                           Direction = ivcFlightSector.Direction, 
                                           TravelTime = ivcFlightSector.TravelTime, 
                                       };
                flight.FlightSectors.Add(flightSector);
            }

            foreach (ivci.GetBookingDetailsResponse.FlightExtra ivcFlightExtra in ivcFlight.FlightExtras)
            {
                var flightExtra = new FlightExtra()
                                      {
                                          ExtraType = ivcFlightExtra.ExtraType, 
                                          QuantitySelected = ivcFlightExtra.Quantity, 
                                          Description = ivcFlightExtra.Description, 
                                          TotalPrice = decimal.Parse(ivcFlightExtra.Price), 
                                          CostingBasis = ivcFlightExtra.CostingBasis
                                      };
                flight.SubComponents.Add(flightExtra);
            }

            return flight;
        }

        /// <summary>
        /// Builds a basket hotel from a connect get booking details response property
        /// </summary>
        /// <param name="ivcProperty">The connect property.</param>
        /// <returns>a basket hotel</returns>
        private Hotel BuildHotel(ivci.GetBookingDetailsResponse.Property ivcProperty)
        {
            var hotel = new Hotel()
                            {
                                ArrivalDate = ivcProperty.ArrivalDate, 
                                PropertyReferenceId = ivcProperty.PropertyReferenceID, 
                                GeographyLevel1Id = ivcProperty.GeographyLevel1ID, 
                                GeographyLevel2Id = ivcProperty.GeographyLevel2ID, 
                                GeographyLevel3Id = ivcProperty.GeographyLevel3ID, 
                                TotalPrice = ivcProperty.TotalPrice, 
                                Price = ivcProperty.TotalPrice, 
                                SubComponents = new List<ISubComponent>(), 
                                Duration = ivcProperty.Duration, 
                                ComponentBooked = true, 
                            };

            foreach (ivci.GetBookingDetailsResponse.Room ivcRoom in ivcProperty.Rooms)
            {
                var room = new Room()
                               {
                                   Adults = ivcRoom.Adults, 
                                   Children = ivcRoom.Children, 
                                   Infants = ivcRoom.Infants, 
                                   GuestIDs = ivcRoom.GuestDetails.Select(gd => gd.GuestID).ToList(), 
                                   MealBasisId = ivcRoom.MealBasisID, 
                                   RoomType = ivcRoom.RoomType, 
                                   Source = ivcProperty.Source
                               };
                hotel.SubComponents.Add(room);
                this.AddGuestIfUnique(ivcRoom.GuestDetails);
            }

            return hotel;
        }

        /// <summary>
        /// Builds the lead guest.
        /// </summary>
        /// <param name="ivcLeadGuest">The connect lead guest.</param>
        private void BuildLeadGuest(ivci.Support.LeadCustomerDetails ivcLeadGuest)
        {
            this.booking.LeadGuest = new LeadGuestDetails()
                                         {
                                             AddressLine1 = ivcLeadGuest.CustomerAddress1, 
                                             AddressLine2 = ivcLeadGuest.CustomerAddress2, 
                                             BookingCountryID = ivcLeadGuest.CustomerBookingCountryID, 
                                             DateOfBirth = ivcLeadGuest.DateOfBirth, 
                                             Email = ivcLeadGuest.CustomerEmail, 
                                             FirstName = ivcLeadGuest.CustomerFirstName, 
                                             LastName = ivcLeadGuest.CustomerLastName, 
                                             Phone = ivcLeadGuest.CustomerPhone, 
                                             Postcode = ivcLeadGuest.CustomerPostcode, 
                                             Title = ivcLeadGuest.CustomerTitle, 
                                             TownCity = ivcLeadGuest.CustomerTownCity, 
                                         };
        }

        /// <summary>
        /// Builds the transfer from an connect transfer.
        /// </summary>
        /// <param name="ivcTransfer">The connect transfer.</param>
        /// <returns>a basket transfer</returns>
        private Transfer BuildTransfer(ivci.GetBookingDetailsResponse.Transfer ivcTransfer)
        {
            var transfer = new Transfer()
                               {
                                   Price = ivcTransfer.Price, 
                                   ArrivalDate = ivcTransfer.DepartureDate, 
                                   ArrivalParentId = ivcTransfer.ArrivalParentID, 
                                   ArrivalParentName = ivcTransfer.ArrivalParentName, 
                                   ArrivalParentType = ivcTransfer.ArrivalParentType, 
                                   DepartureParentId = ivcTransfer.DepartureParentID, 
                                   DepartureParentName = ivcTransfer.DepartureParentName, 
                                   DepartureParentType = ivcTransfer.DepartureParentType, 
                                   Vehicle = ivcTransfer.Vehicle, 
                                   VehicleQuantity = ivcTransfer.VehicleQuantity, 
                                   GuestIDs = ivcTransfer.GuestDetails.Select(gd => gd.GuestID).ToList(), 
                                   Adults = ivcTransfer.Adults, 
                                   Children = ivcTransfer.Children, 
                                   Infants = ivcTransfer.Infants, 
                                   OneWay = ivcTransfer.OneWay, 
                                   TotalPrice = ivcTransfer.Price, 
                                   ComponentBooked = true, 
                                   OutboundJourneyDetails = new TransferJourneyDetails() { Date = ivcTransfer.DepartureDate, Time = ivcTransfer.DepartureTime, FlightCode = ivcTransfer.DepartureFlightCode }, 
                                   ReturnJourneyDetails = new TransferJourneyDetails() { Date = ivcTransfer.ReturnDate, Time = ivcTransfer.ReturnTime, FlightCode = ivcTransfer.ReturnFlightCode }
                               };

            this.AddGuestIfUnique(ivcTransfer.GuestDetails);

            return transfer;
        }

        /// <summary>
        /// Builds the extra.
        /// </summary>
        /// <param name="ivcExtra">The ivc extra.</param>
        /// <returns>The extra component.</returns>
        private Extra BuildExtra(ivci.GetBookingDetailsResponse.Extra ivcExtra)
        {
            var extra = new Extra
                            {
                                ExtraId = ivcExtra.ExtraID,
                                ExtraName = ivcExtra.ExtraName,
                                ExtraType = ivcExtra.ExtraType,
                                Price = ivcExtra.TotalPrice,
                                ArrivalDate = ivcExtra.StartDate,
                                Duration = (ivcExtra.Expiry - ivcExtra.StartDate).Days,
                                Adults = ivcExtra.Adults,
                                Children = ivcExtra.Children,
                                Infants = ivcExtra.Infants,
                                GuestIDs = ivcExtra.GuestDetails.Select(gd => gd.GuestID).ToList(),
                            };
            return extra;
        }
    }
}