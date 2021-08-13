namespace Web.Template.Data.Lookup.Repositories.Payment
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Payment;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Payment;

    /// <summary>
    /// Cached Repository Exposing Currencies.
    /// </summary>
    /// <seealso cref="Web.Template.Data.Lookup.Repositories.Generic.CachedLookupRepository{Web.Template.Domain.Entities.Payment.Currency}" />
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Payment.ICurrencyRepository" />
    public class CurrencyRepository : CachedLookupRepository<Currency>, ICurrencyRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public CurrencyRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}