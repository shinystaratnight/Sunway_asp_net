namespace Web.Template.Application.Configuration
{
    using Web.Template.Application.Interfaces.Configuration;

    /// <summary>
    /// Class TradeConfiguration.
    /// </summary>
    public class TradeConfiguration : ITradeConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether [display direct debits].
        /// </summary>
        /// <value><c>true</c> if [display direct debits]; otherwise, <c>false</c>.</value>
        public bool DisplayDirectDebits { get; set; }
    }
}