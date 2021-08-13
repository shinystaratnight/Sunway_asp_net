namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Payment
{
    using Web.Template.Domain.Entities.Payment;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    ///  Lookup Repository of Credit Card Types.
    /// </summary>
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Generic.ILookupRepository{Web.Template.Domain.Entities.Payment.CreditCardType}" />
    public interface ICreditCardTypeRepository : ILookupRepository<CreditCardType>
    {
    }
}