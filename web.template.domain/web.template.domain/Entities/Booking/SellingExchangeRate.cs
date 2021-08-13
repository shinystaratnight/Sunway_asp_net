namespace Web.Template.Domain.Entities.Booking
{
    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Class SellingExchangeRate.
    /// </summary>
    public class SellingExchangeRate : ILookup
    {
        /// <summary>
        /// Gets or sets the currency identifier.
        /// </summary>
        /// <value>The currency identifier.</value>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the rate.
        /// </summary>
        /// <value>The rate.</value>
        public decimal Rate { get; set; }
    }
}