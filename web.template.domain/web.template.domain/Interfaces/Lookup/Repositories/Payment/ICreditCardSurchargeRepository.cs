namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Payment
{
    using Web.Template.Domain.Entities.Payment;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    ///  Lookup Repository of Credit Card Surcharges.
    /// </summary>
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Generic.ILookupRepository{Web.Template.Domain.Entities.Payment.CreditCardSurcharge}" />
    public interface ICreditCardSurchargeRepository : ILookupRepository<CreditCardSurcharge>
    {
    }
}