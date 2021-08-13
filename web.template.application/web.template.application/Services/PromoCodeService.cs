namespace Web.Template.Application.Services
{
    using System;
    using System.Linq;
    using System.Web;

    using Intuitive;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Basket.Models.Components.SubComponent;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Logging;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Promocode;
    using Web.Template.Application.IVectorConnect.Requests;
    using Web.Template.Application.PromoCode;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// The promotional code service responsible for the management of promotional codes.
    /// </summary>
    /// <seealso cref="IPromoCodeService" />
    public class PromoCodeService : IPromoCodeService
    {
        /// <summary>
        /// The i vector connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory connectRequestFactory;

        /// <summary>
        /// The booking component repository
        /// </summary>
        private IBookingComponentRepository bookingComponentRepository;

        /// <summary>
        /// The login details factory
        /// </summary>
        private IConnectLoginDetailsFactory loginDetailsFactory;

        /// <summary>
        /// The log writer
        /// </summary>
        private ILogWriter logWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PromoCodeService"/> class.
        /// </summary>
        /// <param name="loginDetailsFactory">The login details factory.</param>
        /// <param name="logWriter">The log writer.</param>
        /// <param name="bookingComponentRepository">The booking component repository.</param>
        /// <param name="connectRequestFactory">The connect request factory.</param>
        public PromoCodeService(IConnectLoginDetailsFactory loginDetailsFactory, ILogWriter logWriter, IBookingComponentRepository bookingComponentRepository, IIVectorConnectRequestFactory connectRequestFactory)
        {
            this.loginDetailsFactory = loginDetailsFactory;
            this.logWriter = logWriter;
            this.bookingComponentRepository = bookingComponentRepository;
            this.connectRequestFactory = connectRequestFactory;
        }

        /// <summary>
        /// Applies the promotional code.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="promocode">The promotional code.</param>
        /// <returns>
        /// a promotional code return
        /// </returns>
        public IPromoCodeReturn ApplyPromocode(IBasket basket, string promocode)
        {
            var promoCodeReturn = new PromoCodeReturn() { Basket = basket };

            try
            {
                var hotel = basket.Components.FirstOrDefault(c => c.ComponentType == ComponentType.Hotel);
                var geolevel3Id = 0;
                if (hotel != null)
                {
                    geolevel3Id = ((Hotel)hotel).GeographyLevel3Id;
                }

                var applyPromoCodeRequest = new ivci.ApplyPromoCodeRequest()
                                                {
                                                    LoginDetails = this.loginDetailsFactory.Create(HttpContext.Current), 
                                                    PromoCode = promocode, 
                                                    TotalPrice = basket.TotalPrice + basket.PromoCodeDiscount, 
                                                    GeographyLevel3ID = geolevel3Id
                                                };

                if (basket.SearchDetails != null)
                {
                    applyPromoCodeRequest.UseDate = basket.SearchDetails.DepartureDate;
                    applyPromoCodeRequest.Duration = basket.SearchDetails.Duration;
                }

                if (basket.Rooms != null)
                {
                    applyPromoCodeRequest.Adults = basket.Rooms.SelectMany(r => r.Guests.Where(g => g.Type.ToLower() == "adult")).Count();
                    applyPromoCodeRequest.Children = basket.Rooms.SelectMany(r => r.Guests.Where(g => g.Type.ToLower() == "child")).Count();
                }

                foreach (IBasketComponent basketComponent in basket.Components)
                {
                    applyPromoCodeRequest.Components.Add(this.BuildPromocodeComponent(basketComponent));
                }

                if (applyPromoCodeRequest.Components.Count == 0)
                {
                    basket.PromoCode = string.Empty;
                    basket.PromoCodeDiscount = 0;
                }

                var connectRequest = this.connectRequestFactory.Create(applyPromoCodeRequest, HttpContext.Current);
                var promoCodeResponse = connectRequest.Go<ivci.ApplyPromoCodeResponse>();

                if (promoCodeResponse.ReturnStatus.Success)
                {
                    if (promoCodeResponse.Discount != 0)
                    {
                        promoCodeReturn.Success = true;
                        promoCodeReturn.Discount = promoCodeResponse.Discount;

                        basket.PromoCode = promocode;
                        basket.PromoCodeDiscount = promoCodeResponse.Discount;
                    }
                    else
                    {
                        promoCodeReturn.Success = false;
                        promoCodeReturn.Warnings.Add(promoCodeResponse.Warning);

                        if (basket.PromoCode == promocode)
                        {
                            basket.PromoCode = string.Empty;
                            basket.PromoCodeDiscount = 0;
                        }
                    }
                }
                else
                {
                    promoCodeReturn.Success = false;
                    promoCodeReturn.Warnings.AddRange(promoCodeResponse.ReturnStatus.Exceptions);
                }
            }
            catch (Exception ex)
            {
                promoCodeReturn.Success = false;
                promoCodeReturn.Warnings.Add(ex.ToString());
                this.logWriter.Write("iVectorConnect/Promocode", "PromocodeException", ex.ToString());
            }

            return promoCodeReturn;
        }

        /// <summary>
        /// Removes the promotional code.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <returns>
        /// The basket
        /// </returns>
        public IBasket RemovePromocode(IBasket basket)
        {
            basket.PromoCode = string.Empty;
            basket.PromoCodeDiscount = 0;

            return basket;
        }

        /// <summary>
        /// Builds the component from flight.
        /// </summary>
        /// <param name="basketFlight">The basket flight.</param>
        /// <returns>A component</returns>
        private ivci.ApplyPromoCodeRequest.Component BuildComponentFromFlight(Flight basketFlight)
        {
            var flightComponentId = 0;
            BookingComponent flightComponent = this.bookingComponentRepository.FindBy(bc => bc.Name == "Flight").FirstOrDefault();
            if (flightComponent != null)
            {
                flightComponentId = flightComponent.Id;
            }

            var promocodeComponentFlight = new ivci.ApplyPromoCodeRequest.Component()
                                               {
                                                   BookingComponentID = flightComponentId, 
                                                   BookingComponent = "Flight", 
                                                   Price = basketFlight.TotalPrice, 
                                                   DepartureAirportID = basketFlight.DepartureAirportId, 
                                                   ArrivalAirportID = basketFlight.ArrivalAirportId, 
                                                   FlightCarrierID = basketFlight.FlightCarrierId,
                                                   FlightSupplierID = basketFlight.FlightSupplierId,
                                                   OutboundFlightCode = basketFlight.OutboundFlightDetails.FlightCode,
                                                   ReturnFlightCode = basketFlight.ReturnFlightDetails.FlightCode,
                                                   SupplierID = basketFlight.SupplierId, 
                                                   TotalPassengers = basketFlight.Adults + basketFlight.Children, 
                                                   Adults = basketFlight.Adults
                                               };

            return promocodeComponentFlight;
        }

        /// <summary>
        /// Builds the component from hotel.
        /// </summary>
        /// <param name="basketHotel">The basket hotel.</param>
        /// <returns>a connect component</returns>
        private ivci.ApplyPromoCodeRequest.Component BuildComponentFromHotel(Hotel basketHotel)
        {
            var basketHotelRoom = (Room)basketHotel.SubComponents[0];

            var propertyComponentId = 0;
            BookingComponent firstOrDefault = this.bookingComponentRepository.FindBy(bc => bc.Name == "Property").FirstOrDefault();
            if (firstOrDefault != null)
            {
                propertyComponentId = firstOrDefault.Id;
            }

            int totalAdults = basketHotel.SubComponents.Sum(sc => ((Room)sc).Adults);
            int totalChildren = basketHotel.SubComponents.Sum(sc => ((Room)sc).Children);

            var promocodeComponentHotel = new ivci.ApplyPromoCodeRequest.Component()
                                              {
                                                  BookingComponentID = propertyComponentId, 
                                                  BookingComponent = "Property", 
                                                  PropertyReferenceID = basketHotel.PropertyReferenceId, 
                                                  MealBasisID = basketHotelRoom.MealBasisId, 
                                                  StarRating = basketHotel.Rating.ToSafeInt(), 
                                                  Price = basketHotel.TotalPrice, 
                                                  TotalPassengers = totalAdults + totalChildren, 
                                                  Adults = totalAdults, 
                                                  SupplierID = basketHotelRoom.SupplierId
                                              };

            return promocodeComponentHotel;
        }

        /// <summary>
        /// Builds the component from transfer.
        /// </summary>
        /// <param name="basketTransfer">The basket transfer.</param>
        /// <returns>
        /// a promo code component
        /// </returns>
        private ivci.ApplyPromoCodeRequest.Component BuildComponentFromTransfer(Transfer basketTransfer)
        {
            var transferComponentId = 0;
            BookingComponent transferComponent = this.bookingComponentRepository.FindBy(bc => bc.Name == "Transfer").FirstOrDefault();
            if (transferComponent != null)
            {
                transferComponentId = transferComponent.Id;
            }

            var promocodeComponentTransfer = new ivci.ApplyPromoCodeRequest.Component()
                                                 {
                                                     BookingComponentID = transferComponentId, 
                                                     BookingComponent = "Transfer", 
                                                     TotalPassengers = basketTransfer.Adults + basketTransfer.Children, 
                                                     Adults = basketTransfer.Adults, 
                                                     Price = basketTransfer.TotalPrice, 
                                                 };
            return promocodeComponentTransfer;
        }

        /// <summary>
        /// Builds the promotional code component.
        /// </summary>
        /// <param name="basketComponent">The basket component.</param>
        /// <returns>
        /// a component
        /// </returns>
        private ivci.ApplyPromoCodeRequest.Component BuildPromocodeComponent(IBasketComponent basketComponent)
        {
            ivci.ApplyPromoCodeRequest.Component promocodeComponent;

            switch (basketComponent.ComponentType)
            {
                case ComponentType.Flight:
                    var basketFlight = (Flight)basketComponent;
                    promocodeComponent = this.BuildComponentFromFlight(basketFlight);
                    break;
                case ComponentType.Hotel:
                    var basketHotel = (Hotel)basketComponent;
                    promocodeComponent = this.BuildComponentFromHotel(basketHotel);
                    break;
                case ComponentType.Transfer:
                    var basketTransfer = (Transfer)basketComponent;

                    promocodeComponent = this.BuildComponentFromTransfer(basketTransfer);
                    break;
                default:
                    promocodeComponent = new ivci.ApplyPromoCodeRequest.Component();
                    break;
            }

            return promocodeComponent;
        }
    }
}