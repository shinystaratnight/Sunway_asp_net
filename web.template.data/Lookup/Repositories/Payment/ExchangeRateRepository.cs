namespace Web.Template.Data.Lookup.Repositories.Payment
{
    using System.Data.Entity;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Payment;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Payment;

    /// <summary>
    /// Cached Repository Exposing Exchange Rates.
    /// </summary>
    /// <seealso cref="Web.Template.Data.Lookup.Repositories.Generic.CachedLookupRepository{Web.Template.Domain.Entities.Payment.ExchangeRate}" />
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Payment.IExchangeRateRepository" />
    public class ExchangeRateRepository : CachedLookupRepository<ExchangeRate>, IExchangeRateRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeRateRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public ExchangeRateRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}