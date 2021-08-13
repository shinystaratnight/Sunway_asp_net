namespace Web.Template.Application.Basket.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using AutoMapper;

    using iVectorConnectInterface.Interfaces;
    using iVectorConnectInterface.Basket;
    using iVectorConnectInterface.Support;

    using Models.Components;
    using Newtonsoft.Json;

    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Book.Models;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Basket;
    using Web.Template.Application.Interfaces.Book;
    using Web.Template.Application.Interfaces.BookingAdjustment;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Payment;
    using Web.Template.Application.Interfaces.Prebook;
    using Web.Template.Application.Interfaces.Promocode;
    using Web.Template.Application.Interfaces.Repositories;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Net.IVectorConnect;
    using Web.Template.Application.Payment.Models;
    using Web.Template.Application.Results.ResultModels;
    using Web.Template.Application.Search.SearchModels;
    using Web.Template.Application.IVectorConnect.Requests;

    using GuestDetail = Web.Template.Application.Basket.Models.GuestDetail;
    using ivci = iVectorConnectInterface;
    using PaymentDetails = Web.Template.Application.Basket.Models.PaymentDetails;

    /// <summary>
    /// Basket service used to manage operations on the basket
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Services.IBasketService" />
    /// <seealso cref="IBasketService" />
    public class BasketService : IBasketService
    {
        /// <summary>
        /// The basket book service
        /// </summary>
        private readonly IBasketBookService basketBookService;

        /// <summary>
        /// The basket prebook adaptor
        /// </summary>
        private readonly IBasketPrebookService basketPrebookService;

        /// <summary>
        ///     The basket repository
        /// </summary>
        private readonly IBasketRepository basketRepository;

        /// <summary>
        /// The booking adjustment service
        /// </summary>
        private readonly IBookingAdjustmentService bookingAdjustmentService;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// The promo code service
        /// </summary>
        private readonly IPromoCodeService promoCodeService;

        /// <summary>
        /// The result service
        /// </summary>
        private readonly IResultService resultService;

        /// <summary>
        /// The three d secure service
        /// </summary>
        private readonly IThreeDSecureService threeDSecureService;

        /// <summary>
        /// The connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory connectRequestFactory;

        /// <summary>
        /// The release flight seat lock factory
        /// </summary>
        private readonly IReleaseFlightSeatLockFactory releaseFlightSeatLockFactory;

        /// <summary>
        /// The retrieve stored basket factory
        /// </summary>
        private readonly IRetrieveStoredBasketFactory retrieveStoredBasketFactory;

        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// The store basket factory
        /// </summary>
        private readonly IStoreBasketFactory storeBasketFactory;

        private readonly IConnectLoginDetailsFactory connectLoginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasketService" /> class.
        /// </summary>
        /// <param name="basketRepository">The basket repository.</param>
        /// <param name="resultService">Server used to retrieve result</param>
        /// <param name="basketPrebookService">Takes basket components and builds a prebook request</param>
        /// <param name="mapper">Mapper used for converting one object to another.</param>
        /// <param name="basketBookService">The basket book service.</param>
        /// <param name="promoCodeService">The promo code service.</param>
        /// <param name="releaseFlightSeatLockFactory">The release flight seat lock factory.</param>
        /// <param name="connectRequestFactory">The connect request factory.</param>
        /// <param name="threeDSecureService">The three d secure service.</param>
        /// <param name="storeBasketFactory">The store basket factory.</param>
        /// <param name="retrieveStoredBasketFactory">The retrieve stored basket factory.</param>
        /// <param name="siteService">The site service.</param>
        /// <param name="bookingAdjustmentService">The booking adjustment service.</param>
        public BasketService(
            IBasketRepository basketRepository,
            IResultService resultService,
            IBasketPrebookService basketPrebookService,
            IMapper mapper,
            IBasketBookService basketBookService,
            IPromoCodeService promoCodeService,
            IReleaseFlightSeatLockFactory releaseFlightSeatLockFactory,
            IIVectorConnectRequestFactory connectRequestFactory,
            IThreeDSecureService threeDSecureService,
            IStoreBasketFactory storeBasketFactory,
            IRetrieveStoredBasketFactory retrieveStoredBasketFactory,
            ISiteService siteService,
            IBookingAdjustmentService bookingAdjustmentService,
            IConnectLoginDetailsFactory connectLoginDetailsFactory)
        {
            this.basketRepository = basketRepository;
            this.resultService = resultService;
            this.basketPrebookService = basketPrebookService;
            this.mapper = mapper;
            this.basketBookService = basketBookService;
            this.promoCodeService = promoCodeService;
            this.releaseFlightSeatLockFactory = releaseFlightSeatLockFactory;
            this.connectRequestFactory = connectRequestFactory;
            this.threeDSecureService = threeDSecureService;
            this.storeBasketFactory = storeBasketFactory;
            this.retrieveStoredBasketFactory = retrieveStoredBasketFactory;
            this.siteService = siteService;
            this.bookingAdjustmentService = bookingAdjustmentService;
            this.connectLoginDetailsFactory = connectLoginDetailsFactory;
        }

        /// <summary>
        /// Adds the basket component.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="basketComponent">The basket component.</param>
        /// <returns>The updated basket.</returns>
        public IBasket AddBasketComponent(string basketToken, IBasketComponent basketComponent)
        {
            IBasket basket = this.basketRepository.RetrieveBasketByToken(basketToken);
            this.RemoveExistingComponent(basket, basketComponent.ComponentType);
            basket.Components.Add(basketComponent);
            this.basketRepository.UpdateBasket(basketToken, basket);
            return basket;
        }

        /// <summary>
        /// Adds the component.
        /// </summary>
        /// <param name="componentModel">The component model.</param>
        /// <returns>
        /// A basket
        /// </returns>
        public IBasket AddComponent(BasketComponentModel componentModel)
        {
            IBasket basket = this.basketRepository.RetrieveBasketByToken(componentModel.BasketToken);

            IResult component = this.resultService.RetrieveResult(componentModel.SearchToken, componentModel.ComponentToken);

            if (component != null)
            {
                IBasketComponent basketComponent = component.CreateBasketComponent();

                basketComponent.BasketToken = componentModel.BasketToken;
                basketComponent.SearchToken = componentModel.SearchToken;

                if (componentModel.SubComponentTokens != null)
                {
                    List<ISubComponent> selectedSubComponent = new List<ISubComponent>();

                    if (basketComponent.SubComponents != null)
                    {
                        foreach (ISubComponent subComponent in basketComponent.SubComponents)
                        {
                            if (componentModel.SubComponentTokens.Contains(subComponent.ComponentToken))
                            {
                                selectedSubComponent.Add(subComponent);
                            }
                        }

                        basketComponent.SubComponents = selectedSubComponent;
                    }
                }

                ISearchModel searchModel = this.resultService.RetrieveSearchModel(componentModel.SearchToken);
                if (searchModel != null)
                {
                    basketComponent.SetupComponentSearchDetails(searchModel);
                }

                if (component.GetType() == typeof(ExtraResult))
                {
                    IExtraSearchModel extraSearchModel = this.resultService.RetrieveExtraSearchModel(componentModel.SearchToken);
                    basketComponent.SetupComponentExtraSearchDetails(extraSearchModel);
                }

                basketComponent.SetupMetaData(componentModel.MetaData);

                this.RemoveExistingComponent(basket, basketComponent.ComponentType);

                basket.Components.Add(basketComponent);

                if (basket.SearchDetails == null)
                {
                    basket.SearchDetails = searchModel;

                    if (basket.Rooms == null)
                    {
                        SetupBasketGuests(basket, searchModel);
                    }
                }
                this.basketRepository.UpdateBasket(componentModel.BasketToken, basket);

                if (component.GetType() == typeof(FlightResult))
                {
                    IPrebookReturn prebookReturn = this.PreBookComponent(componentModel.BasketToken, componentModel.ComponentToken);
                    if (prebookReturn.Basket != null)
                    {
                        basket = prebookReturn.Basket;
                    }
                }
            }

            if (!string.IsNullOrEmpty(componentModel.AdjustmentSearchToken))
            {
                List<IAdjustment> adjustments =
                    this.bookingAdjustmentService.RetrieveResult(componentModel.AdjustmentSearchToken);
                if (adjustments != null && adjustments.Count > 0)
                {
                    basket.Adjustments.AddRange(adjustments);
                }
            }

            return basket;
        }

        /// <summary>
        /// Applies the promo code.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="promoCode">The promotional code to be applied.</param>
        /// <returns>A promotional code return</returns>
        public IPromoCodeReturn ApplyPromoCode(string basketToken, string promoCode)
        {
            var basket = this.basketRepository.RetrieveBasketByToken(basketToken);

            var promocodeReturn = this.promoCodeService.ApplyPromocode(basket, promoCode);

            this.basketRepository.UpdateBasket(basketToken, promocodeReturn.Basket);

            return promocodeReturn;
        }

        /// <summary>
        ///     Books the basket.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>A basket</returns>
        public IBookReturn BookBasket(string basketToken)
        {
            IBasket basket = this.basketRepository.RetrieveBasketByToken(basketToken);

            IBookReturn bookReturn = new BookReturn() { Warnings = new List<string>() };

            if (!basket.AllComponentsPreBooked)
            {
                IPrebookReturn prebookReturn = this.PreBookBasket(basketToken);
                if (!prebookReturn.Success)
                {
                    bookReturn.Success = false;
                    bookReturn.Warnings.AddRange(prebookReturn.Warnings);
                    return bookReturn;
                }
            }
            var site = this.siteService.GetSite(HttpContext.Current);
            if (!basket.PaymentDetails.OffsitePaymentTaken)
            {
                int basketStoreId = 0;
                ivci.GetOffsitePaymentRedirectRequest offsiteRequest;
                IIVectorConnectRequest connectRequest;
                List<string> warnings;

                var paymentMode = site.SiteConfiguration.BookingJourneyConfiguration.PaymentMode;

                switch (paymentMode)
                {
                    case PaymentMode.Standard:
                        if (this.Check3DSecure(basketToken))
                        {
                            bookReturn.ThreeDSecureEnrollment = true;
                            return bookReturn;
                        }
                        break;
                    case PaymentMode.OffsiteIFrame:
                        basketStoreId = this.StoreBasket(basketToken, 0);
                        offsiteRequest = this.GetOffsitePaymentRedirectRequest(basketToken, basket, basketStoreId, paymentMode);

                        connectRequest = this.connectRequestFactory.Create(
                            offsiteRequest,
                            HttpContext.Current);

                        warnings = offsiteRequest.Validate();
                        if (warnings.Count == 0)
                        {
                            ivci.GetOffsitePaymentRedirectResponse redirectResponse =
                                connectRequest.Go<ivci.GetOffsitePaymentRedirectResponse>(true);

                            bookReturn.Success = redirectResponse.ReturnStatus.Success;
                            bookReturn.Warnings = redirectResponse.ReturnStatus.Exceptions;
                            bookReturn.OffsiteRedirect = redirectResponse.ReturnStatus.Success;

                            basket.PaymentDetails.OffsitePaymentHtml = redirectResponse.HTML;
                            this.basketRepository.UpdateBasket(basketToken, basket);
                            this.StoreBasket(basketToken, basketStoreId);

                            return bookReturn;
                        }

                        break;

                    case PaymentMode.OffsiteRedirect:



                        basketStoreId = this.StoreBasket(basketToken, 0);

                        //As We're not setting the payment details when using offsite, the amount is not set unless we have a deposit
                        if (basket.PaymentDetails.Amount == 0)
                        {
                            basket.PaymentDetails.TotalAmount = basket.TotalAmountDue;
                            basket.PaymentDetails.Amount = basket.TotalAmountDue;
                        }

                        offsiteRequest = this.GetOffsitePaymentRedirectRequest(basketToken, basket, basketStoreId, paymentMode);

                        connectRequest = this.connectRequestFactory.Create(
                            offsiteRequest,
                            HttpContext.Current);

                        warnings = offsiteRequest.Validate();
                        if (warnings.Count == 0)
                        {
                            ivci.GetOffsitePaymentRedirectResponse redirectResponse =
                                connectRequest.Go<ivci.GetOffsitePaymentRedirectResponse>(true);

                            bookReturn.Success = redirectResponse.ReturnStatus.Success;
                            bookReturn.Warnings = redirectResponse.ReturnStatus.Exceptions;
                            bookReturn.OffsiteRedirect = redirectResponse.ReturnStatus.Success;

                            basket.PaymentDetails.OffsitePaymentHtml = redirectResponse.HTML;
                            this.basketRepository.UpdateBasket(basketToken, basket);
                            this.StoreBasket(basketToken, basketStoreId);

                            return bookReturn;
                        }

                        break;

                    default:
                        if (this.Check3DSecure(basketToken))
                        {
                            bookReturn.ThreeDSecureEnrollment = true;
                            return bookReturn;
                        }
                        break;
                }
            }
            if (!basket.AllComponentsBooked)
            {
                bookReturn = this.basketBookService.Book(basket);
                if (bookReturn.Basket != null)
                {
                    this.basketRepository.UpdateBasket(basketToken, bookReturn.Basket);
                }
            }

            return bookReturn;
        }

        private ivci.GetOffsitePaymentRedirectRequest GetOffsitePaymentRedirectRequest(
            string basketToken,
            IBasket basket,
            int basketStoreId,
            PaymentMode paymentMode)
        {
            var leadGuest = basket.LeadGuest;

            var leadCustomer = new LeadCustomerDetails()
            {
                CustomerTitle = leadGuest.Title ?? "Mr",
                CustomerFirstName = leadGuest.FirstName ?? "TBA",
                CustomerLastName = leadGuest.LastName ?? "TBA",
                CustomerAddress1 = leadGuest.AddressLine1 ?? "",
                CustomerAddress2 = leadGuest.AddressLine2 ?? "",
                CustomerBookingCountryID = leadGuest.BookingCountryID,
                CustomerEmail = leadGuest.Email ?? "",
                CustomerTownCity = leadGuest.TownCity ?? "",
                CustomerPostcode = leadGuest.Postcode ?? "",
                CustomerPhone = leadGuest.Phone ?? "",
                CustomerCounty = "",
                CustomerMobile = "",
                CustomerFax = "",
                CustomerPassportNumber = ""
            };

            var request = HttpContext.Current.Request;
            string baseUrl = $"{request.Url.Scheme}://{request.Url.Authority}{request.ApplicationPath?.TrimEnd('/')}/";

            string redirectToPage = "confirmation";

            if (paymentMode == PaymentMode.OffsiteRedirect)
            {
                redirectToPage = "offsitepayment/paymentresponsepost";
            }

            var offsiteRequest = new ivci.GetOffsitePaymentRedirectRequest()
            {
                ReturnURL =
                                             $"{baseUrl}{redirectToPage}?t={basketToken}",
                LoginDetails =
                                             this.connectLoginDetailsFactory.Create(
                                                 HttpContext.Current),
                BookingDetails =
                                             new ivci.GetOffsitePaymentRedirectRequest.
                                             BookingDetailsDef()
                                             {
                                                 BasketStoreID = basketStoreId,
                                                 TotalPayment =
                                                         basket.PaymentDetails.Amount,
                                                 TotalPassengers =
                                                         basket.Rooms.Sum(
                                                             b => b.Guests.Count),
                                                 LeadCustomer = leadCustomer,
                                             }
            };
            return offsiteRequest;
        }

        public bool Check3DSecure(string basketToken)
        {
            IBasket basket = this.basketRepository.RetrieveBasketByToken(basketToken);
            var site = this.siteService.GetSite(HttpContext.Current);
            var threeDSecureEnrollment = false;

            if (basket.PaymentDetails.PaymentType == PaymentType.CreditCard && string.IsNullOrEmpty(basket.PaymentDetails.ThreeDSecureCode) && string.IsNullOrEmpty(basket.PaymentDetails.PaymentToken))
            {
                if (site.SiteConfiguration.BookingJourneyConfiguration.ThreeDSecureProvider != ThreeDSecureProvider.None)
                {
                    int basketStoreId = this.StoreBasket(basketToken, 0);

                    var request = HttpContext.Current.Request;
                    string baseUrl = $"{request.Url.Scheme}://{request.Url.Authority}{request.ApplicationPath?.TrimEnd('/')}/";

                    IThreeDSecureRedirectModel threeDSecureRedirectModel = new ThreeDSecureRedirectModel()
                    {
                        BookingReference = "",
                        PaymentDetails = basket.PaymentDetails,
                        RedirectUrl = $"{baseUrl}3dsecure/paymentresponse?basketstoreid={basketStoreId}"
                    };

                    IThreeDSecureRedirectReturn threeDSecureRedirectReturn = this.threeDSecureService.Get3DSecureRedirect(threeDSecureRedirectModel);
                    if (threeDSecureRedirectReturn.Enrollment)
                    {
                        basket.PaymentDetails.ThreeDSecureHtml = threeDSecureRedirectReturn.Html;
                        basket.PaymentDetails.PaymentToken = threeDSecureRedirectReturn.PaymentToken;
                        this.basketRepository.UpdateBasket(basketToken, basket);
                        this.StoreBasket(basketToken, basketStoreId);

                        threeDSecureEnrollment = true;
                    }
                }
            }
            return threeDSecureEnrollment;
        }

        /// <summary>
        ///     Changes the guests.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="guestDetails">The guest details.</param>
        /// <returns>A basket</returns>
        public IBasket ChangeGuests(string basketToken, List<BasketRoom> guestDetails)
        {
            IBasket basket = this.basketRepository.RetrieveBasketByToken(basketToken);

            basket.Rooms = guestDetails;

            this.basketRepository.UpdateBasket(basketToken, basket);

            return basket;
        }

        /// <summary>
        /// Changes the property requests.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="hotelRequest">The hotel request.</param>
        /// <returns>The basket</returns>
        public IBasket ChangePropertyRequests(string basketToken, string hotelRequest)
        {
            IBasket basket = this.basketRepository.RetrieveBasketByToken(basketToken);

            foreach (var Component in basket.Components)
            {
                if (Component.ComponentType == ComponentType.Hotel)
                {
                    ((Hotel)Component).Request = hotelRequest;

                    foreach (Models.Components.SubComponent.Room SubComponent in ((Hotel)Component).SubComponents)
                    {
                        SubComponent.Request = hotelRequest;
                    }
                }
            }

            this.basketRepository.UpdateBasket(basketToken, basket);

            return basket;
        }

        /// <summary>
        ///     Changes the lead guest.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="leadGuest">The lead guest.</param>
        /// <returns>A basket</returns>
        public IBasket ChangeLeadGuest(string basketToken, LeadGuestDetails leadGuest)
        {
            IBasket basket = this.basketRepository.RetrieveBasketByToken(basketToken);

            basket.LeadGuest = leadGuest;

            this.basketRepository.UpdateBasket(basketToken, basket);

            return basket;
        }

        /// <summary>
        ///     Changes the payment.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="paymentDetails">The payment details.</param>
        /// <returns>A basket</returns>
        public IBasket ChangePayment(string basketToken, PaymentDetails paymentDetails)
        {
            IBasket basket = this.basketRepository.RetrieveBasketByToken(basketToken);

            basket.PaymentDetails = paymentDetails;

            this.basketRepository.UpdateBasket(basketToken, basket);

            return basket;
        }

        /// <summary>
        /// Changes the guests.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="tradeReference">The trade reference.</param>
        /// <returns>
        /// A basket
        /// </returns>
        public IBasket ChangeTradeReference(string basketToken, string tradeReference)
        {
            IBasket basket = this.basketRepository.RetrieveBasketByToken(basketToken);

            basket.TradeReference = tradeReference;

            this.basketRepository.UpdateBasket(basketToken, basket);

            return basket;
        }

        /// <summary>
        /// Creates the basket.
        /// </summary>
        /// <returns>The basket token</returns>
        public Guid CreateBasket()
        {
            Guid guid = Guid.NewGuid();
            IBasket basket = new Basket()
            {
                BasketToken = guid,
                Components = new List<IBasketComponent> { },
                AllComponentsBooked = false,
                AllComponentsPreBooked = false,
                BookingReference = string.Empty,
                ExternalReference = string.Empty,
                PaymentDetails = new PaymentDetails()
            };
            this.basketRepository.AddNewBasket(guid.ToString(), basket);
            return guid;
        }

        /// <summary>
        /// Gets the basket.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>
        /// The basket
        /// </returns>
        public IBasket GetBasket(string basketToken)
        {
            IBasket basket = this.basketRepository.RetrieveBasketByToken(basketToken);
            return basket;
        }

        /// <summary>
        ///     Pre-book basket.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>A basket</returns>
        public IPrebookReturn PreBookBasket(string basketToken)
        {
            IBasket basket = this.basketRepository.RetrieveBasketByToken(basketToken);
            IPrebookReturn prebookReturn = null;

            prebookReturn = this.basketPrebookService.Prebook(basket);
            if (prebookReturn.Basket != null)
            {
                this.basketRepository.UpdateBasket(basketToken, prebookReturn.Basket);
            }

            return prebookReturn;
        }

        /// <summary>
        ///     Pre-book component.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="componentToken">The component token.</param>
        /// <returns>
        ///     A basket
        /// </returns>
        public IPrebookReturn PreBookComponent(string basketToken, int componentToken)
        {
            IBasket basket = this.basketRepository.RetrieveBasketByToken(basketToken);
            IPrebookReturn prebookReturn = null;

            IBasketComponent firstOrDefault = basket.Components?.FirstOrDefault(c => c.ComponentToken == componentToken);
            if (firstOrDefault != null && !firstOrDefault.ComponentPreBooked)
            {
                prebookReturn = this.basketPrebookService.Prebook(basket, componentToken);
                if (prebookReturn.Basket != null)
                {
                    if (!prebookReturn.Success)
                    {
                        prebookReturn.Basket.Warnings = new List<string> { "Component Prebook Failed" };
                    }
                    this.basketRepository.UpdateBasket(basketToken, prebookReturn.Basket);
                }
            }

            return prebookReturn;
        }

        public IComponentPaymentCancellationReturn GetCancellationCharges(List<BasketComponentModel> componentModels)
        {
            IBasket basket = new Basket() { Components = new List<IBasketComponent>() };
            var componentPaymentCancellationReturn = new ComponentPaymentCancellationReturn() { ComponentToken = componentModels[0].ComponentToken };
            var charges = new List<ICancellationCharge>();
            var payments = new List<IPayment>();
            foreach (var basketComponentModel in componentModels)
            {
                var componentToken = basketComponentModel.ComponentToken;
                var cachekey = $"CancellationCharges_{componentToken}";
                var result = HttpRuntime.Cache[cachekey] as Tuple<List<IPayment>, List<ICancellationCharge>>;
                if (result == null)
                {
                    IResult component = this.resultService.RetrieveResult(
                        basketComponentModel.SearchToken,
                        basketComponentModel.ComponentToken);
                    if (component != null)
                    {
                        IBasketComponent basketComponent = component.CreateBasketComponent();

                        if (basketComponentModel.SubComponentTokens != null)
                        {
                            List<ISubComponent> selectedSubComponent = new List<ISubComponent>();

                            if (basketComponent.SubComponents != null)
                            {
                                foreach (ISubComponent subComponent in basketComponent.SubComponents)
                                {
                                    if (basketComponentModel.SubComponentTokens.Contains(subComponent.ComponentToken))
                                    {
                                        selectedSubComponent.Add(subComponent);
                                    }
                                }
                                basketComponent.SubComponents = selectedSubComponent;
                            }
                        }
                        ISearchModel searchModel =
                            this.resultService.RetrieveSearchModel(basketComponentModel.SearchToken);
                        basketComponent.SetupComponentSearchDetails(searchModel);
                        basket.Components.Add(basketComponent);
                        if (basket.SearchDetails == null)
                        {
                            basket.SearchDetails = searchModel;

                            if (basket.Rooms == null)
                            {
                                SetupBasketGuests(basket, searchModel);
                            }
                        }
                        IPrebookReturn prebookReturn = null;
                        prebookReturn = this.basketPrebookService.Prebook(basket, basketComponent.ComponentToken);
                        if (prebookReturn.Basket != null)
                        {
                            if (prebookReturn.Success)
                            {
                                basketComponent =
                                    prebookReturn.Basket.Components.FirstOrDefault(
                                        o => o.ComponentToken == basketComponent.ComponentToken);
                                if (basketComponent != null)
                                {
                                    var paymentCharges =
                                        new Tuple<List<IPayment>, List<ICancellationCharge>>(
                                            basketComponent.Payments,
                                            basketComponent.CancellationCharges);

                                    HttpRuntime.Cache.Insert(
                                        cachekey,
                                        paymentCharges,
                                        null,
                                        DateTime.Now.AddMinutes(5),
                                        TimeSpan.Zero);
                                    charges.AddRange(basketComponent.CancellationCharges);
                                    payments.AddRange(basketComponent.Payments);
                                }
                            }
                        }
                    }
                }
                else
                {
                    charges.AddRange(result.Item2);
                    payments.AddRange(result.Item1);
                }
            }
            var collatedPayments = this.CollatePayments(payments);
            var collatedCharges = this.CollateCancellationCharges(charges);
            componentPaymentCancellationReturn.CancellationCharges = collatedCharges;
            componentPaymentCancellationReturn.Payments = collatedPayments;
            return componentPaymentCancellationReturn;
        }

        public List<IPayment> CollatePayments(List<IPayment> payments)
        {
            List<IPayment> collatedPayments =
                payments
                .GroupBy(p => new { dueDate = p.DateDue })
                .Select(g => new Payment() { DateDue = g.Key.dueDate, Amount = g.Sum(p => p.Amount) } as IPayment).ToList();
            return collatedPayments;
        }

        public List<ICancellationCharge> CollateCancellationCharges(List<ICancellationCharge> cancellationCharges)
        {
            var charges = new List<ICancellationCharge>();

            if (cancellationCharges.Any())
            {
                DateTime lastEndDate = cancellationCharges.Max(o => o.EndDate);
                var startDates = cancellationCharges.Select(o => o.StartDate).Distinct().OrderBy(o => o).ToList();

                for (var i = 0; i < startDates.Count; i++)
                {
                    var startDate = startDates[i];
                    var endDate = i < startDates.Count - 1 ? startDates[i + 1].AddDays(-1) : lastEndDate;

                    var charge = new CancellationCharge() { Amount = 0, EndDate = endDate, StartDate = startDate };
                    charges.Add(charge);
                }
            }

            foreach (var cancellationCharge in charges)
            {
                foreach (var charge in cancellationCharges)
                {
                    if ((charge.StartDate >= cancellationCharge.StartDate
                        && charge.StartDate <= cancellationCharge.EndDate)
                        || (charge.EndDate >= cancellationCharge.StartDate
                            && cancellationCharge.StartDate >= charge.StartDate))
                    {
                        cancellationCharge.Amount += charge.Amount;
                    }
                }
            }

            return charges;
        }

        /// <summary>
        ///     Removes the component.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="componentToken">The component token.</param>
        /// <param name="subComponentTokens">Tokens linked to sub components</param>
        /// <returns>A basket</returns>
        public IBasket RemoveComponent(string basketToken, int componentToken, List<int> subComponentTokens = null)
        {
            IBasket basket = this.basketRepository.RetrieveBasketByToken(basketToken);

            var basketCompontentsToRemove = new List<IBasketComponent>();
            if (basket.Components != null)
            {
                foreach (IBasketComponent basketComponent in basket.Components)
                {
                    if (basketComponent.ComponentToken == componentToken)
                    {
                        if (subComponentTokens != null)
                        {
                            var subComponentsToRemove = new List<ISubComponent>();
                            foreach (ISubComponent subComponent in basketComponent.SubComponents)
                            {
                                if (subComponentTokens.Contains(subComponent.ComponentToken))
                                {
                                    subComponentsToRemove.Add(subComponent);
                                }
                            }

                            foreach (ISubComponent subComponent in subComponentsToRemove)
                            {
                                basketComponent.SubComponents.Remove(subComponent);
                            }

                            if (basketComponent.SubComponents.Count == 0)
                            {
                                basketCompontentsToRemove.Add(basketComponent);
                            }
                        }
                        else
                        {
                            basketCompontentsToRemove.Add(basketComponent);
                        }
                    }
                }

                foreach (IBasketComponent basketComponent in basketCompontentsToRemove)
                {
                    basket.Components.Remove(basketComponent);
                }

                this.basketRepository.UpdateBasket(basketToken, basket);
            }

            return basket;
        }

        /// <summary>
        /// Applies the promo code.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>
        /// The basket
        /// </returns>
        public IBasket RemovePromoCode(string basketToken)
        {
            var basket = this.basketRepository.RetrieveBasketByToken(basketToken);

            var updatedBasket = this.promoCodeService.RemovePromocode(basket);

            this.basketRepository.UpdateBasket(basketToken, updatedBasket);

            return updatedBasket;
        }

        /// <summary>
        /// Retrieves the stored basket.
        /// </summary>
        /// <param name="basketStoreId">The basket store identifier.</param>
        /// <returns>The basket</returns>
        public IBasket RetrieveStoredBasket(int basketStoreId)
        {
            iVectorConnectRequest requestBody = this.retrieveStoredBasketFactory.Create(basketStoreId);
            IIVectorConnectRequest ivcRequest = this.connectRequestFactory.Create(requestBody, HttpContext.Current);

            ivci.RetrieveStoredBasketResponse retrieveStoredBasketResponse = ivcRequest.Go<ivci.RetrieveStoredBasketResponse>();

            IBasket basket = null;
            if (retrieveStoredBasketResponse.ReturnStatus.Success)
            {
                var basketJson = retrieveStoredBasketResponse.BasketXML;
                basket = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Basket>(basketJson,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            }
            return basket;
        }

        /// <summary>
        /// Stores the basket.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="basketStoreId">The basket store identifier.</param>
        /// <returns>The basket store id</returns>
        public int StoreBasket(string basketToken, int basketStoreId)
        {
            var basket = this.basketRepository.RetrieveBasketByToken(basketToken);

            iVectorConnectRequest requestBody = this.storeBasketFactory.Create(basket, basketStoreId);
            IIVectorConnectRequest ivcRequest = this.connectRequestFactory.Create(requestBody, HttpContext.Current);

            ivci.StoreBasketResponse storeBasketResponse = ivcRequest.Go<ivci.StoreBasketResponse>();

            return storeBasketResponse.BasketStoreID;
        }

        /// <summary>
        /// Updates the basket.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="basket">The basket.</param>
        /// <returns>Web.Template.Application.Interfaces.Models.IBasket.</returns>
        public IBasket UpdateBasket(string basketToken, IBasket basket)
        {
            this.basketRepository.UpdateBasket(basketToken, basket);
            return basket;
        }

        /// <summary>
        /// Updates the basket component.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="basketComponent">The basket component.</param>
        /// <returns>
        /// A basket
        /// </returns>
        public IBasket UpdateBasketComponent(string basketToken, IBasketComponent basketComponent)
        {
            var basket = this.basketRepository.RetrieveBasketByToken(basketToken);

            basket.Components.ForEach(
                c =>
                {
                    if (c.ComponentToken == basketComponent.ComponentToken)
                    {
                        c = basketComponent;
                    }
                });

            this.basketRepository.UpdateBasket(basketToken, basket);

            return basket;
        }

        /// <summary>
        /// Releases the flight seat lock.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>The return status</returns>
        public string ReleaseFlightSeatLock(string basketToken)
        {
            var response = "";
            var basket = this.basketRepository.RetrieveBasketByToken(basketToken);
            basket.Components.ForEach(
                c =>
                {
                    if (c.ComponentType == ComponentType.Flight)
                    {
                        iVectorConnectRequest requestBody = this.releaseFlightSeatLockFactory.Create(basketToken);
                        IIVectorConnectRequest ivcRequest = this.connectRequestFactory.Create(requestBody, HttpContext.Current);
                        ivci.Flight.ReleaseFlightSeatLockResponse flightSeatLockResponse =
                            ivcRequest.Go<ivci.Flight.ReleaseFlightSeatLockResponse>();

                        response = flightSeatLockResponse.ReturnStatus?.ToString();
                    }
                });
            return response;
        }

        /// <summary>
        /// Setups the basket guests.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="searchModel">The search model.</param>
        private static void SetupBasketGuests(IBasket basket, ISearchModel searchModel)
        {
            if (searchModel?.Rooms != null)
            {
                basket.Rooms = new List<BasketRoom>();
                var roomNumber = 1;
                var guestNumber = 1;
                foreach (Room room in searchModel.Rooms)
                {
                    var basketRoom = new BasketRoom() { Guests = new List<GuestDetail>(), RoomNumber = roomNumber };

                    for (int i = 0; i < room.Adults; i++)
                    {
                        var guest = new GuestDetail() { Type = GuestType.Adult.ToString(), GuestID = guestNumber };
                        guestNumber++;
                        basketRoom.Guests.Add(guest);
                    }

                    for (int i = 0; i < room.Children; i++)
                    {
                        var guest = new GuestDetail()
                        {
                            Type = GuestType.Child.ToString(),
                            GuestID = guestNumber,
                            Age = room.ChildAges?.Count > i ? room.ChildAges[i] : 0
                        };
                        guestNumber++;
                        basketRoom.Guests.Add(guest);
                    }

                    for (int i = 0; i < room.Infants; i++)
                    {
                        var guest = new GuestDetail() { Type = GuestType.Infant.ToString(), GuestID = guestNumber };
                        guestNumber++;
                        basketRoom.Guests.Add(guest);
                    }

                    basket.Rooms.Add(basketRoom);
                    roomNumber++;
                }
            }
        }

        /// <summary>
        /// Removes the existing component.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="componentType">Type of the component.</param>
        private void RemoveExistingComponent(IBasket basket, ComponentType componentType)
        {
            var componentsToRemove = new List<IBasketComponent>();
            foreach (var existingComponent in basket.Components)
            {
                if (existingComponent.ComponentType == componentType
                    && existingComponent.ComponentType != ComponentType.Extra)
                {
                    componentsToRemove.Add(existingComponent);
                }
            }

            foreach (var componentToRemove in componentsToRemove)
            {
                basket.Components.Remove(componentToRemove);
            }
        }
    }
}