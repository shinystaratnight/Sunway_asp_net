namespace Web.Template.Application.Lookup.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Domain.Entities.Payment;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Payment;

    /// <summary>
    /// Service responsible for retrieving all information concerning payment lookups.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Lookup.Services.IPaymentService" />
    public class PaymentService : IPaymentService
    {
        /// <summary>
        /// The credit card surcharge repository
        /// </summary>
        private readonly ICreditCardSurchargeRepository creditCardSurchargeRepository;

        /// <summary>
        /// The credit card type repository
        /// </summary>
        private readonly ICreditCardTypeRepository creditCardTypeRepository;

        /// <summary>
        /// The currency repository
        /// </summary>
        private readonly ICurrencyRepository currencyRepository;

        /// <summary>
        /// The exchange rate repository
        /// </summary>
        private readonly IExchangeRateRepository exchangeRateRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentService"/> class.
        /// </summary>
        /// <param name="creditCardSurchargeRepository">The credit card surcharge repository.</param>
        /// <param name="creditCardTypeRepository">The credit card type repository.</param>
        /// <param name="currencyRepository">The currency repository.</param>
        /// <param name="exchangeRateRepository">The exchange rate repository.</param>
        public PaymentService(ICreditCardSurchargeRepository creditCardSurchargeRepository, ICreditCardTypeRepository creditCardTypeRepository, ICurrencyRepository currencyRepository, IExchangeRateRepository exchangeRateRepository)
        {
            this.creditCardSurchargeRepository = creditCardSurchargeRepository;
            this.creditCardTypeRepository = creditCardTypeRepository;
            this.currencyRepository = currencyRepository;
            this.exchangeRateRepository = exchangeRateRepository;
        }

        /// <summary>
        /// Gets all credit card surcharges.
        /// </summary>
        /// <returns>
        /// A list of credit card surcharges.
        /// </returns>
        public List<CreditCardSurcharge> GetAllCreditCardSurcharges()
        {
            return this.creditCardSurchargeRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets all credit card types.
        /// </summary>
        /// <returns>
        /// A list of all credit card types.
        /// </returns>
        public List<CreditCardType> GetAllCreditCardTypes()
        {
            return this.creditCardTypeRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets all currencies.
        /// </summary>
        /// <returns>
        /// A list of all currencies.
        /// </returns>
        public List<Currency> GetAllCurrencies()
        {
            return this.currencyRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets all exchange rates.
        /// </summary>
        /// <returns>
        /// A list of all exchange rates.
        /// </returns>
        public List<ExchangeRate> GetAllExchangeRates()
        {
            return this.exchangeRateRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets the credit card surcharge.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// A single surcharge
        /// </returns>
        public CreditCardSurcharge GetCreditCardSurcharge(int id)
        {
            return this.creditCardSurchargeRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the type of the credit card.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// A single credit card type.
        /// </returns>
        public CreditCardType GetCreditCardType(int id)
        {
            return this.creditCardTypeRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the currency.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// A single Currency.
        /// </returns>
        public Currency GetCurrency(int id)
        {
            return this.currencyRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the exchange rate.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// A single exchange rate.
        /// </returns>
        public ExchangeRate GetExchangeRate(int id)
        {
            return this.exchangeRateRepository.GetSingle(id);
        }
    }
}