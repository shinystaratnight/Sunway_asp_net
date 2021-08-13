namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Payment
{
    using Web.Template.Domain.Entities.Payment;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    ///  Lookup Repository of Currencies.
    /// </summary>
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Generic.ILookupRepository{Web.Template.Domain.Entities.Payment.Currency}" />
    public interface ICurrencyRepository : ILookupRepository<Currency>
    {
    }
}