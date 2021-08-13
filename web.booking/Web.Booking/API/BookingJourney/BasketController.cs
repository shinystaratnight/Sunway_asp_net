namespace Web.Booking.API.BookingJourney
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    
    using Intuitive;

    using Newtonsoft.Json.Linq;

    using Web.Booking.Models.Application;
    using Web.Template.Application.Basket.BasketModels;
    using Web.Template.Application.Basket.BasketModels.Components;
    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Basket.Models.Components.SubComponent;
    using Web.Template.Application.Book.Models;
    using Web.Template.Application.Interfaces.Book;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Prebook;
    using Web.Template.Application.Interfaces.Promocode;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Prebook.Models;

    /// <summary>
    ///     Controller used to expose to the user interface methods to allow changes to the basket
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class BasketController : ApiController
    {
        /// <summary>
        ///     The basket service
        /// </summary>
        private readonly IBasketService basketService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasketController" /> class.
        /// </summary>
        /// <param name="basketService">The basket service.</param>
        public BasketController(IBasketService basketService)
        {
            this.basketService = basketService;
        }

        /// <summary>
        /// Adds the component.
        /// </summary>
        /// <param name="componentModel">The component model.</param>
        /// <returns>The current basket</returns>
        [Route("api/basket/component/add")]
        [HttpPost]
        public IBasket AddComponent([FromBody] BasketComponentModel componentModel)
        {
            componentModel.SearchToken = componentModel.SearchToken.Replace(" ", "+");
            return this.basketService.AddComponent(componentModel);
        }

        /// <summary>
        /// Updates the component.
        /// </summary>
        /// <param name="updateModel">The update model.</param>
        /// <returns>A call to the basket service to update the flight component</returns>
        [Route("api/basket/component/flight/update")]
        [HttpPost]
        public IBasket UpdateFlightComponent([FromBody] UpdateFlightModel updateModel)
        {
            var basketToken = updateModel.BasketToken;
            var componentToken = updateModel.ComponentToken;

            IBasket currentBasket = this.basketService.GetBasket(basketToken);
            Flight currentComponent
                = (Flight)currentBasket.Components.FirstOrDefault(c => c.ComponentToken == componentToken);

            if (currentComponent != null)
            {
                currentComponent.ComponentPreBooked = false;
                foreach (var subComponent in currentComponent.SubComponents.Where(x => x is FlightExtra))
                {
                    var flightExtra = (FlightExtra)subComponent;
                    foreach (FlightExtra newSubComponent in updateModel.SubComponents)
                    {
                        if (newSubComponent.ComponentToken == subComponent.ComponentToken)
                        {
                            flightExtra.QuantitySelected = newSubComponent.QuantitySelected;
                        }
                    }
                }
            }

            return this.basketService.UpdateBasketComponent(basketToken, currentComponent);
        }

        /// <summary>
        ///     Changes the lead guest.
        /// </summary>
        /// <param name="changeBasketModel">The change basket model.</param>
        /// <returns>A basket</returns>
        [Route("api/basket/promocode/add")]
        [HttpPost]
        public IPromoCodeReturn ApplyPromoCode([FromBody] ChangeBasketModel changeBasketModel)
        {
            return this.basketService.ApplyPromoCode(changeBasketModel.BasketToken, changeBasketModel.PromoCode);
        }

        /// <summary>
        ///     Books the basket.
        /// </summary>
        /// <param name="changeBasketModel">The change basket model.</param>
        /// <returns>A basket</returns>
        [Route("api/basket/book")]
        [HttpPost]
        public IBookReturn BookBasket([FromBody] ChangeBasketModel changeBasketModel)
        {
            IBookReturn bookReturn = new BookReturn() { Warnings = new List<string>() };

            if (changeBasketModel != null)
            {
                if (HttpContext.Current.Session["BookLock"].ToSafeBoolean() == false)
                {
                    HttpContext.Current.Session["BookLock"] = true;
                    bookReturn = this.basketService.BookBasket(changeBasketModel.BasketToken);
                    HttpContext.Current.Session["BookLock"] = false;
                }
                else
                {
                    bookReturn.Success = false;
                    bookReturn.Warnings.Add("Book already in progress.");
                }
            }
            else
            {
                bookReturn.Success = false;
                bookReturn.Warnings.Add("Invalid change basket model");
            }

            return bookReturn;
        }

        /// <summary>
        /// Books the basket.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="basket">The basket.</param>
        /// <returns>The book return</returns>
        [Route("api/basket/book/{basketToken}")]
        [HttpPost]
        public IBookReturn BookBasket(string basketToken, [FromBody] BasketBookModel basket)
        {
            IBookReturn bookReturn = new BookReturn() { Warnings = new List<string>() };
            if (basketToken != string.Empty && basket != null)
            {
                if (HttpContext.Current.Session["BookLock"].ToSafeBoolean() == false)
                {
                    this.basketService.ChangeGuests(basketToken, basket.GuestDetails);
                    this.basketService.ChangeLeadGuest(basketToken, basket.LeadGuest);
                    this.basketService.ChangePayment(basketToken, basket.PaymentDetails);
                    this.basketService.ChangeTradeReference(basketToken, basket.TradeReference);
                    this.basketService.ChangePropertyRequests(basketToken, basket.HotelRequest);

                    HttpContext.Current.Session["BookLock"] = true;
                    bookReturn = this.basketService.BookBasket(basketToken);
                    HttpContext.Current.Session["BookLock"] = false;
                }
                else
                {
                    bookReturn.Success = false;
                    bookReturn.Warnings.Add("Book already in progress.");
                }
            }
            else
            {
                bookReturn.Success = false;
                bookReturn.Warnings.Add("Invalid change basket model");
            }

            return bookReturn;
        }

        /// <summary>
        ///     Changes the guests.
        /// </summary>
        /// <param name="changeBasketModel">The change basket model.</param>
        /// <returns>A basket</returns>
        [Route("api/basket/change/guests")]
        [HttpPost]
        public IBasket ChangeGuests([FromBody] ChangeBasketModel changeBasketModel)
        {
            return this.basketService.ChangeGuests(changeBasketModel.BasketToken, changeBasketModel.GuestDetails);
        }

        /// <summary>
        ///     Changes the lead guest.
        /// </summary>
        /// <param name="changeBasketModel">The change basket model.</param>
        /// <returns>A basket</returns>
        [Route("api/basket/change/leadguest")]
        [HttpPost]
        public IBasket ChangeLeadGuest([FromBody] ChangeBasketModel changeBasketModel)
        {
            return this.basketService.ChangeLeadGuest(changeBasketModel.BasketToken, changeBasketModel.LeadGuest);
        }

        /// <summary>
        ///     Changes the multiple.
        /// </summary>
        /// <param name="changeBasketModel">The change basket model.</param>
        /// <returns>A basket</returns>
        [Route("api/basket/change")]
        [HttpPost]
        public IBasket ChangeMultiple([FromBody] ChangeBasketModel changeBasketModel)
        {
            this.basketService.ChangeLeadGuest(changeBasketModel.BasketToken, changeBasketModel.LeadGuest);
            this.basketService.ChangePayment(changeBasketModel.BasketToken, changeBasketModel.PaymentDetails);
            return this.basketService.ChangeGuests(changeBasketModel.BasketToken, changeBasketModel.GuestDetails);
        }

        /// <summary>
        ///     Changes the payment.
        /// </summary>
        /// <param name="changeBasketModel">The change basket model.</param>
        /// <returns>A basket</returns>
        [Route("api/basket/change/payment")]
        [HttpPost]
        public IBasket ChangePayment([FromBody] ChangeBasketModel changeBasketModel)
        {
            return this.basketService.ChangePayment(changeBasketModel.BasketToken, changeBasketModel.PaymentDetails);
        }

        /// <summary>
        ///     Components the pre book.
        /// </summary>
        /// <param name="componentModel">The component model.</param>
        /// <returns>A basket</returns>
        [Route("api/basket/component/prebook")]
        [HttpPost]
        public IPrebookReturn ComponentPreBook([FromBody] BasketComponentModel componentModel)
        {
            IPrebookReturn prebookReturn = new PrebookReturn() { Warnings = new List<string>() };
            if (componentModel != null)
            {
                prebookReturn = this.basketService.PreBookComponent(componentModel.BasketToken, componentModel.ComponentToken);
            }
            else
            {
                prebookReturn.Success = false;
                prebookReturn.Warnings.Add("Invalid component model");
            }

            return prebookReturn;
        }

		/// <summary>
		///     Components the pre book.
		/// </summary>
		/// <param name="componentModels">The component model.</param>
		/// <returns>A basket</returns>
		[Route("api/basket/component/charges")]
		[HttpPost]
		public IComponentPaymentCancellationReturn GetCancellationCharges([FromBody] List<BasketComponentModel> componentModels)
		{
			IComponentPaymentCancellationReturn cancellationReturn = new ComponentPaymentCancellationReturn() { Warnings = new List<string>() };
			if (componentModels != null)
			{
                cancellationReturn = this.basketService.GetCancellationCharges(componentModels);
			}
			else
			{
                cancellationReturn.Warnings.Add("Invalid component model");
			}

			return cancellationReturn;
		}

		/// <summary>
		/// Creates the basket.
		/// </summary>
		/// <returns>The basket token.</returns>
		[Route("api/basket/create")]
        [HttpGet]
        public Guid CreateBasket()
        {
            Guid guid = this.basketService.CreateBasket();
            return guid;
        }

        /// <summary>
        /// Adds the component.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>
        /// The current basket
        /// </returns>
        [Route("api/basket/{basketToken}")]
        [HttpGet]
        public IBasket GetBasket(string basketToken)
        {
            IBasket basket = this.basketService.GetBasket(basketToken);
            return basket;
        }

        /// <summary>
        /// Pre-books the basket.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>
        /// A basket
        /// </returns>
        [Route("api/basket/prebook/{basketToken}")]
        [HttpGet]
        public IPrebookReturn PrebookBasket(string basketToken)
        {
            return this.basketService.PreBookBasket(basketToken);
        }

        /// <summary>
        /// Releases the flight seat lock.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>The return status</returns>
        [Route("api/basket/releaseflightseatlock/{basketToken}")]
        [HttpGet]
        public string ReleaseFlightSeatLock(string basketToken)
        {
            return this.basketService.ReleaseFlightSeatLock(basketToken);
        }

        /// <summary>
        ///     Removes the component.
        /// </summary>
        /// <param name="componentModel">The component model.</param>
        /// <returns>A basket</returns>
        [Route("api/basket/component/remove")]
        [HttpPost]
        public IBasket RemoveComponent([FromBody] BasketComponentModel componentModel)
        {
            return this.basketService.RemoveComponent(componentModel.BasketToken, componentModel.ComponentToken);
        }

        /// <summary>
        ///     Changes the lead guest.
        /// </summary>
        /// <param name="changeBasketModel">The change basket model.</param>
        /// <returns>A basket</returns>
        [Route("api/basket/promocode/remove")]
        [HttpPost]
        public IBasket RemovePromoCode([FromBody] ChangeBasketModel changeBasketModel)
        {
            return this.basketService.RemovePromoCode(changeBasketModel.BasketToken);
        }

        /// <summary>
        /// Setups the fake basket.
        /// </summary>
        /// <param name="basket">The basket.</param>
        private static void SetupFakeBasket(IBasket basket)
        {
            basket.Rooms = new List<BasketRoom>();
            var roomNumber = 1;

            for (int i = 0; i < 2; i++)
            {
                var basketRoom = new BasketRoom() { Guests = new List<GuestDetail>(), RoomNumber = roomNumber };
                for (int j = 0; i < 3; i++)
                {
                    var guest = new GuestDetail() { Type = "Adult" };
                    basketRoom.Guests.Add(guest);
                }

                for (int j = 0; i < 0; i++)
                {
                    var guest = new GuestDetail() { Type = "Child" };
                    basketRoom.Guests.Add(guest);
                }

                for (int j = 0; i < 0; i++)
                {
                    var guest = new GuestDetail() { Type = "Infant" };
                    basketRoom.Guests.Add(guest);
                }

                basket.Rooms.Add(basketRoom);
                roomNumber += 1;
            }

            basket.Components = new List<IBasketComponent>();

            Flight flight = new Flight()
                                {
                                    CancellationCharges = new List<ICancellationCharge>() { new CancellationCharge() { Amount = new decimal(100.00), EndDate = DateTime.Now.AddDays(200), StartDate = DateTime.Now } }, 
                                    ArrivalDate = DateTime.Now.AddDays(100)
                                };

            basket.Components.Add(flight);

            Hotel hotel = new Hotel()
                              {
                                  TermsAndConditions = "See Brochure", 
                                  TermsAndConditionsUrl = "", 
                                  CancellationCharges = new List<ICancellationCharge>() { new CancellationCharge() { Amount = new decimal(100.00), EndDate = DateTime.Now.AddDays(100), StartDate = DateTime.Now.AddDays(70) } }, 
                                  ArrivalDate = DateTime.Now.AddDays(100)
                              };
            var room = new Room();
            hotel.SubComponents = new List<ISubComponent>();
            hotel.SubComponents.Add(room);

            basket.Components.Add(hotel);

            basket.Errata = new List<IErratum>() { new Erratum() { Description = "this is an erratum description", Subject = "this is an erratum subject" } };

            basket.Payments = new List<IPayment>() { new Payment() { Amount = new decimal(1000), DateDue = DateTime.Now.AddDays(100) } };

            basket.PromoCodeDiscount = new decimal(10);
        }
    }
}