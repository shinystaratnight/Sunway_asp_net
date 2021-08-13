namespace Web.Template.Application.Quote.Services
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Quote;
    using Web.Template.Application.Interfaces.Quote.Adaptors;
    using Web.Template.Application.Interfaces.Quote.Builders;
    using Web.Template.Application.Interfaces.Quote.Models;
    using Web.Template.Application.Interfaces.Quote.Processors;
    using Web.Template.Application.Interfaces.Quote.Services;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.User;
    using Web.Template.Application.IVectorConnect.Requests;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class ConnectBasketQuoteCreateService.
    /// </summary>
    public class ConnectBasketQuoteCreateService : IBasketQuoteCreateService
    {
        /// <summary>
        /// The connect login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory connectLoginDetailsFactory;

        /// <summary>
        /// The connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory connectRequestFactory;

        /// <summary>
        /// The quote create request adaptor factory
        /// </summary>
        private readonly IQuoteCreateRequestAdaptorFactory quoteCreateRequestAdaptorFactory;

        /// <summary>
        /// The quote create response processor
        /// </summary>
        private readonly IQuoteCreateResponseProcessor quoteCreateResponseProcessor;

        /// <summary>
        /// The quote create return builder
        /// </summary>
        private readonly IQuoteCreateReturnBuilder quoteCreateReturnBuilder;

        /// <summary>
        /// The user service
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectBasketQuoteCreateService" /> class.
        /// </summary>
        /// <param name="quoteCreateReturnBuilder">The quote create return builder.</param>
        /// <param name="connectLoginDetailsFactory">The connect login details factory.</param>
        /// <param name="quoteCreateRequestAdaptorFactory">The quote create request adaptor factory.</param>
        /// <param name="connectRequestFactory">The connect request factory.</param>
        /// <param name="quoteCreateResponseProcessor">The quote create response processor.</param>
        /// <param name="userService">The user service.</param>
        public ConnectBasketQuoteCreateService(
            IQuoteCreateReturnBuilder quoteCreateReturnBuilder,
            IConnectLoginDetailsFactory connectLoginDetailsFactory,
            IQuoteCreateRequestAdaptorFactory quoteCreateRequestAdaptorFactory,
            IIVectorConnectRequestFactory connectRequestFactory,
            IQuoteCreateResponseProcessor quoteCreateResponseProcessor,
            IUserService userService)
        {
            this.quoteCreateReturnBuilder = quoteCreateReturnBuilder;
            this.connectLoginDetailsFactory = connectLoginDetailsFactory;
            this.quoteCreateRequestAdaptorFactory = quoteCreateRequestAdaptorFactory;
            this.connectRequestFactory = connectRequestFactory;
            this.quoteCreateResponseProcessor = quoteCreateResponseProcessor;
            this.userService = userService;
        }

        public IQuoteCreateReturn Create(IBasket basket)
        {
            this.quoteCreateReturnBuilder.AddWarnings(this.ValidateBasket(basket));

            try
            {
                if (this.quoteCreateReturnBuilder.CurrentlySuccessful)
                {
                    QuoteRequest connectRequestBody = this.BuildQuoteRequest(basket);

                    if (this.quoteCreateReturnBuilder.CurrentlySuccessful)
                    {
                        Intuitive.FileFunctions.AddLogEntry("Quote", "CreateRequest", Intuitive.Serializer.Serialize(connectRequestBody).InnerXml);
                        QuoteResponse quoteResponse = this.GetResponse(connectRequestBody);
                        Intuitive.FileFunctions.AddLogEntry("Quote", "CreateResponse", Intuitive.Serializer.Serialize(quoteResponse).InnerXml);
                        this.ProcessResponse(quoteResponse, basket);
                    }
                }
            }
            catch (Exception exception)
            {
                this.quoteCreateReturnBuilder.SetToFailure();
                this.quoteCreateReturnBuilder.AddWarning(exception.ToString());
            }

            IQuoteCreateReturn quoteCreateReturn = this.quoteCreateReturnBuilder.Build();

            return quoteCreateReturn;
        }

        /// <summary>
        /// Builds the quote request.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <returns>iVectorConnectInterface.Basket.QuoteRequest.</returns>
        private QuoteRequest BuildQuoteRequest(IBasket basket)
        {
            QuoteRequest connectRequestBody = this.SetupConnectRequest();

            basket.SetupGuestIDs();
            this.SetupGuestNames(basket);
            this.SetupGuestDetailsOnRequest(basket, connectRequestBody);

            var user = this.userService.GetUser(HttpContext.Current);
            this.SetupLeadGuest(basket, connectRequestBody, user);

            if (user.TradeSession.TradeContact != null)
            {
                connectRequestBody.TradeContactID = user.TradeSession.TradeContact.Id;
            }
            connectRequestBody.TradeReference = basket.TradeReference ?? string.Empty;
            connectRequestBody.PromotionalCode = basket.PromoCode;

            foreach (IBasketComponent basketComponent in basket.Components)
            {
                var requestAdaptor =
                    this.quoteCreateRequestAdaptorFactory.CreateAdaptorByComponentType(basketComponent.ComponentType);
                requestAdaptor.Create(basketComponent, connectRequestBody);
            }

            return connectRequestBody;
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="connectRequestBody">The connect request body.</param>
        /// <returns>The quote response.</returns>
        private QuoteResponse GetResponse(QuoteRequest connectRequestBody)
        {
            var connectRequest = this.connectRequestFactory.Create(connectRequestBody, HttpContext.Current);
            var quoteResponse = connectRequest.Go<QuoteResponse>(true);
            return quoteResponse;
        }

        /// <summary>
        /// Processes the response.
        /// </summary>
        /// <param name="quoteResponse">The quote response.</param>
        /// <param name="basket">The basket.</param>
        private void ProcessResponse(QuoteResponse quoteResponse, IBasket basket)
        {
            this.quoteCreateReturnBuilder.AddWarnings(quoteResponse.ReturnStatus.Exceptions);

            if (!quoteResponse.ReturnStatus.Success)
            {
                this.quoteCreateReturnBuilder.SetToFailure();
            }

            if (this.quoteCreateReturnBuilder.CurrentlySuccessful)
            {
                this.quoteCreateResponseProcessor.Process(quoteResponse, basket);
            }

            this.quoteCreateReturnBuilder.SetBasket(basket);
        }

        /// <summary>
        /// Setups the connect request.
        /// </summary>
        /// <returns>The quote request</returns>
        private QuoteRequest SetupConnectRequest()
        {
            var connectRequest = new QuoteRequest() { LoginDetails = this.connectLoginDetailsFactory.Create(HttpContext.Current) };
            return connectRequest;
        }

        /// <summary>
        /// Setups the guest details on request.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="connectRequestBody">The connect request body.</param>
        private void SetupGuestDetailsOnRequest(IBasket basket, QuoteRequest connectRequestBody)
        {
            connectRequestBody.GuestDetails = new List<ivci.Support.GuestDetail>();
            foreach (BasketRoom basketRoom in basket.Rooms)
            {
                foreach (IGuest guest in basketRoom.Guests)
                {
                    var guestdetail = new ivci.Support.GuestDetail(guest.Type, guest.Title, guest.FirstName, guest.LastName, guest.Age, guest.DateOfBirth, guest.NationalityId);
                    guestdetail.GuestID = guest.GuestID;
                    connectRequestBody.GuestDetails.Add(guestdetail);
                }
            }
        }

        private void SetupGuestNames(IBasket basket)
        {
            int passengerCount = 0;
            foreach (BasketRoom basketRoom in basket.Rooms)
            {
                foreach (IGuest guest in basketRoom.Guests)
                {
                    guest.Title = string.IsNullOrEmpty(guest.Title) ? "Mr" : guest.Title;
                    guest.FirstName = string.IsNullOrEmpty(guest.FirstName) ? this.GetTBAFirstName(passengerCount) : guest.FirstName;
                    guest.LastName = string.IsNullOrEmpty(guest.LastName) ? "TBA" : guest.LastName;
                    passengerCount++;
                }
            }
        }

        private string GetTBAFirstName(int passengerCount)
        {
            char[] characterArray = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            int letterPosition = passengerCount % 26;
            int numberOfLetters = 2 + (passengerCount / 26);
            char letter = characterArray[letterPosition];

            var name = new System.Text.StringBuilder();
            for (var i = 0; i < numberOfLetters; i++)
            {
                name.Append(letter);
            }

            return name.ToString();
        }

        /// <summary>
        /// Setups the lead guest.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="connectRequestBody">The connect request body.</param>
        /// <param name="user">The user.</param>
        private void SetupLeadGuest(IBasket basket, QuoteRequest connectRequestBody, IUserSession user)
        {
            if (user.TradeSession?.Trade != null && !user.OverBranded)
            {
                basket.LeadGuest.AddressLine1 = user.TradeSession.Trade.Address1;
                basket.LeadGuest.AddressLine2 = user.TradeSession.Trade.Address2;
                basket.LeadGuest.TownCity = user.TradeSession.Trade.TownCity;
                basket.LeadGuest.Postcode = user.TradeSession.Trade.PostCode;
                basket.LeadGuest.BookingCountryID = user.TradeSession.Trade.BookingCountryId;
                basket.LeadGuest.Phone = user.TradeSession.Trade.Telephone;

                if (!String.IsNullOrEmpty(user.TradeSession.TradeContact?.Email))
                {
                    basket.LeadGuest.Email = user.TradeSession.TradeContact.Email;
                }
                else
                {
                    basket.LeadGuest.Email = user.TradeSession.Trade.Email;
                }

                if (basket.Rooms.Count > 0 && basket.Rooms[0].Guests.Count > 0)
                {
                    basket.LeadGuest.FirstName = basket.Rooms[0].Guests[0].FirstName;
                    basket.LeadGuest.LastName = basket.Rooms[0].Guests[0].LastName;
                    basket.LeadGuest.Title = basket.Rooms[0].Guests[0].Title;
                }
            }

            var leadGuest = new ivci.Support.LeadCustomerDetails()
            {
                ContactCustomer = false,
                CustomerAddress1 = basket.LeadGuest.AddressLine1,
                CustomerAddress2 = basket.LeadGuest.AddressLine2,
                CustomerBookingCountryID = basket.LeadGuest.BookingCountryID,
                CustomerCounty = basket.LeadGuest.BookingCountry,
                CustomerEmail = basket.LeadGuest.Email,
                CustomerTitle = basket.LeadGuest.Title,
                CustomerFirstName = basket.LeadGuest.FirstName,
                CustomerLastName = basket.LeadGuest.LastName,
                CustomerFax = string.Empty,
                CustomerMobile = basket.LeadGuest.Phone,
                CustomerTownCity = basket.LeadGuest.TownCity,
                CustomerPostcode = basket.LeadGuest.Postcode
            };

            connectRequestBody.LeadCustomer = leadGuest;
        }

        /// <summary>
        /// Validates the basket.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <returns>A list of warnings</returns>
        private List<string> ValidateBasket(IBasket basket)
        {
            var warnings = new List<string>();

            if (basket == null)
            {
                warnings.Add("You can not Book a basket that does not exist.");
            }

            if (basket?.Components == null || basket?.Components?.Count == 0)
            {
                warnings.Add("You need atleast one component in your basket to Book");
            }

            return warnings;
        }
    }
}
