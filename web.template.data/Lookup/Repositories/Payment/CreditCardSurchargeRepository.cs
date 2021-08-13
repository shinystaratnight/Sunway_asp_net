namespace Web.Template.Data.Lookup.Repositories.Payment
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Payment;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Payment;

    /// <summary>
    /// Cached Repository Exposing Credit Card Surcharges.
    /// </summary>
    /// <seealso cref="Web.Template.Data.Lookup.Repositories.Generic.CachedLookupRepository{Web.Template.Domain.Entities.Payment.CreditCardSurcharge}" />
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Payment.ICreditCardSurchargeRepository" />
    public class CreditCardSurchargeRepository : CachedLookupRepository<CreditCardSurcharge>, 
                                                 ICreditCardSurchargeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreditCardSurchargeRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public CreditCardSurchargeRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}