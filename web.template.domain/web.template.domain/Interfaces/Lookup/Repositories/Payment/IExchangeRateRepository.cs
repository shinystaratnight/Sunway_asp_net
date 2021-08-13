namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Payment
{
    using Web.Template.Domain.Entities.Payment;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    ///  Lookup Repository of Exchange rates.
    /// </summary>
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Generic.ILookupRepository{Web.Template.Domain.Entities.Payment.ExchangeRate}" />
    public interface IExchangeRateRepository : ILookupRepository<ExchangeRate>
    {
    }
}