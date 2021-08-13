namespace Web.Booking.API.Lookup
{
    using System.Collections.Generic;
    using System.Web.Http;

    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Domain.Entities.Payment;

    /// <summary>
    /// Payment controller responsible for exposing payment lookup to the front end.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class PaymentController : ApiController
    {
        /// <summary>
        /// The payment service
        /// </summary>
        private readonly IPaymentService paymentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentController"/> class.
        /// </summary>
        /// <param name="paymentService">The payment service.</param>
        public PaymentController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        /// <summary>
        /// Gets all credit card surcharges.
        /// </summary>
        /// <returns>All credit card surcharges</returns>
        [Route("api/payment/creditcardsurcharge")]
        [HttpGet]
        public List<CreditCardSurcharge> GetAllCreditCardSurcharges()
        {
            return this.paymentService.GetAllCreditCardSurcharges();
        }

        /// <summary>
        /// Gets all credit card types.
        /// </summary>
        /// <returns>All credit card types</returns>
        [Route("api/payment/creditcardtype")]
        [HttpGet]
        public List<CreditCardType> GetAllCreditCardTypes()
        {
            return this.paymentService.GetAllCreditCardTypes();
        }

        /// <summary>
        /// Gets all currencies.
        /// </summary>
        /// <returns>All currencies.</returns>
        [Route("api/payment/currency")]
        [HttpGet]
        public List<Currency> GetAllCurrencies()
        {
            return this.paymentService.GetAllCurrencies();
        }

        /// <summary>
        /// Gets all exchange rates.
        /// </summary>
        /// <returns>All exchange rates</returns>
        [Route("api/payment/exchangeRate")]
        [HttpGet]
        public List<ExchangeRate> GetAllExchangeRates()
        {
            return this.paymentService.GetAllExchangeRates();
        }

        /// <summary>
        /// Gets the credit card surcharge.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A credit card surcharge for the given id</returns>
        [Route("api/payment/creditcardsurcharge/{id}")]
        [HttpGet]
        public CreditCardSurcharge GetCreditCardSurcharge(int id)
        {
            return this.paymentService.GetCreditCardSurcharge(id);
        }

        /// <summary>
        /// Gets the type of the credit card.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A credit card type for the given id.</returns>
        [Route("api/payment/creditcardtype/{id}")]
        [HttpGet]
        public CreditCardType GetCreditCardType(int id)
        {
            return this.paymentService.GetCreditCardType(id);
        }

        /// <summary>
        /// Gets the currency.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>a currency for the given id.</returns>
        [Route("api/payment/currency/{id}")]
        [HttpGet]
        public Currency GetCurrency(int id)
        {
            return this.paymentService.GetCurrency(id);
        }

        /// <summary>
        /// Gets the exchange rate.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>an exchange rate for the given id</returns>
        [Route("api/payment/exchangeRate/{id}")]
        [HttpGet]
        public ExchangeRate GetExchangeRate(int id)
        {
            return this.paymentService.GetExchangeRate(id);
        }
    }
}