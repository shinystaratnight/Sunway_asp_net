namespace Web.Template.Data.Lookup.Repositories.Payment
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Payment;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Payment;

    /// <summary>
    /// Cached Repository Exposing Booking Credit Card Types.
    /// </summary>
    /// <seealso cref="Web.Template.Data.Lookup.Repositories.Generic.CachedLookupRepository{Web.Template.Domain.Entities.Payment.CreditCardType}" />
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Payment.ICreditCardTypeRepository" />
    public class CreditCardTypeRepository : CachedLookupRepository<CreditCardType>, ICreditCardTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreditCardTypeRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public CreditCardTypeRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}