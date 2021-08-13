namespace Web.Template.Application.Interfaces.Lookup.Services
{
    using System.Collections.Generic;

    using Web.Template.Domain.Entities.Payment;

    /// <summary>
    /// Payment service interface, defining a class responsible for managing access to payment information.
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Gets all credit card surcharges.
        /// </summary>
        /// <returns>A list of credit card surcharges.</returns>
        List<CreditCardSurcharge> GetAllCreditCardSurcharges();

        /// <summary>
        /// Gets all credit card types.
        /// </summary>
        /// <returns>A list of all credit card types.</returns>
        List<CreditCardType> GetAllCreditCardTypes();

        /// <summary>
        /// Gets all currencies.
        /// </summary>
        /// <returns>A list of all currencies.</returns>
        List<Currency> GetAllCurrencies();

        /// <summary>
        /// Gets all exchange rates.
        /// </summary>
        /// <returns>A list of all exchange rates.</returns>
        List<ExchangeRate> GetAllExchangeRates();

        /// <summary>
        /// Gets the credit card surcharge.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A single surcharge</returns>
        CreditCardSurcharge GetCreditCardSurcharge(int id);

        /// <summary>
        /// Gets the type of the credit card.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A single credit card type.</returns>
        CreditCardType GetCreditCardType(int id);

        /// <summary>
        /// Gets the currency.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A single Currency.</returns>
        Currency GetCurrency(int id);

        /// <summary>
        /// Gets the exchange rate.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A single exchange rate.</returns>
        ExchangeRate GetExchangeRate(int id);
    }
}