namespace Web.Template.Application.Book.Adaptors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Interfaces.Book;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Prebook;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.User;
    using Web.Template.Application.IVectorConnect.Requests;
    using Web.Template.Application.Prebook.Models;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class that builds Property search requests
    /// </summary>
    /// <seealso cref="IBasketBookService" />
    public class ConnectBasketBookService : IBasketBookService
    {
        /// <summary>
        /// The prebook request adaptor factory
        /// </summary>
        private readonly IBookRequestAdaptorFactory bookRequestAdaptorFactory;

        /// <summary>
        /// The prebook response processor
        /// </summary>
        private readonly IBookResponseProcessor bookResponseProcessor;

        /// <summary>
        /// The prebook return factory
        /// </summary>
        private readonly IBookReturnBuilder bookReturnBuilder;

        /// <summary>
        /// The connect login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory connectLoginDetailsFactory;

        /// <summary>
        /// The i vector connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory connectRequestFactory;

        /// <summary>
        /// The user service
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectBasketBookService"/> class.
        /// </summary>
        /// <param name="connectRequestFactory">The connect request factory.</param>
        /// <param name="connectLoginDetailsFactory">The connect login details factory.</param>
        /// <param name="bookRequestAdaptorFactory">The book request adaptor factory.</param>
        /// <param name="bookReturnBuilder">The book return builder.</param>
        /// <param name="bookResponseProcessor">The book response processor.</param>
        /// <param name="userService">The user service.</param>
        public ConnectBasketBookService(
            IIVectorConnectRequestFactory connectRequestFactory,
            IConnectLoginDetailsFactory connectLoginDetailsFactory,
            IBookRequestAdaptorFactory bookRequestAdaptorFactory,
            IBookReturnBuilder bookReturnBuilder,
            IBookResponseProcessor bookResponseProcessor,
            IUserService userService)
        {
            this.connectRequestFactory = connectRequestFactory;
            this.connectLoginDetailsFactory = connectLoginDetailsFactory;
            this.bookRequestAdaptorFactory = bookRequestAdaptorFactory;
            this.bookReturnBuilder = bookReturnBuilder;
            this.bookResponseProcessor = bookResponseProcessor;
            this.userService = userService;
        }

        /// <summary>
        /// Creates a search connect search request using a WebTemplate search model.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <returns>
        /// A Connect Property search request
        /// </returns>
        public IBookReturn Book(IBasket basket)
        {
            this.bookReturnBuilder.AddWarnings(this.ValidateBasket(basket));

            try
            {
                if (this.bookReturnBuilder.CurrentlySuccessful)
                {
                    BookRequest connectRequestBody = this.BuildBookRequest(basket);

                    if (this.bookReturnBuilder.CurrentlySuccessful)
                    {
                        Intuitive.FileFunctions.AddLogEntry("Booking", "BookRequest", Intuitive.Serializer.Serialize(connectRequestBody).InnerXml);
                        var bookResponse = this.GetResponse(connectRequestBody);
                        Intuitive.FileFunctions.AddLogEntry("Booking", "BookResponse", Intuitive.Serializer.Serialize(bookResponse).InnerXml);
                        this.ProcessResponse(bookResponse, basket);
                    }
                }
            }
            catch (Exception exception)
            {
                this.bookReturnBuilder.SetToFailure();
                this.bookReturnBuilder.AddWarning(exception.ToString());
            }

            IBookReturn bookReturn = this.bookReturnBuilder.Build();

            return bookReturn;
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="connectRequestBody">The connect request body.</param>
        /// <returns>A prebook response</returns>
        public BookResponse GetResponse(BookRequest connectRequestBody)
        {
            var connectRequest = this.connectRequestFactory.Create(connectRequestBody, HttpContext.Current);
            var bookResponse = connectRequest.Go<BookResponse>(true);

            return bookResponse;
        }

        /// <summary>
        /// Prebook the specified basket.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="componentToken">The component token.</param>
        /// <returns>A prebook return</returns>
        public IPrebookReturn Prebook(IBasket basket, int componentToken = 0)
        {
            return new PrebookReturn();
        }

        /// <summary>
        /// Setups the guest details on request.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="connectRequestBody">The connect request body.</param>
        private static void SetupGuestDetailsOnRequest(IBasket basket, BookRequest connectRequestBody)
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

        /// <summary>
        /// Setups the lead guest.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="connectRequestBody">The connect request body.</param>
        /// <param name="user">The user.</param>
        private static void SetupLeadGuest(IBasket basket, BookRequest connectRequestBody, IUserSession user)
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
                CustomerPostcode = basket.LeadGuest.Postcode,
                CustomerPhone = basket.LeadGuest.Phone
            };

            connectRequestBody.LeadCustomer = leadGuest;
        }

        /// <summary>
        /// Builds the prebook request.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <returns>
        /// A prebook request
        /// </returns>
        private BookRequest BuildBookRequest(IBasket basket)
        {
            BookRequest connectRequestBody = this.SetupConnectRequest();

            var user = this.userService.GetUser(HttpContext.Current);

            basket.SetupGuestIDs();
            SetupLeadGuest(basket, connectRequestBody, user);
            SetupGuestDetailsOnRequest(basket, connectRequestBody);


            if (!string.IsNullOrEmpty(basket.BookingReference))
            {
                connectRequestBody.BookingReference = basket.BookingReference;
            }

            connectRequestBody.ExternalReference = basket.ExternalReference;
            connectRequestBody.TradeContactID = user.TradeSession != null ? user.TradeSession.TradeContactId : 0;
            connectRequestBody.TradeReference = basket.TradeReference ?? string.Empty;
            connectRequestBody.PromotionalCode = basket.PromoCode;

            IEnumerable<IBasketComponent> basketComponents = basket?.Components.Where(c => !c.ComponentBooked);
            foreach (IBasketComponent component in basketComponents)
            {
                var requestAdaptor = this.bookRequestAdaptorFactory.CreateAdaptorByComponentType(component.ComponentType);
                requestAdaptor.Create(component, connectRequestBody);
            }

            if (user.IncludePaymentDetails)
            {
                connectRequestBody.Payment = this.SetupPaymentDetails(basket);
            }

            return connectRequestBody;
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="bookResponse">The book response.</param>
        /// <param name="basket">The basket.</param>
        /// <param name="basketComponent">The basket component.</param>
        private void ProcessResponse(BookResponse bookResponse, IBasket basket, IBasketComponent basketComponent = null)
        {
            this.bookReturnBuilder.AddResponse(bookResponse);
            this.bookReturnBuilder.AddWarnings(bookResponse.ReturnStatus.Exceptions);

            if (!bookResponse.ReturnStatus.Success)
            {
                this.bookReturnBuilder.SetToFailure();
            }

            if (this.bookReturnBuilder.CurrentlySuccessful)
            {
                this.bookResponseProcessor.Process(bookResponse, basket);
            }

            this.bookReturnBuilder.SetBasket(basket);
        }

        /// <summary>
        /// Setups the connect request.
        /// </summary>
        /// <returns>
        /// A bare bones connect request
        /// </returns>
        private BookRequest SetupConnectRequest()
        {
            var connectRequest = new BookRequest() { LoginDetails = this.connectLoginDetailsFactory.Create(HttpContext.Current), };
            return connectRequest;
        }

        /// <summary>
        /// Setups the payment details.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <returns>
        /// payment details
        /// </returns>
        private ivci.Support.PaymentDetails SetupPaymentDetails(IBasket basket)
        {
            var paymentDetails = new ivci.Support.PaymentDetails()
            {
                Amount = basket.PaymentDetails.Amount,
                CCCardHoldersName = basket.PaymentDetails.CardHoldersName,
                CCCardNumber = basket.PaymentDetails.CardNumber,
                CCCardTypeID = basket.PaymentDetails.CardTypeID,
                CCExpireMonth = basket.PaymentDetails.ExpiryMonth,
                CCExpireYear = basket.PaymentDetails.ExpiryYear,
                CCIssueNumber = basket.PaymentDetails.IssueNumber,
                CCSecurityCode = basket.PaymentDetails.SecurityNumber,
                CCStartMonth = basket.PaymentDetails.StartMonth,
                CCStartYear = basket.PaymentDetails.StartYear,
                PaymentToken = basket.PaymentDetails.PaymentToken,
                PaymentType = basket.PaymentDetails.PaymentType.ToString(),
                Surcharge = basket.PaymentDetails.Surcharge,
                ThreeDSecureCode = basket.PaymentDetails.ThreeDSecureCode,
                TotalAmount = basket.PaymentDetails.TotalAmount,
                TransactionID = basket.PaymentDetails.TransactionID
            };
            return paymentDetails;
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